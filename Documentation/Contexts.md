# Contexts
For synergy applications of any significant size and complexity, developers will find it necessary to manage the lifetimes of non memory resources. For maximum performance with SDMS, this means you should minimize opening and closing of files. Similarly for SQLConnection channels it is much more performant to reuse than to reopen. 

### Basic SDMS
In the most basic case for SDMS, we can just use a [file channel pool](Reference/IFileChannelManager.md) this takes care of opening a file channel for a given mode to a given filename. If there is already an available channel that matches those parameters it will be used, otherwise it will make a call to [open](http://docs.synergyde.com/#lrm/lrmChap4OPEN.htm). When you're finished with a channel, you can just call `ReturnChannel` to put it back into the pool for the next request to make use of.

### Basic State
In addition to the SDMS channels you might have non trivial setup that needs to be carried out prior to processing requests. This could involve reading configuration data or opening SQLConnection channels. If that state can be placed into an object that implements [IContextBase](Reference/IContextBase.md), we can pool it using a [Free Threaded Context Pool](Reference/FreeThreadedContextPool.md). This allows for your state object to be created and initialized as needed, but reused by later requests rather than being thrown away. If you need these state objects to be created per user session, you should also implement [ISessionStickyContext and ITimeoutContext](Reference/StickyContext.md). That way, your state object will follow around requests made by the same user, when passed the same [cookie](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state). `ITimeoutContext` is needed because HTTP(S) is a stateless protocol and a user can disappear at anytime. With an ITimeoutContext you can control how long you want the session to stay alive between requests.

### xfServer
If there isn't a significant load on your server its possible to use xfServer with a plain file channel pool. If you need additional scaling beyond a single connection to xfServer it's required that you make a call to [s_server_thread_init](http://docs.synergyde.com/#lrm/lrmChap9SSERVERTHREADINIT.htm). Once you've called `s_server_thread_init` channel management becomes significantly more complex. Channels opened on a given thread must now only be used from that thread. This leads to the next evolution of the file channel pool, just make a separate on for each thread! Unfortunately in most environments we aren't really in control of what thread we are called on, when being asked to process a request. We need to be able to call `s_server_thread_shutdown` if the thread is being shut down, but we dont control the lifetime of those threads. This is where a [Threaded Context Pool](Reference/ThreadedContextPool.md) is useful. Rather than relying on ASP .Net Core to choose what thread we should process our request on. We can force requests to be processed on a thread controlled by our context pool. calling `IApplicationBuilder.UseEagerContext<MyContextTypeName>` passing an instance of a ThreadedContextPool, will take advantage of ASP .Net Core Middleware to ensure that if a Controller has a dependency on a class the extends `ThreadedContextBase` our request will be handled on a thread owned by the `ThreadedContextPool`. If configured, threads created by `ThreadedContextPool` will have `s_server_thread_init` and `s_server_thread_shutdown` called on them when needed. This leaves your code able to make use of a per thread `IFileChannelManager` without worrying about getting threads mixed up.

### Isolation
Using global data sections, commons, static records, static class fields or hard coded channel numbers results in code that cannot be run on multiple threads at the same time. Multi-threading is an important part of achieving reasonable scalability in ASP .Net Core, so solving for non-threadsafe code is equally as important. In the Full .Net Framework we can use an[AppDomain](https://msdn.microsoft.com/en-us/library/system.appdomain) to isolate running code from almost all side effects that might be caused by other running `AppDomains`. This will not isolate environment variable changes made within a process. In order to make AppDomain isolation simple and performant, you can use [AppDomainContextPool](Reference/AppDomainContextPool.md) to managed your context class that extends `AppDomainContextBase`. The way the isolation works, you can call your existing non-threadsafe code within your context class as though it were the only code running in the process. Consumers of your context class will get an instance from the `AppDomainContextPool` and interact with it as though the underlying code were actually threadsafe. An `AppDomain` can be created or unloaded on demand. Using an `AppDomain` within a non UI application will result in taking an additional Synergy Runtime license.

.Net Core adds some additional complexity to isolation. `AppDomain` exists in .Net Core, but for the purpose of code isolation it is non functional. The recommended path is to instead use [AssemblyLoadContext](https://github.com/dotnet/coreclr/blob/master/Documentation/design-docs/assemblyloadcontext.md). Unfortunately `AssemblyLoadContext` doesn't exist in the Full .Net Framework as of 4.7.2. So this solution is .Net Core specific. There is currently one additional limitation, it is not possible to unload an `AssemblyLoadContext`, making it a requirement pool and reuse them. There are some upsides to `AssemblyLoadContext`, it is significantly more lightweight and poses no performance cost to making calls from code running in one context to another. There is no marshaling of arguments and much less memory is taken up by a running `AssemblyLoadContext`. 

### Traditional Synergy
If you have logic written in Synergy but one of the following conditions exits
* you cannot compile it with Synergy .Net
* you need to run it on OpenVMS
* you need to run it on a remote system
* you need to run it in a separate process

You can make use of [IDynamicCallProvider](Reference/DynamicCallProvider.md) to invoke that logic passing arguments and returning data. As with most of the other concepts in this library we can pool these, in this case using either `RemoteExternalContextPool` or `ProcessExternalContextPool` depending on if we're communicating over SSH to a remote machine or creating a process locally. Because this scenario potentially involves multiple machines and at least separate processes, things can get more difficult to follow. 

For a local process the flow is relatively straightforward. The TraditionalBridge project offers all of the generic support functionality that is needed. This is logging, json parsing, json writing and basic routine dispatching. The code you want to run can be exposed one of two ways, If you have [xfServer+ routine attributes](http://docs.synergyde.com/index.htm#xfnl/xfnlChap2Usingattributestodefinesynergymethods.htm) or a [method catalog](http://docs.synergyde.com/index.htm#xfnl/xfnlChap2Smcoverview.htm) you can generate strongly typed dispatch stubs with CodeGen. This is the most performant, featured and ultimately reliable method of dispatching routines. If you cant use CodeGen to create dispatch stubs, the generic routine dispatcher will do its best to map the arguments you pass into a function, subroutine or method with the following caveats, structures and class arguments must be wrapped by or derived from [DataObjectBase](DataObject.md). Arrays can only be passed as a [ArrayList](http://docs.synergyde.com/index.htm#lrm/lrmChap10SYSTEMCOLLECTIONSARRAYLIST.htm) that can only contain classes derived from `DataObjectBase` or primitives like string, a, d, and i. Your code should be compiled into a dbr and should reference the TraditionalBridge project. The mainline of your dbr needs to look something like this.

```
main
	record
		dispatcher, @MyDispatcher
		ttChan, i4, 0
		jsonReader, @Json.Json
		jsonVal, @Json.JsonValue
proc
	xcall flags(0101010010)
	open(ttChan, O, "TT:")
	puts(ttChan, "READY" + %char(13)+ %char(10))
	dispatcher = new MyDispatcher()
	dispatcher.Dispatch(ttChan)
endmain
```

Where `MyDispatcher` is a class that you've code generated from a combination of repository structures and potentially xfServer+ method catalog data. `ProcessExternalContextPool` will take care of creating, initializing, recycling and destroying the spawned dbr's that will execute your logic. Spawned processes will each take a synergy runtime license and it will be executed as the same user as the running webserver.

For remote processes the flow is identical once the remote process is started, but first we have to make a connection and do any authentication. There are several security flows that can occur

* Preconfigured username/password is stored in configuration data (insecure only recommended for development scenarios!!!).
 *Preconfigured username/password is stored in a secure configuration store such as [app-secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio) or [key-vault](https://azure.microsoft.com/en-us/services/key-vault/)
* SSH Private keys are stored in a secure configuration store as above
* In a webservice that is authenticated against an Azure Active Directory or Active Directory server, its possible to pass the authentication token through the web service using JWT. When the request is made to create a process, the JWT from the request can be used to acquire AltSecurityIdentities from the AD account. These security credentials are then used to log into SSH and create the target process. This security flow generally precludes the use of a pool of pre-created processes/connections. Processes can be left running (and reused by the same user) for a predetermined amount of time but because they cannot be shared or pre-created there may be negative performance implications.

In any case your interaction point with the library will be a [PasswordAuthenticationMethod or PrivateKeyAuthenticationMethod](https://github.com/sshnet/SSH.NET#multi-factor-authentication) passed to the constructor of your `RemoteExternalContextPool`. Once you're connected we run the supplied commands line remotely. This command line can do whatever environment setup is needed but should result in running `dbr` or `dbs` against the dbr you created earlier.
