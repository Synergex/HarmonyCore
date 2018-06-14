# Harmony Core
Harmony Core is a framework that consists of libraries, conventions, and [CodeGen](https://github.com/SteveIves/CodeGen) templates that can be used to expose Synergy logic and data as a RESTful web service using [OData](http://www.odata.org/) and [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/).

### Why REST?
We're opinionated. We think [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) is the best possible way to expose Synergy data and logic over a network. Our customers are most successful when they turn the logic and data they've worked on for the last 40 years into easily consumable black boxes of functionality. The best black boxes can be used without a proprietary protocol and without needing to make a complex series of calls that have little or no relation to one another. Maintaining state between calls is fraught with peril. It is difficult to protect a system sufficiently from misuse when a web service consumer is allowed to partially complete a transaction and then disappear. When possible, web service operations should be [idempotent](https://en.wikipedia.org/wiki/Idempotence), and if that isn't possible they should at least strive to be [atomic](https://en.wikipedia.org/wiki/Atomicity_(database_systems)). 

### Why OData?
"Open Data Protocol (OData) is an open protocol which allows the creation and consumption of queryable and interoperable RESTful APIs in a simple and standard way" ([Wikipedia](https://en.wikipedia.org/wiki/Open_Data_Protocol), 6/14/2018). When using OData for web services, we emit plain [JSON](http://json.org/), which is easily consumable by nearly any client you can imagine. OData lets you expose the entities as operations your code supports, but it frees you from explicitly having to make every operation variant. For example, instead of writing code that exposes all of a customer's orders, and then writing separate code that exposes all of a customer's orders that meet a certain criteria, with OData you just expose the customer orders and let Entity Framework (EF) Core (an ASP<span></span>.NET Core component) and OData handle the filtering.
```
GET serviceRoot/Orders?$filter=Supplier eq 'Scott'
```

But it's more than just filtering. When you've defined relationships in your Synergy Repository, OData queries can actually expand those relations using a $expand:

```
GET serviceRoot/Orders('1234')?$expand=CustomerOrders
```

This example uses the primary key '1234' to retrieve an order, and the response will include all other orders this customer has made. This is done using our EF Core Provider, which ultimately just translates EF Core operations into Synergy Select class operations. There are several built-in functions and predicates supported by OData, and you can learn more about them [here](http://www.odata.org/getting-started/basic-tutorial/). These functions dramatically reduce the amount of work you need to put into exposing Synergy logic and data via a great web service.

You might be starting to wonder, what about security? Maybe you don't want to allow all of the built-in functions and predicates because certain users shouldn't have access to certain types of data. OData supports [query validation](https://github.com/OData/WebApi/tree/master/src/Microsoft.AspNet.OData.Shared/Query/Validators) so you can implement a query validator to ensure that your extensive query functionality can only be used in the ways you deem appropriate.


### Why ASP<span></span>.NET Core?
ASP<span></span>.NET Core is a ground-up rewrite of the entire web stack for .NET. Microsoft has applied all the lessons learned over the years with large-scale deployment of ASP<span></span>.NET. What they've come up with runs on both the full .NET Framework and .NET Core, and they've significantly improved the performance and general scalability characteristics. Unlike the original ASP<span></span>.NET, everything is being developed in the open on [GitHub](https://github.com/aspnet/Home) with significant community engagement and contributions. 

### Why Entity Framework Core?
Entity Framework Core, commonly referenced as EF Core, is a full rewrite of Entity Framework. Much the like other libraries microsoft has been naming "Core", there has been a significant focus on portability, performance, and extensibility. The team has an explicit goal to provide support for non-relational databases (of which Synergy DBMS is one). This is a big change from the SQL-Server-centric Entity Framework 6.0 release that preceded EF Core. Because our Synergy Select class is capable of performing all the underlying read operations that EF Core supports, it is a great fit for accessing DataObjects. Write/update/delete operations are supported in our EF Core provider, but instead using [FileIO](FileIO.md) classes to enable custom I/O routines. You can read more about our EF Core Provider [here](EntityFramework.md).

## Foundational Concepts
### DataObject
DataObject is the fundamental building block in Harmony Core. DataObjects are usually CodeGenerated from a structure in a [Synergy repository](http://docs.synergyde.com/index.htm#rps/rpsChap1Whatisrepository.htm) and they inherit from `Harmony.Core.DataObjectBase`. DataObjects are generated with metadata that tells other parts of Harmony Core the size, type, and location (position in record) of each of your fields. You can read a more in-depth exploration of DataObject [here](DataObject.md).
### Dependency Injection
At its most basic level, dependency injection consists of three parts. The first is an `IServiceProvider`, if you have an instance of `IServiceProvider`, you can ask it to provide you with an instance of a `Type` it has been registered with previously. The second part is the class that needs to be constructed. For example, if your constructor takes a `String` parameter, when it is constructed it will receive an instance of a `String` as registered in your `IServiceProvider`. The third part, the part that glues it all together, is `ActivatorUtilities.CreateInstance<YourTypeGoesHere>(IServiceProvider)`. `CreateInstance` actually takes all the services provided by the passed-in provider and creates a new instance of the type you supplied using a type argument. For additional information, check the [ASP.NET Core documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) or for a more general overview, Wikipedia has a fairly complete article [here](https://en.wikipedia.org/wiki/Dependency_injection).

### Contexts
Harmony Core provides various implementations of [IContextBase](Reference/IContextBase.md) that can be used to provide highly performant, thread-safe access to your Synergy logic and data. You can read more about contexts [here](Contexts.md).

### ODataController
`Microsoft.AspNet.OData.ODataController` is the base class for [controllers](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/actions?view=aspnetcore-2.1#what-is-a-controller) in OData. Methods that you want to be routable should be public and generally are named based on what [HTTP verb](http://www.restapitutorial.com/lessons/httpmethods.html) you will use to call it. Construction of controllers supports dependency injection, which means you can declare dependencies as parameters to your public constructor method. If requests for those types can be satisfied by the `IServiceProvider` your constructor will be called with, those parameters fill out. The following is an example method that will be invoked when the controller is called using HTTP GET without any parameters:

```
{ODataRoute("Orders")}
{EnableQuery(MaxExpansionDepth=3, MaxSkip=10, MaxTop=5, PageSize=4)}
public method Get, @IActionResult
proc
	mreturn Ok(DBContext.Orders)
endmethod
```
This is also an example of how you can provide an explicit route path and enable (and control) the way the underlying framework can satisfy OData queries.

### Logging
### ASP .Net Core Middleware

## Quick Starts