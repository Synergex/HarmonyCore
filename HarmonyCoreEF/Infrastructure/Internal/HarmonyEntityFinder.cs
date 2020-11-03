using Harmony.Core.EF.Extensions;
using Harmony.Core.EF.Extensions.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Harmony.Core.EF.Infrastructure.Internal
{
    internal class HarmonyEntityFinder<T> : EntityFinder<T> where T : class
    {
        private IStateManager _stateManager;
        private IEntityType _entityType;
        public HarmonyEntityFinder(
            [NotNull] IStateManager stateManager,
            [NotNull] IDbSetSource setSource,
            [NotNull] IDbSetCache setCache,
            [NotNull] IEntityType entityType) : base(stateManager, setSource, setCache, entityType)
        {
            _stateManager = stateManager;
            _entityType = entityType;
        }


        private T FindTracked(object[] keyValues, out IReadOnlyList<IProperty> keyProperties)
        {
            var key = _entityType.FindPrimaryKey();
            keyProperties = HarmonyDbSetExtensions.GetPKFieldNames<T>(_entityType).Select(name => _entityType.FindProperty(name)).ToList();

            if (keyProperties.Count != keyValues.Length)
            {
                //we allow partial matches so we dont need to throw here
            }

            for (var i = 0; i < keyValues.Length; i++)
            {
                var valueType = keyValues[i].GetType();
                var propertyType = keyProperties[i].ClrType;
                if (valueType != propertyType.UnwrapNullableType())
                {
                    throw new ArgumentException(
                        string.Format("key part {0}:{1} was mismatched with {2}:{3}",
                            i, typeof(T).ShortDisplayName(), valueType.ShortDisplayName(), propertyType.ShortDisplayName()));
                }
            }

            return _stateManager.TryGetEntry(key, keyValues)?.Entity as T;
        }

        //public override T Find(object[] keyValues)
        //{
        //    return base.Find(new object[] { string.Join('|', keyValues) });
        //}
    }

    internal class HarmonyEntityFinderSource : IEntityFinderSource
    {
        public HarmonyEntityFinderSource()
        {
        }

        public IEntityFinder Create(IStateManager stateManager, IDbSetSource setSource, IDbSetCache setCache, IEntityType type)
        {
            return Activator.CreateInstance(typeof(HarmonyEntityFinder<>).MakeGenericType(type.ClrType), stateManager, setSource, setCache, type) as IEntityFinder;
        }
    }

}
