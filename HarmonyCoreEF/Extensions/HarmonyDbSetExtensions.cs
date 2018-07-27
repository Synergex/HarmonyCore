using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;

namespace Harmony.Core.EF.Extensions
{
    public static class HarmonyDbSetExtensions
    {
        static ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> _compiledFindQueryLookup = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>();
        static ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> _compiledFirstOrDefaultLookup = new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();
        static ParsingConfig DefaultParseConfig = new ParsingConfig();
        public static T FindCompiled<K, T>(this DbSet<T> thisp, K keyValue) 
            where T : class
        {
            var context = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(DbContext)) as DbContext;
            var contextType = context.GetType();
            var entityType = context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType.FindPrimaryKey();
            var primaryKeyName = primaryKey.Properties.First().Name;
            var compiledQueryLookup = _compiledFindQueryLookup.GetOrAdd(contextType, (ty) => new ConcurrentDictionary<Type, object>());
            var compiledQuery = compiledQueryLookup.GetOrAdd(typeof(T), (ty) =>
            {
                var keyParameter = Expression.Parameter(typeof(K), "keyValue");
                var contextParameter = Expression.Parameter(typeof(DbContext), "context");
                var entityParameter = Expression.Parameter(typeof(T), "entity");
                Expression whereClause = Expression.Equal(
                        Expression.Property(entityParameter, primaryKeyName),
                        keyParameter);
                var querySet = Expression.Call(contextParameter, typeof(DbContext).GetMethod("Set").MakeGenericMethod(new Type[] { typeof(T) }));
                var whereLambda = Expression.Lambda<Func<T, bool>>(whereClause, entityParameter);
                var whereCall = Expression.Call(typeof(System.Linq.Queryable), "Where", new Type[] { typeof(T) }, querySet, whereLambda);
                var firstOrDefaultResult = Expression.Call(typeof(System.Linq.Queryable), "FirstOrDefault", new Type[] { typeof(T) }, whereCall);
                var resultLambda = Expression.Lambda<Func<DbContext, K, T>>(firstOrDefaultResult, contextParameter, keyParameter);
                return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda);
            }) as Func<DbContext, K, T>;

            //Precompile this func seperately
            //if (context.ChangeTracker.QueryTrackingBehavior != QueryTrackingBehavior.NoTracking)
            //{
            //    // Execute against the in-memory entities, which we get from ChangeTracker (but not filtering the state of the entities).
            //    var entries = context.ChangeTracker.Entries<T>().Select((EntityEntry e) => (T)e.Entity);
            //    T entity = compiledQuery(context, keyValue);

            //    // If found in memory then we're done.
            //    if (entity != null) { return entity; }
            //}

            // Otherwise execute the query against the database.
            return compiledQuery(context, keyValue);
        }

        public static T FirstOrDefault<T>(this DbSet<T> thisp, string expression, params object[] parameters)
            where T : class
        {
            if (parameters.Length > 5)
            {
                return thisp.FirstOrDefault(expression, parameters);
            }
            else
            {
                var context = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(DbContext)) as DbContext;
                var contextType = context.GetType();
                var compiledQueryLookup = _compiledFirstOrDefaultLookup.GetOrAdd(contextType, (ty) => new ConcurrentDictionary<string, object>());
                var compiledQuery = compiledQueryLookup.GetOrAdd(expression, (ex) =>
                {
                    var contextParameter = Expression.Parameter(typeof(DbContext), "context");
                    var linqParameters = new ParameterExpression[parameters.Length + 1];
                    var exprLinqParameters = new ParameterExpression[parameters.Length];
                    linqParameters[0] = contextParameter;
                    for (int i = 1; i < linqParameters.Length; i++)
                    {
                        exprLinqParameters[i - 1] = linqParameters[i] = Expression.Parameter(typeof(object));
                    }

                    var querySet = Expression.Call(contextParameter, typeof(DbContext).GetMethod("Set").MakeGenericMethod(new Type[] { typeof(T) }));
                    var whereLambda = DynamicExpressionParser.ParseLambda<T, bool>(DefaultParseConfig, false, expression, exprLinqParameters);
                    var firstOrDefaultResult = Expression.Call(typeof(System.Linq.Queryable), "FirstOrDefault", new Type[] { typeof(T) }, querySet, whereLambda);

                    switch (parameters.Length)
                    {
                        case 0:
                            var resultLambda = Expression.Lambda<Func<DbContext, T>>(firstOrDefaultResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda);
                        case 1:
                            var resultLambda1 = Expression.Lambda<Func<DbContext, object, T>>(firstOrDefaultResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda1);
                        case 2:
                            var resultLambda2 = Expression.Lambda<Func<DbContext, object, object, T>>(firstOrDefaultResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda2);
                        case 3:
                            var resultLambda3 = Expression.Lambda<Func<DbContext, object, object, object, T>>(firstOrDefaultResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda3);
                        case 4:
                            var resultLambda4 = Expression.Lambda<Func<DbContext, object, object, object, object, T>>(firstOrDefaultResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda4);
                        case 5:
                            var resultLambda5 = Expression.Lambda<Func<DbContext, object, object, object, object, object, T>>(firstOrDefaultResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda5);
                        default:
                            throw new NotImplementedException();
                    }
                });

                switch (parameters.Length)
                {
                    case 0:
                        return ((Func<DbContext, T>)compiledQuery)(context);
                    case 1:
                        return ((Func<DbContext, object, T>)compiledQuery)(context, parameters[0]);
                    case 2:
                        return ((Func<DbContext, object, object, T>)compiledQuery)(context, parameters[0], parameters[1]);
                    case 3:
                        return ((Func<DbContext, object, object, object, T>)compiledQuery)(context, parameters[0], parameters[1], parameters[2]);
                    case 4:
                        return ((Func<DbContext, object, object, object, object, T>)compiledQuery)(context, parameters[0], parameters[1], parameters[2], parameters[3]);
                    case 5:
                        return ((Func<DbContext, object, object, object, object, object, T>)compiledQuery)(context, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        //public static T FirstOrDefaultIncluding<T>(this DbSet<T> thisp, string expression, string[] include, params object[] parameters)
        //    where T : class
        //{

        //    var linqParameters = new ParameterExpression[parameters.Length];
        //    for (int i = 0; i < linqParameters.Length; i++)
        //    {
        //        linqParameters[i] = Expression.Parameter(parameters[i].GetType());
        //    }
        //    var context = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(DbContext)) as DbContext;
        //    var contextType = context.GetType();
        //    var keyParameter = Expression.Parameter(typeof(K), "keyValue");
        //    var contextParameter = Expression.Parameter(contextType, "context");
        //    var entityParameter = Expression.Parameter(typeof(T), "entity");
        //    var querySet = Expression.Call(contextParameter, typeof(DbContext).GetMethod("Set").MakeGenericMethod(new Type[] { typeof(T) }));
        //    var whereLambda = DynamicExpressionParser.ParseLambda<T>(DefaultParseConfig, false, expression, entityParameter);
        //    var firstOrDefaultResult = Expression.Call(typeof(System.Linq.Queryable), "FirstOrDefault", new Type[] { typeof(T) }, querySet, whereLambda);
        //    var resultLambda = Expression.Lambda(firstOrDefaultResult, contextParameter, keyParameter);
        //    return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda);
        //}

        //public static IEnumerable<T> Where<T>(this DbSet<T> thisp, string expression, params object[] parameters)
        //    where T : class
        //{
        //    var context = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(DbContext)) as DbContext;
        //    var contextType = context.GetType();
        //}

        //public static IEnumerable<T> WhereIncluding<T>(this DbSet<T> thisp, string expression, string[] include, params object[] parameters)
        //    where T : class
        //{
        //    var context = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(DbContext)) as DbContext;
        //    var contextType = context.GetType();
        //}

        public static T FindAlternate<T>(this DbSet<T> thisp, string keyName, object keyValue) where T : class
        {
            // Find DbContext, entity type, and primary key.
            var context = ((IInfrastructure<IServiceProvider>)thisp).GetService<DbContext>();
            var entityType = context.Model.FindEntityType(typeof(T));
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.Equal(
                    Expression.Property(entityParameter, keyName),
                    Expression.Constant(keyValue));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);

            // Execute against the in-memory entities, which we get from ChangeTracker (but not filtering the state of the entities).
            var entries = context.ChangeTracker.Entries<T>().Select((EntityEntry e) => (T)e.Entity);
            T entity = entries.AsQueryable().Where(lambdaExpression).FirstOrDefault(); // First is what triggers the query execution.

            // If found in memory then we're done.
            if (entity != null) { return entity; }

            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression).FirstOrDefault();
        }

        public static T FindAlternate<T>(this DbSet<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2) where T : class
        {
            // Find DbContext, entity type, and primary key.
            var context = ((IInfrastructure<IServiceProvider>)thisp).GetService<DbContext>();
            var entityType = context.Model.FindEntityType(typeof(T));
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.And(
                Expression.Equal(
                    Expression.Property(entityParameter, keyName),
                    Expression.Constant(keyValue)),
                Expression.Equal(
                    Expression.Property(entityParameter, keyName2),
                    Expression.Constant(keyValue2)));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);

            // Execute against the in-memory entities, which we get from ChangeTracker (but not filtering the state of the entities).
            var entries = context.ChangeTracker.Entries<T>().Select((EntityEntry e) => (T)e.Entity);
            T entity = entries.AsQueryable().Where(lambdaExpression).FirstOrDefault(); // First is what triggers the query execution.

            // If found in memory then we're done.
            if (entity != null) { return entity; }

            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression).FirstOrDefault();
        }

        public static T FindAlternate<T>(this DbSet<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2, string keyName3, object keyValue3) where T : class
        {
            // Find DbContext, entity type, and primary key.
            var context = ((IInfrastructure<IServiceProvider>)thisp).GetService<DbContext>();
            var entityType = context.Model.FindEntityType(typeof(T));
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause =
                Expression.And(
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName),
                        Expression.Constant(keyValue)),
                    Expression.And(
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName2),
                            Expression.Constant(keyValue2)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName3),
                            Expression.Constant(keyValue3))));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);

            // Execute against the in-memory entities, which we get from ChangeTracker (but not filtering the state of the entities).
            var entries = context.ChangeTracker.Entries<T>().Select((EntityEntry e) => (T)e.Entity);
            T entity = entries.AsQueryable().Where(lambdaExpression).FirstOrDefault(); // First is what triggers the query execution.

            // If found in memory then we're done.
            if (entity != null) { return entity; }

            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression).FirstOrDefault();
        }

        public static IQueryable<T> FindAlternates<T>(this DbSet<T> thisp, string keyName, object keyValue) where T : class
        {
            // Find DbContext, entity type, and primary key.
            var context = ((IInfrastructure<IServiceProvider>)thisp).GetService<DbContext>();
            var entityType = context.Model.FindEntityType(typeof(T));
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.Equal(
                    Expression.Property(entityParameter, keyName),
                    Expression.Constant(keyValue));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }

        public static IQueryable<T> FindAlternates<T>(this DbSet<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2) where T : class
        {
            // Find DbContext, entity type, and primary key.
            var context = ((IInfrastructure<IServiceProvider>)thisp).GetService<DbContext>();
            var entityType = context.Model.FindEntityType(typeof(T));
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.And(
                Expression.Equal(
                    Expression.Property(entityParameter, keyName),
                    Expression.Constant(keyValue)),
                Expression.Equal(
                    Expression.Property(entityParameter, keyName2),
                    Expression.Constant(keyValue2)));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }
    }
}
