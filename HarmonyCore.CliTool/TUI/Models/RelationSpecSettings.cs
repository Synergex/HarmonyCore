using CodeGen.RepositoryAPI;
using HarmonyCoreExtensions;
using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Items.AddRange(relations.Select(rel => new RelationSpecItem(solutionContext, rel, _structureExContext, _structureContext)));
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
                var madeItem = new RelationSpecItem(_solutionContext, MakeRelation(foundRelation), _structureExContext, _structureContext);
                Items.Add(madeItem);
                return madeItem;
            }
            else
                return null;
        }

        static CustomRelationSpec MakeRelation(RpsRelation relation)
        {
            return new CustomRelationSpec
            {
                FromStructure = relation.FromStructure,
                ToStructure = relation.ToStructure,
                FromKey = relation.FromKey,
                ToKey = relation.ToKey,
                RelationName = relation.Name
            };
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
        }
    }
}
