using Harmony.Core.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Test.CS
{
    [TestClass]
    public class ObjectPoolTests
    {
        class MyTestContext : IPooledContextBase2, IContextBase
        {
            [ThreadStatic]
            static Random _random;
            internal static int _instanceCount;
            bool _inited = false;
            public bool IsHealthy
            {
                get
                {
                    if (_random == null)
                        _random = new Random();
                    return _random.Next(0, 2) == 0;
                }
            }

            public ContextIsolationLevel IsolationLevel => ContextIsolationLevel.FreeThreaded;

            public MyTestContext()
            {
                Interlocked.Increment(ref _instanceCount);
            }

            public void Destroy()
            {
                _inited = false;
                Interlocked.Decrement(ref _instanceCount);

            }

            public Task EnsureReady()
            {
                Assert.IsTrue(_inited);
                return Task.FromResult(true);
            }

            public void InitServices(IServiceProvider sp)
            {
                _inited = true;
            }

            public Task Recycle()
            {
                _inited = false;
                return Task.FromResult(true);
            }
        }

        [TestMethod]
        public void LoadTest()
        {
            var tasks = new List<Task>();
            BlockingPoolContextFactory<MyTestContext> contextFactory = new BlockingPoolContextFactory<MyTestContext>((sp) => new MyTestContext(), 6, 4, TimeSpan.FromSeconds(30), true);
            for(int i = 0; i < 8; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    for (int ii = 0; ii < 1000; ii++)
                    {
                        var madeContext = contextFactory.MakeContext(null);
                        await madeContext.EnsureReady();
                        contextFactory.ReturnContext(madeContext);
                        await Task.Yield();
                    }
                }));
            }
            Task.WhenAll(tasks).Wait();
            Assert.IsTrue(MyTestContext._instanceCount <= 4);
            contextFactory.TrimPool(0).Wait();
            Assert.AreEqual(MyTestContext._instanceCount, 0);
        }
    }
}
