using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Models;
using System;
using System.Data;
using System.Linq;

namespace Services.Test.CS
{
    [TestClass]
    public class UnitTest1
    {

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            UnitTestEnvironment.AssemblyInitialize(context);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            UnitTestEnvironment.AssemblyCleanup();
        }

        [TestMethod]
        public void DualContextUpdate()
        {
            var startupClass = new Startup(null, null);
            var startupServices = new ServiceCollection();
            startupClass.ConfigureServices(startupServices);
            using (var sp = startupServices.BuildServiceProvider())
            {
                using (var context = sp.GetService<Services.Models.DbContext>())
                {
                    foreach (var customer in context.Customers.Where(cust => cust.Name.EndsWith("Nursery")))
                    {
                        customer.ZipCode = 0;
                    }


                    var startupClass2 = new Startup(null, null);
                    var startupServices2 = new ServiceCollection();
                    startupClass2.ConfigureServices(startupServices2);
                    using (var sp2 = startupServices2.BuildServiceProvider())
                    {
                        using (var context2 = sp2.GetService<Services.Models.DbContext>())
                        {
                            foreach (var customer in context2.Customers.Where(cust => cust.Name.EndsWith("Nursery")))
                            {
                                customer.ZipCode = 1;
                            }
                            context2.SaveChanges();
                        }
                    }

                    foreach (var customer in context.Customers.Where(cust => cust.Name.EndsWith("Nursery")))
                    {
                        //zip code should be set to 0 from the first run against these records
                        //if it has 1 as its value, then we have reloaded the data and that is incorrect behavior when change tracking is turned on
                        Assert.AreEqual(0, customer.ZipCode);
                    }
                    context.ChangeTracker.DetectChanges();
                    try
                    {
                        context.SaveChanges();
                        Assert.Fail("should have detected changes");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        //we changed these records inside context 2
                    }
                }
            }

        }

        [TestMethod]
        public void Contains()
        {
            var startupClass = new Startup(null, null);
            var startupServices = new ServiceCollection();
            startupClass.ConfigureServices(startupServices);
            using (var sp = startupServices.BuildServiceProvider())
            {
                using (var context = sp.GetService<Services.Models.DbContext>())
                {
                    var customers = context.Customers.Where(cust => cust.Name.Contains("Nursery")).ToList();
                    Assert.IsTrue(customers.Count > 0);
                    foreach (var customer in customers)
                    {
                        Assert.IsTrue(customer.Name.Contains("Nursery"));
                    }
                }
            }
        }

        [TestMethod]
        public void EFSparse()
        {
            Harmony.Core.FileIO.Queryable.PreparedQueryPlan.LocalSparse = true;
            var startupClass = new Startup(null, null);
            var startupServices = new ServiceCollection();
            startupClass.ConfigureServices(startupServices);
            using (var sp = startupServices.BuildServiceProvider())
            {
                using (var context = sp.GetService<Services.Models.DbContext>())
                {
                    var customers = context.Customers.Where(cust => cust.Name.Contains("Nursery")).Select(cust => new { Name = cust.Name, fred = cust.City }).ToList();
                    Assert.IsTrue(customers.Count > 0);
                    foreach (var customer in customers)
                    {
                        Assert.IsTrue(customer.Name.Contains("Nursery"));
                    }
                }
            }
        }

        [TestMethod]
        public void Projection1()
        {
            var startupClass = new Startup(null, null);
            var startupServices = new ServiceCollection();
            startupClass.ConfigureServices(startupServices);
            using (var sp = startupServices.BuildServiceProvider())
            {
                using (var context = sp.GetService<Services.Models.DbContext>())
                {
                    //referencing a navigation propery in the selector will cause it to be included
                    //as though we had called context.Orders.Include(salesOrderHeader => salesOrderHeader.REL_OrderItems)
                    var header1 = from salesOrderheader in context.Orders
                                  where salesOrderheader.OrderNumber < 100
                                  select new { SalesOrderHeader = salesOrderheader, Total = salesOrderheader.REL_OrderItems };

                    foreach (var header in header1)
                    {
                        Console.Write(header.SalesOrderHeader.CustomerNumber);
                    }
                }
            }
        }

        [TestMethod]
        public void IncludeWhere()
        {
            var startupClass = new Startup(null, null);
            var startupServices = new ServiceCollection();
            startupClass.ConfigureServices(startupServices);
            using (var sp = startupServices.BuildServiceProvider())
            {
                using (var context = sp.GetService<Services.Models.DbContext>())
                {
                    var customers = context.Customers.Where(customer => customer.CustomerNumber == 8).Include(customer => customer.REL_CustomerFavoriteItem).ToList();
                    Assert.AreEqual(customers.Count, 1);
                    Assert.AreEqual(customers.First().CustomerNumber, 8);
                    Assert.IsNotNull(customers.First().REL_CustomerFavoriteItem);
                }
            }
        }

        [TestMethod]
        public void IncludeFirstOrDefault()
        {
            var startupClass = new Startup(null, null);
            var startupServices = new ServiceCollection();
            startupClass.ConfigureServices(startupServices);
            using (var sp = startupServices.BuildServiceProvider())
            {
                using (var context = sp.GetService<Services.Models.DbContext>())
                {
                    var customer = context.Customers.Include(customer => customer.REL_CustomerFavoriteItem).FirstOrDefault(customer => customer.CustomerNumber == 8);
                    Assert.IsNotNull(customer);
                    Assert.AreEqual(customer.CustomerNumber, 8);
                    Assert.IsNotNull(customer.REL_CustomerFavoriteItem);
                }
            }
        }

    }
}
