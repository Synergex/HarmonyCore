using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Test.CS
{
    class BaseServiceProvider
    {
        static BaseServiceProvider()
        {
            var startupClass = new Startup(null, null);
            var startupServices = new ServiceCollection();
            startupClass.ConfigureServices(startupServices);
            provider = startupServices.BuildServiceProvider();
        }
        private static IServiceProvider provider;
        public static IServiceScope Services
        {
            get
            {
                return provider.CreateScope();
            }
        }

        public static void Cleanup()
        {
            (provider as IDisposable)?.Dispose();
            provider = null;
        }
    }
}
