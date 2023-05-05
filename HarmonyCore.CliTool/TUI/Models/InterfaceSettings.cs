using CodeGen.MethodCatalogAPI;
using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    public class InterfaceSettings : IMultiItemSettingsBase, IRemovableItem 
    {
        SolutionInfo _context;
        public List<ISingleItemSettings> Items { get; } = new List<ISingleItemSettings>();
        public string Name { get; } = "Interfaces";
        public bool CanAddItems => _context.CodeGenSolution.TraditionalBridge?.Smc != null;
        public bool CanRemoveItems { get; set; } = false; 
        public InterfaceSettings(SolutionInfo context)
        {
            _context = context;
            foreach(var iface in _context.CodeGenSolution.ExtendedInterfaces)
            {
                Items.Add(MakeSingleInterface(iface));
            }
        }

        public (ISingleItemSettings, IPropertyItemSetting) GetInitialProperty()
        {
            var dummySingleSetting = new InterfacePickerHelper(_context, new HashSet<string>(Items.Select(itm => itm.Name), StringComparer.OrdinalIgnoreCase)) as ISingleItemSettings;
            return (dummySingleSetting, dummySingleSetting.DisplayProperties.First());
        }

        public ISingleItemSettings AddItem(IPropertyItemSetting initSetting)
        {
            var madeInterface = new InterfaceEx
            {
                Name = initSetting.Value as string, 
                GenerateInterface = true, 
                GenerateSignalRHub = _context.CodeGenSolution.SmcSignalRHubs ?? false, 
                GenerateWebAPIController = _context.CodeGenSolution.TraditionalBridge.EnableXFServerPlusMigration ?? false
            };
            _context.CodeGenSolution.ExtendedInterfaces.Add(madeInterface);
            var result = MakeSingleInterface(madeInterface);
            Items.Add(result);
            CanRemoveItems = true;
            return result;
        }

        public void RemoveItem(ISingleItemSettings itemToRemove)
        {
            Items.Remove(itemToRemove);
            var itemFromEtendedInterfacesList = _context.CodeGenSolution.ExtendedInterfaces.FirstOrDefault(item => item.Name == itemToRemove.Name);
            _context.CodeGenSolution.ExtendedInterfaces.Remove(itemFromEtendedInterfacesList);
            CanRemoveItems = Items.Count < 1 ? false : true;
        }

        private SingleInterfaceSetting MakeSingleInterface(InterfaceEx madeInterface)
        {
            return new SingleInterfaceSetting(_context, madeInterface,
                            _context.CodeGenSolution.TraditionalBridge.Smc.Interfaces.First(iface => string.Compare(iface.Name, madeInterface.Name, true) == 0));
        }

        class InterfacePickerHelper : SingleItemSettingsBase, IContextWithFilter
        {
            HashSet<string> _disallowItems;
            public InterfacePickerHelper(SolutionInfo context, HashSet<string> disallowItems) : base(context)
            {
                _disallowItems = disallowItems;
                Name = "Pick interface";
            }

            [Prompt("Name")]
            [InterfaceNameOptions]
            public string InterfaceName { get; set; }

            public bool AllowItem(string item)
            {
                return !_disallowItems.Contains(item);
            }
        }

        public class SingleInterfaceSetting : SingleItemSettingsBase
        {
            InterfaceEx _interfaceEx;
            SmcInterface _smcInterface;
            public SingleInterfaceSetting(SolutionInfo context, InterfaceEx interfaceEx, SmcInterface smcInterface) : base(context)
            {
                _interfaceEx = interfaceEx;
                _smcInterface = smcInterface;
                BaseInterface.LoadSameProperties(interfaceEx);
                Name = interfaceEx.Name;
                Authorization = new AuthOptionSettings(_interfaceEx.Authorization, (newVal) => _interfaceEx.Authorization = newVal);
            }

            public override void Save(SolutionInfo context)
            {
                BaseInterface.SaveSameProperties(_interfaceEx);
                Authorization.Save(context);
            }

            [Prompt("Name")] 
            [DisallowEdits]
            public string InterfaceName { get; set; }
            [ComplexObjectExtractor]
            public AuthOptionSettings Authorization { get; set; }

            [Prompt("Enable OData controller")]
            [NullableBoolInjector]
            [NullableBoolExtractor]
            [NullableBoolOptionsExtractor]
            public bool? GenerateODataController { get; set; }
            [Prompt("Enable WebAPI controller")]
            [NullableBoolInjector]
            [NullableBoolExtractor]
            [NullableBoolOptionsExtractor]
            public bool? GenerateWebAPIController { get; set; }
            [Prompt("Enable SignalR")]
            [NullableBoolInjector]
            [NullableBoolExtractor]
            [NullableBoolOptionsExtractor]
            public bool? GenerateSignalRHub { get; set; }
            [Prompt("Enable basic")]
            [NullableBoolInjector]
            [NullableBoolExtractor]
            [NullableBoolOptionsExtractor]
            public bool? GenerateInterface { get; set; }
        }
    }
}
