// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Harmony.Core.EF.Storage.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Harmony.Core.Context;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;

namespace Harmony.Core.EF.Query.Internal
{
	/// <summary>
	///     This API supports the Entity Framework Core infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	public class HarmonyQueryContext : QueryContext
	{
		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public HarmonyQueryContext(
			QueryContextDependencies dependencies,
			Func<IQueryBuffer> queryBufferFactory,
			IDataObjectProvider store)
			: base(dependencies, () => new QueryBufferWrapper(dependencies))
		{
			Store = store;

		}

		private class QueryBufferWrapper : QueryBuffer
		{
			public QueryBufferWrapper(QueryContextDependencies dependencies) : base(dependencies)
			{
			}

			public override object GetPropertyValue(object entity, IProperty property)
			{
				if (property.FieldInfo.Name.StartsWith("_KEY_"))
					return "";
				else
					return base.GetPropertyValue(entity, property);
			}
		}

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IDataObjectProvider Store { get; }
        private Dictionary<Type, IEntityType> MetadataLookup = new Dictionary<Type, IEntityType>();
        public Dictionary<Type, List<DataObjectBase>> ResultLookup;

        public IEntityType GetEntityType(Type ty)
        {
            IEntityType result;
            if (!MetadataLookup.TryGetValue(ty, out result))
            {
                result = Context.Model.FindEntityType(ty.FullName);
                MetadataLookup.Add(ty, result);
            }
            return result;
        }

		
    }
}
