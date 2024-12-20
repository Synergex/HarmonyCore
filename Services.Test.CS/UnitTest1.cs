using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            BaseServiceProvider.Cleanup();
            UnitTestEnvironment.AssemblyCleanup();

        }

        [TestMethod]
        public void DualContextUpdate()
        {
            using (var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    foreach (var customer in context.Customers.Where(cust => cust.Name.EndsWith("Nursery")))
                    {
                        customer.ZipCode = 0;
                    }


                    using (var sp2 = BaseServiceProvider.Services)
                    {
                        using (var context2 = sp2.ServiceProvider.GetService<Services.Models.DbContext>())
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
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
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
        public void DateTimeRange()
        {
            using (var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    var old = new DateTime(1980, 1, 1);
                    var orders = context.Orders.Where(ord => ord.DateOrdered != DateTime.MinValue && ord.DateOrdered <= now).Take(5).Select(ord => ord.DateOrdered).ToList();
                    var orders2 = context.Orders.Where(ord => ord.DateOrdered != DateTime.MinValue && ord.DateOrdered >= old).Take(5).Select(ord => ord.DateOrdered).ToList();
                    Assert.IsTrue(Enumerable.SequenceEqual(orders, orders2));
                }
            }
        }

        [TestMethod]
        public void InCollection()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var ids = new int[] { 1, 2, 4, 9 };
                    var customers = context.Customers.Where(cust => ids.Contains(cust.CustomerNumber)).ToList();
                    Assert.IsTrue(customers.Count > 0);
                }
            }
        }

        [TestMethod]
        public void InSingleCollection()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var ids = new int[] { 1 };
                    var customers = context.Customers.Where(cust => ids.Contains(cust.CustomerNumber)).ToList();
                    Assert.IsTrue(customers.Count > 0);
                }
            }
        }

        [TestMethod]
        public void InEmptyCollection()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var ids = new int[] { };
                    var customers = context.Customers.Where(cust => ids.Contains(cust.CustomerNumber)).ToList();
                    Assert.IsTrue(customers.Count == 0);
                }
            }
        }

        [TestMethod]
        public void InCollection2()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var states = new string[] { "CA", "FL", "WA" };
                    var customers = context.Customers.Where(cust => states.Contains(cust.State)).ToList();
                    Assert.IsTrue(customers.Count > 0);
                }
            }
        }

        [TestMethod]
        public void EFSparse()
        {
            //force sparse select even though we arent using xfServer here
            Harmony.Core.FileIO.Queryable.PreparedQueryPlan.LocalSparse = true;
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
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
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
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
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var customers = context.Customers.Where(customer => customer.CustomerNumber == 8).Include(customer => customer.REL_CustomerFavoriteItem).ToList();
                    Assert.AreEqual(customers.Count, 1);
                    Assert.AreEqual(customers.First().CustomerNumber, 8);
                    Assert.IsNotNull(customers.First().REL_CustomerFavoriteItem);
                }
            }
        }

        [TestMethod]
        public void IncludeDeep()
        {
            var tasks = new List<Task>();
            for (int taskN = 0; taskN < 20; taskN++)
            {
                var task = Task.Run(() =>
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        using (var sp = BaseServiceProvider.Services)
                        {
                            using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                            {
                                var customersQuerable = context.Customers.AsNoTracking().Include(customer => customer.REL_CustomerFavoriteItem).ThenInclude(itm => itm.REL_OrderItems).ThenInclude(orderItem => orderItem.REL_Order);
                                var customers = customersQuerable.ToList();

                                Assert.AreEqual(customers.Count, 38);
                            }
                        }
                    }
                });
                tasks.Add(task);
            }


            var allResult = Task.WhenAll(tasks.ToArray());
            while(allResult.IsCompleted == false)
            {
                GC.Collect(1, GCCollectionMode.Forced, false, false);
                GC.WaitForPendingFinalizers();
            }

        }

        [TestMethod]
        public void IncludeFirstOrDefault()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var customer = context.Customers.Include(customer => customer.REL_CustomerFavoriteItem).FirstOrDefault(customer => customer.CustomerNumber == 8);
                    Assert.IsNotNull(customer);
                    Assert.AreEqual(customer.CustomerNumber, 8);
                    Assert.IsNotNull(customer.REL_CustomerFavoriteItem);
                }
            }
        }


        [TestMethod]
        public void UpdateFavoriteItem()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var customer = context.Customers.Include(customer => customer.REL_CustomerFavoriteItem).FirstOrDefault(customer => customer.CustomerNumber == 8);

                    customer.REL_CustomerFavoriteItem.CommonName = "bunnybear";
                    var changeCount = context.SaveChanges();
                    Assert.AreEqual(1, changeCount);
                }
            }
        }


        [TestMethod]
        public void UpdateFavoriteItem2()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var customer = context.Customers.FirstOrDefault(customer => customer.CustomerNumber == 8);
                    customer.FavoriteItem = context.Items.Where(item => item.FlowerColor.ToLower() == "blue").First().ItemNumber;

                    var changeCount = context.SaveChanges();
                    Assert.AreEqual(1, changeCount);
                }
            }
        }


        [TestMethod]
        public void MultiToLower()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    
                    var second = context.Items.Where(item => item.FlowerColor.ToLower() == "white" && item.LatinName.Contains("ainvillea")).First();
                    var first = context.Items.Where(item => item.FlowerColor.ToLower() == "white" && item.LatinName.ToLower() == "bougainvillea").First();
                    Assert.AreEqual(first.LatinName, second.LatinName);
                    Assert.AreEqual(second.LatinName, "Bougainvillea");
                }
            }
        }

        [TestMethod]
        public void MultiToLower2()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {

                    var second = context.Items.Where(item => item.FlowerColor.ToLower() == "white" && item.LatinName.ToLower().Contains("bougai")).First();
                    var first = context.Items.Where(item => item.FlowerColor.ToLower() == "white" && item.LatinName.ToLower() == "bougainvillea").First();
                    Assert.AreEqual(first.LatinName, second.LatinName);
                    Assert.AreEqual(second.LatinName, "Bougainvillea");
                }
            }
        }

        [TestMethod]
        public void ImplicitUnion()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {

                    var second = context.Items.Include(item => item.REL_Vendor).Where(item => item.FlowerColor.ToLower() == "white" || item.REL_Vendor.City == "Boston").ToList();
                    var thing = context.Items.Include(item => item.REL_Vendor).Where(item => item.FlowerColor.ToLower() == "white").ToList();
                    var first = context.Items.Include(item => item.REL_Vendor).Where(item => item.REL_Vendor.City == "Boston").ToList();

                    Assert.IsTrue(Enumerable.SequenceEqual(second, first.Union(thing)));
                }
            }
        }

        [TestMethod]
        public void ImplicitUnion2()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {

                    var second = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape == "bush" && (item.FlowerColor.ToLower() == "white" || item.REL_Vendor.City == "Boston")).ToList();
                    var thing = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape == "bush" && (item.FlowerColor.ToLower() == "white")).ToList();
                    var first = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape == "bush" && (item.REL_Vendor.City == "Boston")).ToList();

                    Assert.IsTrue(Enumerable.SequenceEqual(second, first.Union(thing)));
                }
            }
        }

        [TestMethod]
        public void ImplicitUnion3()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {

                    var second = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape.ToLower() == "bush" && (item.FlowerColor.ToLower() == "white" || item.REL_Vendor.City.ToLower() == "Boston")).ToList();
                    var thing = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape.ToLower() == "bush" && (item.FlowerColor.ToLower() == "white")).ToList();
                    var first = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape.ToLower() == "bush" && (item.REL_Vendor.City.ToLower() == "Boston")).ToList();

                    Assert.IsTrue(Enumerable.SequenceEqual(second, first.Union(thing)));
                }
            }
        }

        [TestMethod]
        public void ImplicitUnion4()
        {
            using(var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {

                    //var second = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape.ToLower().Contains("ush") && (item.FlowerColor.ToLower().Contains("ite") || item.REL_Vendor.City.ToLower().Contains("ston"))).ToList();
                    //var thing = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape.ToLower().Contains("ush") && (item.FlowerColor.ToLower().Contains("ite"))).ToList();
                    var first = context.Items.Include(item => item.REL_Vendor).Where(item => item.Shape.ToLower().Contains("ush") && (item.REL_Vendor.City.ToLower().Contains("ston"))).ToList();

                    //Assert.IsTrue(Enumerable.SequenceEqual(second, first.Union(thing)));
                }
            }
        }

        [TestMethod]
        public void OrderBy()
        {
            using (var sp = BaseServiceProvider.Services)
            {
                using (var context = sp.ServiceProvider.GetService<Services.Models.DbContext>())
                {
                    var customers = context.Customers;
                    var orders = context.Orders;

                    // join customers and orders and do an orderby on orders collection
                    var customerOrdersJoin = customers
                            .Include(
                                customer => customer.REL_CustomerOrders
                                .OrderByDescending(ordrs => ordrs.OrderNumber)
                                )
                            .ToList();

                    foreach (var customer in customerOrdersJoin)
                    {
                        var ordernum = -1;
                        foreach (var order in customer.REL_CustomerOrders)
                        {
                            if (ordernum != -1)
                                Assert.IsTrue(ordernum > order.OrderNumber);
                            ordernum = order.OrderNumber;
                        }
                    }

                    // join customers and items and order the result set on a joined field
                    var customerItemsJoin = customers
                         .Include(
                             customer => customer.REL_CustomerFavoriteItem
                             )
                         .OrderByDescending(customer => customer.REL_CustomerFavoriteItem.ItemNumber)
                         .ToList();

                    var itemNum = -1;
                    foreach (var customer in customerItemsJoin)
                    {
                        if (itemNum != -1)
                            Assert.IsTrue(itemNum >= customer.REL_CustomerFavoriteItem.ItemNumber);
                        itemNum = customer.REL_CustomerFavoriteItem.ItemNumber;
                    }

                    // join customers, orders, and items and order an orders collection and result set on ItemNumber
                    var customerOrdersItemsJoin = customers
                            .Include(
                                customer => customer.REL_CustomerOrders
                                .OrderByDescending(ordrs => ordrs.OrderNumber)
                                )
                            .Include(customer => customer.REL_CustomerFavoriteItem)
                            .OrderByDescending(customer => customer.REL_CustomerFavoriteItem.ItemNumber)
                            .ToList();

                    itemNum = -1;
                    foreach (var customer in customerOrdersItemsJoin)
                    {
                        if (itemNum != -1)
                            Assert.IsTrue(itemNum >= customer.REL_CustomerFavoriteItem.ItemNumber);
                        itemNum = customer.REL_CustomerFavoriteItem.ItemNumber;

                        var ordernum = -1;
                        foreach (var order in customer.REL_CustomerOrders)
                        {
                            if (ordernum != -1)
                                Assert.IsTrue(ordernum > order.OrderNumber);
                            ordernum = order.OrderNumber;
                        }
                    }
                }
            }
        }
    }
}
