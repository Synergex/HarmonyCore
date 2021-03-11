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
    }
}
