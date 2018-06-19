# Simple OData with Harmony Core and Synergy
## Prerequisites
* [CodeGen](https://github.com/SteveIves/CodeGen)
* [Visual Studio 2017 15.7 or later](https://www.visualstudio.com/vs/community/) - When installing this you must select the desktop, web and .net core workloads
* [Synergy DBL Integration For Visual Studio 10.3.5f or later](www.synergyde.com)
* [Postman](https://www.getpostman.com/)

## Intoduction
This quickstart is going to walk through creating a simple OData Web Service using Synergy DBL, Harmony Core, Entity Framework Core, ASP .Net Core, and the OData WebAPI library. All of the libraries come from [nuget.org](https://www.nuget.org). Once we've built and are running our web service, we're going to test it out using Postman. Postman is a useful tool when developing web services because it lets you easily test your web service without having to write a client app.

## Getting started

First up we're going to open Visual Studio 2017 and create a new Console App

![Create New Console App](link_to_create_new_project_dialog_screenshot "Create New Console App")

This is a full .Net Framework Console App, we could have used a .Net Core Console app but for now, there are less rough edges inside SDI when working with the full framework. Now that we've got a project created we need to add a few nuget package references, so open up the nuget package manager

![Nuget Package Manager](link_to_nuget_package_manager_dialog_screenshot "Add Nuget Package")

We're going to add to following packages, several of these packages are marked as prerelease so you will need to check the box to allow prerelease software from the nuget package manager.
* Microsoft.AspNetCore.Mvc.Core 2.1.0
* Microsoft.AspNetCore.OData 7.0.0-beta4
* Microsoft.EntityFrameworkCore 2.1.0
* Harmony.Core 0.1.0 - this package doesn't exist
* Harmony.Core.NetFx 0.1.0 - this package doesn't exist
* Harmony.Core.AspNetCore 0.1.0 - this package doesn't exist
* Harmony.Core.EF 0.1.0 - this package doesn't exist

In this very simple example we're only going to code generate the DataObject. We're doing this because we think its important for users of Harmony Core to understand what's going on behind the scenes, even if most of the time you can just let CodeGen do the heavy lifting. Assuming you've got the HarmonyCore.Test.Repository project, you can run CodeGen against the DataObject.tpl using the following command line.

`CodeGen -t DataObject.tpl -some additional options to actually build this file`
`CodeGen -t DataObjectMetaData.tpl -some additional options to actually build this file`

This should give us an Orders.dbl (and OrdersMetadata.dbl) file that we can just add to the project we've created.
The first code we write will be our Startup class, ASP .Net Core uses a class (usually called Startup) to do most of the configuration heavy lifting. for more on the Startup class check out this [ASP .Net Core Fundementals Page](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup). We're just going to use a small portion of the functionality that can be crammed into a Startup class.

Our Startup class is composed of two parts, the first one is the ConfigureServices method. This is where we perform any [dependency injection](../DependencyInjection.md) related setup.

```
public class Startup
	public method ConfigureServices, void
		services, @IServiceCollection 
	proc
		data channelManager = new FileChannelManager() 
		data objectProvider = new DataObjectProvider(channelManager)
		objectProvider.AddDataObjectMapping<Orders>("DAT:orders.ddf", FileOpenMode.UpdateRelative)
		services.AddSingleton<FileChannelManager>(channelManager)
		services.AddSingleton<IDataObjectProvider>(objectProvider)
		services.AddSingleton<DbContextOptions<MyDBContext>>(new DbContextOptions<MyDBContext>())
		services.AddScoped<MyDBContext, MyDBContext>()
		services.AddOData()
		services.AddMvcCore()
	endmethod
endclass
```

Now for the second part of our Startup class we need to add a Configure method. In our case we're using it to set up supported OData operations, let it know about our data types and give it a url root to route requests to. It looks like this.

```
public method Configure, void
	app, @IApplicationBuilder
	env, @IHostingEnvironment
proc
	data model = EdmModelBuilder.GetEdmModel()
	lambda MVCBuilder(builder)
	begin
		builder.Select().Expand().Filter().OrderBy().MaxTop(100).Count()
		builder.MapODataServiceRoute("odata", "odata", model)
	end
	app.UseMvc(MVCBuilder)
endmethod
```

You might be wondering what EdmModelBuilder and MyDBContext are. you're going to write those classes shortly. EdmModelBuilder is the guts of informing OData about all of the types we're going to expose. For our very basic single type data model its going to look like this.

```
public class EdmModelBuilder
	private static mEdmModel, @IEdmModel
	public static method GetEdmModel, @IEdmModel
	proc
		if(mEdmModel == ^null)
		begin
			data builder = new ODataConventionModelBuilder()
			builder.EntitySet<Orders>("Orders")
			mEdmModel = builder.GetEdmModel()
		end

		mreturn mEdmModel
	endmethod

endclass
```

GetEdmModel can get quite a bit more complicated but in real applications we would be code generating it using the structures and relations contained in your Synergy Repository.

MyDBContext is the entry point that our controllers use into Entity Framework Core. In this basic example there isnt much going on, just declaring a collection for our Orders Type and some boilerplate configuration of Entity Framework Core itself.

```
public class MyDBContext extends DbContext
	
	mDataProvider, @IDataObjectProvider
	public method MyDBContext
		options, @DbContextOptions<MyDBContext>
		dataProvider, @IDataObjectProvider
		endparams
		parent(options)
	proc
		mDataProvider = dataProvider
	endmethod


	public readwrite property Orders, @DbSet<Orders>

	protected override method OnConfiguring, void
		opts, @DbContextOptionsBuilder
	proc
		HarmonyDbContextOptionsExtensions.UseHarmonyDatabase(opts, mDataProvider)
	endmethod

	protected override method OnModelCreating, void
		parm, @ModelBuilder
	proc
		parm.Ignore(^typeof(AlphaDesc))
		parm.Ignore(^typeof(DataObjectMetadataBase))
		parent.OnModelCreating(parm);
	endmethod

endclass
```

So much configuration, but we're finally getting to the actual controller now. We're going to provide simple read only OData access to our Orders so there isnt much here but if you want to know more about the conventions used for controllers in ASP .Net Core check [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/actions). Our controller is going to look like this.

```
public class OrdersController extends ODataController
	public readwrite property DBContext, @MyDBContext

	public method OrdersController
		dbContext, @OrdersController
	proc
		this.DBContext = dbContext
	endmethod
	{ODataRoute("Orders")}
	{EnableQuery}
	public method Get, @IActionResult
	proc
		mreturn Ok(DBContext.Orders)
	endmethod
endclass
```

This controller class is going to get created once per request, and the parameter you see in the constructor will be provided by the Dependency Injection framework inside ASP .Net Core itself. returning `Ok(DBContext.Orders)` is handing the OData library our EF Core `DBSet` letting it know that this should return HTTP 200 Ok but because we've enabled queries there are a bunch of operations the caller can perform without us having to explicitly write code to support it. 

The last step in creating our web service is to fill in the mainline inside Program.dbl, this glues everything together and ends up actually does the hosting part.

```
main
proc
    data host = new WebHostBuilder()
&           .UseKestrel()
&           .UseContentRoot(Directory.GetCurrentDirectory())
&           .UseStartup<Startup>()
&           .Build()
 
    host.Run()
endmain
```

If we make sure that DAT is set in our environment, and that inside that folder you have orders.ddf from the sample data files in this repository, you should now be able to run your console app. By default Kestrel will host your app at http://localhost:5000 and thats good enough for this guide. If you want to do something different check out this guide to hosting with [Kestrel](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel). 

Onward to testing! Fire up postman and do something bla bla finish this quick start by showing how to call 

