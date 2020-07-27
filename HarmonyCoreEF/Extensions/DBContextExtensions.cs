using Harmony.Core.Context;
using Harmony.Core.EF.Storage;
using Harmony.Core.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harmony.Core.EF.Extensions
{
    public static class DBContextExtensions
    {
        public static int SaveChanges(this DbContext context, IPrimaryKeyFactory keyFactory)
        {
            var result = context.SaveChanges();
            keyFactory.Commit();
            return result;
        }
    }
}
