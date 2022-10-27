using CodeGen.RepositoryAPI;
using HarmonyCoreExtensions;
using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static HarmonyCoreExtensions.Helpers;

namespace HarmonyCore.CliTool.TUI.Models
{
    public class RelationSpecSettings : PropertyItemSetting, IMultiItemSettingsBase
    {
        public List<ISingleItemSettings> Items { get; } = new List<ISingleItemSettings>();

        public string Name => "Relation Specification";

        public bool CanAddItems => _structureContext.Relations.Count > 0;

        List<CustomRelationSpec> _relations;
        RpsStructure _structureContext;
        StructureEx _structureExContext;
        SolutionInfo _solutionContext;
        public RelationSpecSettings(SolutionInfo solutionContext, List<CustomRelationSpec> relations, StructureEx structureExContext, RpsStructure structureContext)
        {
            _structureContext = structureContext;
            _structureExContext = structureExContext;
            _relations = relations;
            _solutionContext = solutionContext;
            //base relations come from _structureContext.Relations
            //overlay the custom relations on top
            //will need to handle resplitting this out during save

            var baseItems = _structureContext.Relations.Select(MakeRelationCurry);

            Items.AddRange(GenerateRelationSpecs(baseItems.Concat(relations)).Values);
        }

        Dictionary<string, RelationSpecItem> GenerateRelationSpecs(IEnumerable<CustomRelationSpec> specs)
        {
            var overlayedRelations = new Dictionary<string, RelationSpecItem>();
            foreach (var rel in specs)
            {
                var relationSpec = new RelationSpecItem(_solutionContext, rel, _structureExContext, _structureContext);
                overlayedRelations[relationSpec.ToString()] = relationSpec;
            }
            return overlayedRelations;
        }

        Func<RpsRelation, CustomRelationSpec> MakeRelationCurry => (rel) => MakeRelation(_solutionContext.CodeGenSolution, rel);

        void ISettingsBase.Save(SolutionInfo context)
        {
            //iterate over the Items collection, compare it to a fresh generation and only save the items that are different
            var generatedItems = GenerateRelationSpecs(_structureContext.Relations.Select(MakeRelationCurry));

            foreach (var item in Items.OfType<RelationSpecItem>())
            {
                //TODO: this doesnt deal with deleting a custom relation spec item
                if (generatedItems.TryGetValue(item.ToString(), out var generatedItem))
                {
                    if (item.HasChanges(generatedItem))
                    {
                        item.Save(context);
                        item.MaybeAdd(_relations);
                    }
                }
                else
                {
                    item.Save(context);
                }
            }
        }

        public IEnumerable<string> AddableItems()
        {
            //get the list of defined relations for the current structure context
            return _structureContext.Relations.Select(rel => rel.Name);
        }

        public ISingleItemSettings AddItem(PropertyItemSetting initSetting)
        {
            var foundRelation = _structureContext.Relations.FirstOrDefault((rpsRel) => string.Compare(rpsRel.Name, initSetting.Value as string, true) == 0);
            if (foundRelation != null)
            {
                var madeItem = new RelationSpecItem(_solutionContext, MakeRelation(_solutionContext.CodeGenSolution, foundRelation), _structureExContext, _structureContext);
                Items.Add(madeItem);
                return madeItem;
            }
            else
                return null;
        }

        static CustomRelationSpec MakeRelation(Solution context, RpsRelation relation)
        {
            var fromStructure = context.RPS.GetStructure(relation.FromStructure);
            var toStructure = context.RPS.GetStructure(relation.ToStructure);
            var fromKey = fromStructure?.Keys?.FirstOrDefault(rpsKey => rpsKey.Name == relation.FromKey);
            var toKey = toStructure?.Keys?.FirstOrDefault(rpsKey => rpsKey.Name == relation.ToKey);

            return HarmonyCoreExtensions.Helpers.GetRelationSpec(context.TemplatesFolder, new ConcurrentDictionary<object, object>(),
                context.RPS.Structures, fromStructure, toStructure, fromKey, toKey, false);
        }

        public (ISingleItemSettings, PropertyItemSetting) GetInitialProperty()
        {
            throw new NotImplementedException();
        }

        public class RelationSpecItem : SingleItemSettingsBase, IContextWithStructure
        {
            RpsStructure _structureContext;
            StructureEx _structureExContext;
            SolutionInfo _solutionContext;
            CustomRelationSpec _relationSpec;
            public RelationSpecItem(SolutionInfo context, CustomRelationSpec relationSpec, StructureEx structureExContext, RpsStructure structureContext) : base(context)
            {
                _structureContext = structureContext;
                _structureExContext = structureExContext;
                _relationSpec = relationSpec;
                BaseInterface.LoadSameProperties(relationSpec);
                Name = relationSpec.RelationName;
            }

            public bool HasChanges(RelationSpecItem compareTo)
            {
                if (compareTo.ToStructure == ToStructure &&
                    compareTo.FromKey == FromKey &&
                    compareTo.ToKey == ToKey &&
                    compareTo.RelationName == RelationName &&
                    compareTo.RequiresMatch == RequiresMatch &&
                    compareTo.ValidationMode == ValidationMode &&
                    compareTo.BackRelation == BackRelation &&
                    compareTo.RelationType == RelationType &&
                    compareTo.CustomValidatorName == CustomValidatorName)
                    return false;
                else
                    return true;
            }

            public void MaybeAdd(List<CustomRelationSpec> collection)
            {
                if (!collection.Contains(_relationSpec))
                {
                    collection.Add(_relationSpec);
                }
            }

            public override void Save(SolutionInfo context)
            {
                BaseInterface.SaveSameProperties(_relationSpec);
            }

            [DisallowEdits]
            public string FromStructure { get; set; }

            [StructKeyOptions]
            public string FromKey { get; set; }

            [DisallowEdits]
            [StructNameOptions]
            public string ToStructure { get; set; }
            [StructKeyOptions]
            public string ToKey { get; set; }

            public string RelationName { get; set; }

            public bool RequiresMatch { get; set; }

            public RelationValidationMode ValidationMode { get; set; }

            public string BackRelation { get; set; }

            public string RelationType { get; set; }

            public string CustomValidatorName { get; set; }

            StructureEx IContextWithStructure.StructureExContext => _structureExContext;

            RpsStructure IContextWithStructure.StructureContext => _structureContext;

            public override string ToString()
            {
                return string.Format("{0}-{1}-{2}-{3}", FromStructure, ToStructure, FromKey, ToKey);
            }
        }
        public class BackRelationSpecItem : SingleItemSettingsBase, IContextWithStructure
        {
            IContextWithStructure _baseContext;
            CustomRelationSpec _relation;
            public BackRelationSpecItem(SolutionInfo solution, CustomRelationSpec relation, IContextWithStructure baseContext) : base(solution)
            {
                _baseContext = baseContext;
                _relation = relation;
                //TODO fill FromStructure, FromKey, ToStructure, ToKey from _relation.BackRelation
            }

            [StructNameOptions]
            [Prompt("Backlink structure")]
            public string FromStructure { get; set; }

            [StructKeyOptions]
            [Prompt("Backlink key")]
            public string FromKey { get; set; }

            [StructKeyOptions]
            [Prompt("Target key (in the current structure)")]
            public string ToKey { get; set; }

            public StructureEx StructureExContext => _baseContext.StructureExContext;

            public RpsStructure StructureContext => _baseContext.StructureContext;

            public override string ToString()
            {
                return string.Format("{0}-{1}-{2}", FromStructure,  FromKey, ToKey);
            }
        }
    }
}
