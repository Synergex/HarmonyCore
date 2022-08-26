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
    internal class StructureSettings : IMultiItemSettingsBase
    {
        SolutionInfo _context;
        Dictionary<string, Dictionary<string, object>> _structureProperties;

        public string Name { get; } = "Structures";

        public List<ISingleItemSettings> Items { get; } = new List<ISingleItemSettings>();

        public bool CanAddItems => true;

        public StructureSettings(SolutionInfo context) 
        {
            _context = context;
            _structureProperties = new Dictionary<string, Dictionary<string, object>>(context.CodeGenSolution.GetExtendedStructureProperties(), StringComparer.OrdinalIgnoreCase);
            Items = context.CodeGenSolution.ExtendedStructures.Select(str => new SingleStructureSettings(context, str) as ISingleItemSettings).ToList();
            Name = "Structures";
        }

        public (ISingleItemSettings, PropertyItemSetting) GetInitialProperty()
        {
            var dummySingleSetting = new StructurePickerHelper(_context, new HashSet<string>(Items.Select(itm => itm.Name), StringComparer.OrdinalIgnoreCase)) as ISingleItemSettings;
            return (dummySingleSetting, dummySingleSetting.DisplayProperties.First());
        }

        public ISingleItemSettings AddItem(PropertyItemSetting initSetting)
        {
            var madeStructure = new StructureEx { Name = initSetting.Value as string };
            _context.CodeGenSolution.ExtendedStructures.Add(madeStructure);
            var result = new SingleStructureSettings(_context, madeStructure);
            Items.Add(result);
            return result;
        }

        class StructurePickerHelper : SingleItemSettingsBase, IContextWithFilter
        {
            HashSet<string> _disallowItems;
            public StructurePickerHelper(SolutionInfo context, HashSet<string> disallowItems) :base(context)
            {
                _disallowItems = disallowItems;
                Name = "Pick structure";
            }

            [Prompt("Name")]
            [StructNameOptions]
            public string StructureName { get; set; }

            public bool AllowItem(string item)
            {
                return !_disallowItems.Contains(item);
            }
        }

        class SingleStructureSettings : SingleItemSettingsBase
        {
            StructureEx _structure;
            RpsStructure _rpsStructure;

            public SingleStructureSettings(SolutionInfo context, string structureName) : base(context)
            {
                _structure = new StructureEx { Name = structureName };
                _rpsStructure = context.CodeGenSolution.RPS.GetStructure(structureName);
                Name = structureName;
                BaseInterface.LoadSameProperties(_structure);
                RelationsSpecs = new RelationSpecSettings(context, _structure.RelationsSpecs, _structure, _rpsStructure);
            }

            public SingleStructureSettings(SolutionInfo context) : base(context)
            {
                _structure = new StructureEx();
                RelationsSpecs = null;
            }

            public SingleStructureSettings(SolutionInfo context, StructureEx structure) : base(context)
            {
                _structure = structure;
                _rpsStructure = context.CodeGenSolution.RPS.GetStructure(structure.Name);
                Name = structure.Name;
                BaseInterface.LoadSameProperties(structure);
                RelationsSpecs = new RelationSpecSettings(context, structure.RelationsSpecs, _structure, _rpsStructure);
                ControllerAuthorization = new AuthOptionSettings(structure.ControllerAuthorization, (newVal) => structure.ControllerAuthorization = newVal);
                PostAuthorization = new AuthOptionSettings(structure.PostAuthorization, (newVal) => structure.PostAuthorization = newVal);
                PutAuthorization = new AuthOptionSettings(structure.PutAuthorization, (newVal) => structure.PutAuthorization = newVal);
                PatchAuthorization = new AuthOptionSettings(structure.PatchAuthorization, (newVal) => structure.PatchAuthorization = newVal);
                DeleteAuthorization = new AuthOptionSettings(structure.DeleteAuthorization, (newVal) => structure.DeleteAuthorization = newVal);
                GetAuthorization = new AuthOptionSettings(structure.GetAuthorization, (newVal) => structure.GetAuthorization = newVal);
            }

            public override void Save(SolutionInfo context)
            {
                BaseInterface.SaveSameProperties(_structure);

                ((IMultiItemSettingsBase)RelationsSpecs).Save(context);
                ControllerAuthorization.Save(context);
                PostAuthorization.Save(context);
                PutAuthorization.Save(context);
                PatchAuthorization.Save(context);
                DeleteAuthorization.Save(context);
                GetAuthorization.Save(context);
            }
            [IEnumerableExtractor("|")]
            public List<string> Aliases { get; set; }
            [DictionaryExtractor("->", "|")]
            public Dictionary<string, string> Files { get; set; }

            [IEnumerableExtractor("|")]
            [IEnumerableInjector(typeof(HashSet<string>), "|")]
            [GeneratorOptions]
            [AllowMultiSelection]
            public HashSet<string> EnabledGenerators { get; set; }

            [Prompt("Custom relation specs")]
            [ComplexObjectExtractor]
            public RelationSpecSettings RelationsSpecs { get; set; }

            public bool? EnableRelations { get; set; }
            
            public bool? EnableRelationValidation { get; set; }

            public bool? EnableGetAll { get; set; }

            public bool? EnableGetOne { get; set; }

            public bool? EnableAltGet { get; set; }

            public bool? EnablePut { get; set; }

            public bool? EnablePost { get; set; }

            public bool? EnablePatch { get; set; }

            public bool? EnableDelete { get; set; }

            [ComplexObjectExtractor]
            public AuthOptionSettings ControllerAuthorization { get; set; }

            [ComplexObjectExtractor]
            public AuthOptionSettings PostAuthorization { get; set; }

            [ComplexObjectExtractor]
            public AuthOptionSettings PutAuthorization { get; set; }

            [ComplexObjectExtractor]
            public AuthOptionSettings PatchAuthorization { get; set; }

            [ComplexObjectExtractor]
            public AuthOptionSettings DeleteAuthorization { get; set; }

            [ComplexObjectExtractor]
            public AuthOptionSettings GetAuthorization { get; set; }

            [Prompt("OData query options")]
            public string ODataQueryOptions { get; set; }

            [ComplexObjectExtractor]
            public Dictionary<string, Dictionary<string, object>> Fields { get; set; }
        }
    }
}
