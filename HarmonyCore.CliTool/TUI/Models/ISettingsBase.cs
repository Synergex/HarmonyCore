using CodeGen.MethodCatalogAPI;
using CodeGen.RepositoryAPI;
using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    public interface ISettingsBase
    {
        string Name { get; }
        bool IsEnabled(Solution solution)
        {
            return true;
        }
        void Save(SolutionInfo context);
    }

    public interface IContextWithStructure
    {
        StructureEx StructureExContext { get; }
        RpsStructure StructureContext { get; }
    }
    public interface IContextWithInterface
    {
        InterfaceEx InterfaceExContext { get; }
        SmcInterface InterfaceContext { get; }
    }

    public interface IContextWithFilter
    {
        bool AllowItem(string item);
    }
}
