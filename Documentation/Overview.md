# Harmony Core
Harmony Core is a framework that consists of libraries, conventions and [CodeGen](https://github.com/SteveIves/CodeGen) templates that can be used to expose synergy logic and data as a RESTful Web Service using [OData](http://www.odata.org/) and [ASP.Net Core](https://docs.microsoft.com/en-us/aspnet/core/).

### Why REST?
We're opinionated, we think [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) is the best possible way to expose synergy data and logic over a network. Our customers are most successful when they turn the logic and data they've worked on for the last 40 years into an easily consumable black box of functionality. The best black boxes can be used without a proprietary protocol and without needing to make a complex series of calls that have little or no relation to one another. Maintaining state between calls is fraught with peril. It is difficult for a programmer to sufficiently protect a system from misuse when a web service consumer is allowed to partially complete a transaction and then disappear. When possible web service operations should be [Idempotent](https://en.wikipedia.org/wiki/Idempotence), and if that isn't possible they should at least strive to be [Atomic](https://en.wikipedia.org/wiki/Atomicity_(database_systems)). 

### Why OData?
Open Data Protocol (OData) is an open protocol which allows the creation and consumption of queryable and interoperable RESTful APIs in a simple and standard way. When using OData for web services we emit plain [JSON](http://json.org/) which is easily consumable by any client you might imagine. OData lets you expose the entities are operations your code supports, but it frees you from having to explicitly make every operation variant. For example instead of needing to write code that exposes all of a customers Orders, and separate code that exposes all of a customers Orders that meet a certain criteria, using OData you just expose the customer orders and let Entity Framework Core and OData handle the filtering.
```
GET serviceRoot/Orders?$filter=Supplier eq 'Scott'
```

It's not just filtering, when you've defined relationships in your Synergy Repository, OData queries can actually expand those relations using a $expand

```
GET serviceRoot/Orders('1234')?$expand=CustomerOrders
```

This will get an order using the primary key '1234', then also include in the response, all of the other orders this customer has made. This is done using our Entity Framework Core Provider but ultimately that just translates EF Core operations into Synergy Select class operations. There are several built in functions and predicates supported by OData and you can learn more about them [here](http://www.odata.org/getting-started/basic-tutorial/). These functions dramatically reduce the amount of work you need to put into exposing a great web service.

You might be reading this and starting to wonder, what about security? Maybe you dont want to allow all of the built in functions and predicates because certain users shouldnt have access to certain types of data. OData supports [query validation](https://github.com/OData/WebApi/tree/master/src/Microsoft.AspNet.OData.Shared/Query/Validators). By implementing a query validator you can ensure that your extensive query functionality can only be used in the ways you deem appropriate.


### Why ASP .Net Core?
ASP .Net Core is a ground up rewrite of the entire web stack for .Net. Microsoft has applied all of the lessons learned over the years with large scale deployment of ASP .Net. What they've come up with runs on both the full .Net Framework and .Net Core and They've significantly improved the performance and general scalability characteristics. Unlike the original ASP .Net, everything is being developed in the open on [GitHub](https://github.com/aspnet/Home) with significant community engagement and contributions. 

### Why Entity Framework Core?
Entity Framework Core, commonly referenced as EF Core is a full rewrite of Entity Framework. Much the like other libraries microsoft has been naming 'Core', there has been a significant focus on portability, performance and extensibility. The team has an explicit goal to provide support for non relational databases (of which SDBMS is one), this is a big change from the MS SQL centric EF 6.0 release that preceded EF Core. Because our Synergy Select class is capable of performing all of the underlying read operations that EF Core supports, it has ended up being a great fit for accessing DataObjects. Write/Update/Delete operations are supported in our EF Core provider, but instead using [FileIO](FileIO.md) classes to enable custom io routines. You can read more about our EF Core Provider [here](EntityFramework.md)

## Foundational Concepts
### DataObject
DataObjects are the fundamental building block in HarmonyCore. DataObjects are usually CodeGenerated from a structure in a [Synergy Repository](http://docs.synergyde.com/index.htm#rps/rpsChap1Whatisrepository.htm) and they inherit from `Harmony.Core.DataObjectBase`. DataObjects are also generated with metadata that tells other parts of HarmonyCore, how big, what type and where in the record all of your fields are. You can read a more in depth exploration of DataObject [here](DataObject.md)
### Dependency Injection
At its most basic level, dependency injection is just three parts. The first part is an `IServiceProvider`, if you have an instance of `IServiceProvider` you can ask it to provide you with an instance of a `Type` that it has previously been registered with. The second part is your class that needs to be constructed. If for example your constructor took a `String` parameter, when it gets constructed it would receive an instance of a `String` as registered in your `IServiceProvider`. The third part glues it all together and it's `ActivatorUtilities.CreateInstance<YourTypeGoesHere>(IServiceProvider)`. `CreateInstance` actually takes all of the services provided by the passed in provider and creates a new instance of the type you supplied using a type argument. For additional information please check the [ASP.Net Core documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) or for a more general overview wikipedia has a fairly complete article [here](https://en.wikipedia.org/wiki/Dependency_injection).

### Contexts
Harmony Core provides various implementations of [IContextBase](Reference/IContextBase.md) that can be used to provide highly performant, thread safe access to your synergy logic and data. You can read more about Contexts [here](Contexts.md).

### ODataController
`Microsoft.AspNet.OData.ODataController` is the base class for [controllers](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/actions?view=aspnetcore-2.1#what-is-a-controller) in OData. Methods that you are intending to be routable should be public and generally are named based on what [HTTP Verb](http://www.restapitutorial.com/lessons/httpmethods.html) you intend it to be called using. Construction of controllers is performed with support for dependency injection. This means you can declare dependencies as parameters to your public constructor method, if requests for those types can be satisfied by the `IServiceProvider` your constructor will be called with those parameters fill out. Below is an example method that will be invoked when the controller is called using HTTP GET without passing any parameters.

```
{ODataRoute("Orders")}
{EnableQuery(MaxExpansionDepth=3, MaxSkip=10, MaxTop=5, PageSize=4)}
public method Get, @IActionResult
proc
	mreturn Ok(DBContext.Orders)
endmethod
```
This is also an example of providing an explicit route path and enabling but controlling the way the underlying framework is willing to satisfy OData queries.

### Logging
### ASP .Net Core Middleware

## Quick Starts