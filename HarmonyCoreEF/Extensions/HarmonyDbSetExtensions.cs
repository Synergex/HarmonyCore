using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
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
        static ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>> _queryableLookup = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>>();
        static ConcurrentDictionary<Guid, ConcurrentDictionary<Type, object>> _compiledFindQueryLookup = new ConcurrentDictionary<Guid, ConcurrentDictionary<Type, object>>();
        static ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>> _compiledFirstOrDefaultLookup = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>>();
        static ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>> _compiledWhereLookup = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>>();
        static ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>> _compiledWhereIncludeLookup = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, object>>();
        static ParsingConfig DefaultParseConfig = new ParsingConfig();
        public static T FindCompiled<K, T>(this DbSet<T> thisp, K keyValue)
            where T : class
        {
            var currentContext = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            var context = currentContext.Context;
            var contextType = context.Model.ModelId;
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
                var querySet = SetExpr<T>(contextParameter);
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

        public static void AddOrUpdate<T, K>(this DbSet<T> thisp, K primaryKey, T obj)
             where T : class
        {
            var found = FindCompiled<K, T>(thisp, primaryKey);
            if (found != null)
            {
                thisp.Update(obj);
            }
            else
            {
                thisp.Add(obj);
            }
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
                var currentContext = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
                var context = currentContext.Context;
                var contextType = context.Model.ModelId;
                var compiledQueryLookup = _compiledFirstOrDefaultLookup.GetOrAdd(contextType, (ty) => new ConcurrentDictionary<string, object>());
                var compiledQuery = compiledQueryLookup.GetOrAdd(expression, (ex) =>
                {
                    var contextParameter = Expression.Parameter(typeof(DbContext), "context");
                    var linqParameters = new ParameterExpression[parameters.Length + 1];
                    var exprLinqParameters = new Expression[parameters.Length];
                    linqParameters[0] = contextParameter;
                    for (int i = 1; i < linqParameters.Length; i++)
                    {
                        exprLinqParameters[i - 1] = Expression.Convert(linqParameters[i] = Expression.Parameter(typeof(object)), parameters[i - 1].GetType());
                    }

                    var querySet = SetExpr<T>(contextParameter);
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

        //public static IQueryable<T> First<T>(IQueryable<T> query)
        //{

        //}

        //public static SingleResult<T> SingleResult<T>(this IQueryable<T> query)
        //{

        //}

        public static IQueryable<T> FindQuery<T>(this DbSet<T> thisp, params object[] parameters)
            where T : class
        {
            // Find DbContext, entity type, and primary key.
            var currentContext = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            var context = currentContext.Context;
			return FindQuery<T>((IQueryable<T>)thisp, context, parameters);
        }

        public static IList<string> GetPKFieldNames<T>(IEntityType entityType)
        {
            if (typeof(DataObjectBase).IsAssignableFrom(typeof(T)))
            {
                var metadata = DataObjectMetadataBase.LookupType(typeof(T));
                var fields = metadata.GetKeyFields(0, KeyType.ISAMGenerated | KeyType.KeyFactorySupplied | KeyType.UserSupplied);
                return fields;
            }
            else
            {
                var primaryKey = entityType.FindPrimaryKey();
                return primaryKey.Properties.Select(prop => prop.Name).ToList();
            }
            
        }

		public static IQueryable<T> FindQuery<T>(this IQueryable<T> thisp, DbContext context, params object[] parameters)
			where T : class
		{
			var entityParameter = Expression.Parameter(typeof(T), "entity");
			Expression whereClause = null;
            var primaryKeyFields = GetPKFieldNames<T>(context.Model.FindEntityType(typeof(T)));
            for (int i = 0; i < primaryKeyFields.Count && i < parameters.Length; i++)
			{
				var property = primaryKeyFields[i];
				var newWhereClause = Expression.Equal(
					Expression.Property(entityParameter, property),
					Expression.Constant(parameters[i]));
				if (whereClause != null)
				{
					whereClause = Expression.AndAlso(whereClause, newWhereClause);
				}
				else
				{
					whereClause = newWhereClause;
				}
			}

			var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
			return thisp.Where(lambdaExpression);
		}

		public static T FirstOrDefaultIncluding<T>(this DbSet<T> thisp, string including, string expression, params object[] parameters)
            where T : class
        {
            if (parameters.Length > 5)
            {
                return thisp.FirstOrDefault(expression, parameters);
            }
            else
            {
                var currentContext = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
                var context = currentContext.Context;
                var contextType = context.Model.ModelId;
                var compiledQueryLookup = _compiledFirstOrDefaultLookup.GetOrAdd(contextType, (ty) => new ConcurrentDictionary<string, object>());
                var compiledQuery = compiledQueryLookup.GetOrAdd(expression + ";including;" + including, (ex) =>
                {
                    var contextParameter = Expression.Parameter(typeof(DbContext), "context");
                    var linqParameters = new ParameterExpression[parameters.Length + 1];
                    var exprLinqParameters = new Expression[parameters.Length];
                    linqParameters[0] = contextParameter;
                    for (int i = 1; i < linqParameters.Length; i++)
                    {
                        exprLinqParameters[i - 1] = Expression.Convert(linqParameters[i] = Expression.Parameter(typeof(object)), parameters[i - 1].GetType());
                    }

                    var querySet = SetExpr<T>(contextParameter);
                    var whereLambda = DynamicExpressionParser.ParseLambda<T, bool>(DefaultParseConfig, false, expression, exprLinqParameters);
                    var whereResult = Expression.Call(typeof(System.Linq.Queryable), "Where", new Type[] { typeof(T) }, querySet, whereLambda);
                    var includeResult = whereResult;
                    foreach (var includeBit in including.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var propertyType = typeof(T).GetProperty(includeBit).PropertyType;
                        includeResult = Expression.Call(typeof(EntityFrameworkQueryableExtensions), "Include", new Type[] { typeof(T), propertyType }, includeResult, MakePropertySelector<T>(includeBit, propertyType));
                    }

                    var firstOrDefaultResult = Expression.Call(typeof(System.Linq.Queryable), "FirstOrDefault", new Type[] { typeof(T) }, includeResult);

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

        public static IEnumerable<T> Where<T>(this DbSet<T> thisp, string expression, params object[] parameters)
            where T : class
        {
            if (parameters.Length > 5)
            {
                return thisp.Where(expression, parameters);
            }
            else
            {
                var currentContext = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
                var context = currentContext.Context;
                var contextType = context.Model.ModelId;
                var compiledQueryLookup = _compiledWhereLookup.GetOrAdd(contextType, (ty) => new ConcurrentDictionary<string, object>());
                var compiledQuery = compiledQueryLookup.GetOrAdd(expression, (ex) =>
                {
                    var contextParameter = Expression.Parameter(typeof(DbContext), "context");
                    var linqParameters = new ParameterExpression[parameters.Length + 1];
                    var exprLinqParameters = new Expression[parameters.Length];
                    linqParameters[0] = contextParameter;
                    for (int i = 1; i < linqParameters.Length; i++)
                    {
                        exprLinqParameters[i - 1] = Expression.Convert(linqParameters[i] = Expression.Parameter(typeof(object)), parameters[i - 1].GetType());
                    }

                    var querySet = SetExpr<T>(contextParameter);
                    var whereLambda = DynamicExpressionParser.ParseLambda<T, bool>(DefaultParseConfig, false, expression, exprLinqParameters);
                    var whereResult = Expression.Call(typeof(System.Linq.Queryable), "Where", new Type[] { typeof(T) }, querySet, whereLambda);

                    switch (parameters.Length)
                    {
                        case 0:
                            var resultLambda = Expression.Lambda<Func<DbContext, IEnumerable<T>>>(whereResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda);
                        case 1:
                            var resultLambda1 = Expression.Lambda<Func<DbContext, object, IEnumerable<T>>>(whereResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda1);
                        case 2:
                            var resultLambda2 = Expression.Lambda<Func<DbContext, object, object, IEnumerable<T>>>(whereResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda2);
                        case 3:
                            var resultLambda3 = Expression.Lambda<Func<DbContext, object, object, object, IEnumerable<T>>>(whereResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda3);
                        case 4:
                            var resultLambda4 = Expression.Lambda<Func<DbContext, object, object, object, object, IEnumerable<T>>>(whereResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda4);
                        case 5:
                            var resultLambda5 = Expression.Lambda<Func<DbContext, object, object, object, object, object, IEnumerable<T>>>(whereResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda5);
                        default:
                            throw new NotImplementedException();
                    }
                });

                switch (parameters.Length)
                {
                    case 0:
                        return ((Func<DbContext, IEnumerable<T>>)compiledQuery)(context);
                    case 1:
                        return ((Func<DbContext, object, IEnumerable<T>>)compiledQuery)(context, parameters[0]);
                    case 2:
                        return ((Func<DbContext, object, object, IEnumerable<T>>)compiledQuery)(context, parameters[0], parameters[1]);
                    case 3:
                        return ((Func<DbContext, object, object, object, IEnumerable<T>>)compiledQuery)(context, parameters[0], parameters[1], parameters[2]);
                    case 4:
                        return ((Func<DbContext, object, object, object, object, IEnumerable<T>>)compiledQuery)(context, parameters[0], parameters[1], parameters[2], parameters[3]);
                    case 5:
                        return ((Func<DbContext, object, object, object, object, object, IEnumerable<T>>)compiledQuery)(context, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static LambdaExpression MakePropertySelector<T>(string propertyName, Type propertyType)
        {
            var instance = Expression.Parameter(typeof(T), "instance");
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), propertyType);
            return Expression.Lambda(delegateType, Expression.Property(instance, propertyName), instance);
        }

        public static DbSet<TEntity> Set<TEntity>(DbContext context)
            where TEntity : class
            => (DbSet<TEntity>)((IDbSetCache)context).GetOrAddSet(context.GetDependencies().SetSource, typeof(TEntity));

        private static MethodCallExpression SetExpr<T>(Expression context)
        {
            return Expression.Call(null, typeof(HarmonyDbSetExtensions).GetMethod("Set").MakeGenericMethod(new Type[] { typeof(T) }), context);
        }

        public static IEnumerable<T> WhereIncluding<T>(this DbSet<T> thisp, string including, string expression, params object[] parameters)
            where T : class
        {
            var currentContext = ((IInfrastructure<IServiceProvider>)thisp).Instance.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            var context = currentContext.Context;
            var contextType = context.Model.ModelId;
            if (parameters.Length > 5)
            {
                return thisp.Where(expression, parameters);
            }
            else
            {
                var compiledQueryLookup = _compiledWhereIncludeLookup.GetOrAdd(contextType, (ty) => new ConcurrentDictionary<string, object>());
                var compiledQuery = compiledQueryLookup.GetOrAdd(expression + ";including;" + including, (ex) =>
                {
                    var contextParameter = Expression.Parameter(typeof(DbContext), "context");
                    var linqParameters = new ParameterExpression[parameters.Length + 1];
                    var exprLinqParameters = new Expression[parameters.Length];
                    linqParameters[0] = contextParameter;
                    for (int i = 1; i < linqParameters.Length; i++)
                    {
                        exprLinqParameters[i - 1] = Expression.Convert(linqParameters[i] = Expression.Parameter(typeof(object)), parameters[i - 1].GetType());
                    }

                    var querySet = SetExpr<T>(contextParameter);
                    var whereLambda = DynamicExpressionParser.ParseLambda<T, bool>(DefaultParseConfig, false, expression, exprLinqParameters);
                    var whereResult = Expression.Call(typeof(System.Linq.Queryable), "Where", new Type[] { typeof(T) }, querySet, whereLambda);
                    var includeResult = whereResult;
                    foreach (var includeBit in including.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var propertyType = typeof(T).GetProperty(includeBit).PropertyType;
                        includeResult = Expression.Call(typeof(EntityFrameworkQueryableExtensions), "Include", new Type[] { typeof(T), propertyType }, includeResult, MakePropertySelector<T>(includeBit, propertyType));
                    }

                    switch (parameters.Length)
                    {
                        case 0:
                            var resultLambda = Expression.Lambda<Func<DbContext, IEnumerable<T>>>(includeResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda);
                        case 1:
                            var resultLambda1 = Expression.Lambda<Func<DbContext, object, IEnumerable<T>>>(includeResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda1);
                        case 2:
                            var resultLambda2 = Expression.Lambda<Func<DbContext, object, object, IEnumerable<T>>>(includeResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda2);
                        case 3:
                            var resultLambda3 = Expression.Lambda<Func<DbContext, object, object, object, IEnumerable<T>>>(includeResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda3);
                        case 4:
                            var resultLambda4 = Expression.Lambda<Func<DbContext, object, object, object, object, IEnumerable<T>>>(includeResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda4);
                        case 5:
                            var resultLambda5 = Expression.Lambda<Func<DbContext, object, object, object, object, object, IEnumerable<T>>>(includeResult, linqParameters);
                            return Microsoft.EntityFrameworkCore.EF.CompileQuery(resultLambda5);
                        default:
                            throw new NotImplementedException();
                    }
                });

                switch (parameters.Length)
                {
                    case 0:
                        return ((Func<DbContext, IEnumerable<T>>)compiledQuery)(context);
                    case 1:
                        return ((Func<DbContext, object, IEnumerable<T>>)compiledQuery)(context, parameters[0]);
                    case 2:
                        return ((Func<DbContext, object, object, IEnumerable<T>>)compiledQuery)(context, parameters[0], parameters[1]);
                    case 3:
                        return ((Func<DbContext, object, object, object, IEnumerable<T>>)compiledQuery)(context, parameters[0], parameters[1], parameters[2]);
                    case 4:
                        return ((Func<DbContext, object, object, object, object, IEnumerable<T>>)compiledQuery)(context, parameters[0], parameters[1], parameters[2], parameters[3]);
                    case 5:
                        return ((Func<DbContext, object, object, object, object, object, IEnumerable<T>>)compiledQuery)(context, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static IQueryable<T> FindAlternate<T>(this IQueryable<T> thisp, string keyName, object keyValue) where T : class
        {
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.Equal(
                    Expression.Property(entityParameter, keyName),
                    Expression.Constant(keyValue));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }

        public static IQueryable<T> FindAlternate<T>(this IQueryable<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2) where T : class
        {
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.AndAlso(
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

        public static IQueryable<T> FindAlternate<T>(this IQueryable<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2, string keyName3, object keyValue3) where T : class
        {
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.AndAlso(
                Expression.AndAlso(
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName),
                        Expression.Constant(keyValue)),
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName2),
                        Expression.Constant(keyValue2))),
                Expression.Equal(
                        Expression.Property(entityParameter, keyName3),
                        Expression.Constant(keyValue3)));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }

        public static IQueryable<T> FindAlternate<T>(this IQueryable<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2, string keyName3, object keyValue3, string keyName4, object keyValue4) where T : class
        {
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.AndAlso(
                Expression.AndAlso(
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName),
                        Expression.Constant(keyValue)),
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName2),
                        Expression.Constant(keyValue2))),
                Expression.AndAlso(
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName3),
                        Expression.Constant(keyValue3)),
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName4),
                        Expression.Constant(keyValue4))));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }

        public static IQueryable<T> FindAlternate<T>(this IQueryable<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2, string keyName3, object keyValue3, string keyName4, object keyValue4, string keyName5, object keyValue5) where T : class
        {
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.AndAlso(
                Expression.AndAlso(
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName),
                        Expression.Constant(keyValue)),
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName2),
                        Expression.Constant(keyValue2))),
                Expression.AndAlso(
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName3),
                        Expression.Constant(keyValue3)),
                    Expression.AndAlso(
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName4),
                            Expression.Constant(keyValue4)),
                        Expression.Equal(Expression.Property(entityParameter, keyName5),
                            Expression.Constant(keyValue5)))));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }

        public static IQueryable<T> FindAlternate<T>(this IQueryable<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2, string keyName3, object keyValue3, string keyName4, object keyValue4, string keyName5, object keyValue5, string keyName6, object keyValue6) where T : class
        {
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.AndAlso(
                Expression.AndAlso(
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName),
                        Expression.Constant(keyValue)),
                    Expression.Equal(
                        Expression.Property(entityParameter, keyName2),
                        Expression.Constant(keyValue2))),
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName3),
                            Expression.Constant(keyValue3)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName4),
                            Expression.Constant(keyValue4))),
                    Expression.AndAlso(Expression.Equal(
                            Expression.Property(entityParameter, keyName5),
                            Expression.Constant(keyValue5)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName6),
                            Expression.Constant(keyValue6)))));
            

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }

        public static IQueryable<T> FindAlternate<T>(this IQueryable<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2, string keyName3, object keyValue3, string keyName4, object keyValue4, 
            string keyName5, object keyValue5, string keyName6, object keyValue6, string keyName7, object keyValue7) where T : class
        {
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.AndAlso(
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName),
                            Expression.Constant(keyValue)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName2),
                            Expression.Constant(keyValue2))),
                    Expression.AndAlso(Expression.Equal(
                            Expression.Property(entityParameter, keyName3),
                            Expression.Constant(keyValue3)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName4),
                            Expression.Constant(keyValue4)))),
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName5),
                            Expression.Constant(keyValue5)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName6),
                            Expression.Constant(keyValue6))),
                    Expression.Equal(
                            Expression.Property(entityParameter, keyName7),
                            Expression.Constant(keyValue7))));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }

        public static IQueryable<T> FindAlternate<T>(this IQueryable<T> thisp, string keyName, object keyValue, string keyName2, object keyValue2, string keyName3, object keyValue3, string keyName4, object keyValue4,
            string keyName5, object keyValue5, string keyName6, object keyValue6, string keyName7, object keyValue7, string keyName8, object keyValue8) where T : class
        {
            // Build the lambda expression for the query: (TEntity entity) => AND( entity.keyProperty[i] == keyValues[i])
            var entityParameter = Expression.Parameter(typeof(T), "entity");
            Expression whereClause = Expression.AndAlso(
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName),
                            Expression.Constant(keyValue)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName2),
                            Expression.Constant(keyValue2))),
                    Expression.AndAlso(Expression.Equal(
                            Expression.Property(entityParameter, keyName3),
                            Expression.Constant(keyValue3)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName4),
                            Expression.Constant(keyValue4)))),
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName5),
                            Expression.Constant(keyValue5)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName6),
                            Expression.Constant(keyValue6))),
                    Expression.AndAlso(Expression.Equal(
                            Expression.Property(entityParameter, keyName7),
                            Expression.Constant(keyValue7)),
                        Expression.Equal(
                            Expression.Property(entityParameter, keyName8),
                            Expression.Constant(keyValue8)))));

            var lambdaExpression = (Expression<Func<T, bool>>)Expression.Lambda(whereClause, entityParameter);
            // Otherwise execute the query against the database.
            return thisp.Where(lambdaExpression);
        }


        //public static IQueryable<T> WhereIncluding<T>(this DbSet<T> thisp, HarmonyFilteredInclude including, string expression, params object[] parameters)
        //    where T : class
        //{
        //}
    }
}
