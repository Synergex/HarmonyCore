using CodeGen.Engine;
using HarmonyCore.CliTool.TUI.Models;
using HarmonyCoreGenerator.Model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Helpers
{
    internal class DynamicSettingsLoader
    {
        public static async Task<Dictionary<string, ISettingsBase>> LoadDynamicSettings(SolutionInfo context, string path)
        {
            var resultSettings = new Dictionary<string, ISettingsBase>();

            if (String.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return resultSettings;

            var scriptOptions = ScriptOptions.Default
                .WithEmitDebugInformation(true)
                .WithReferences(new Assembly[] { typeof(ISettingsBase).Assembly, typeof(CodeGenTask).Assembly, typeof(List<string>).Assembly, typeof(ObservableCollection<>).Assembly })
                .WithImports("HarmonyCoreCodeGenGUI.Models", "HarmonyCoreGenerator.Generator", "HarmonyCoreGenerator.Model", "System.Collections.Generic",
                    "System", "System.IO", "System.Linq", "CodeGen.Engine", "System.Collections.ObjectModel");

            foreach (var scriptFile in Directory.EnumerateFiles(path, "*.csx"))
            {
                using var scriptContents = File.Open(scriptFile, FileMode.Open);
                var script = CSharpScript.Create<ISettingsBase>(scriptContents, scriptOptions.WithFilePath(scriptFile));
                var result = await script.RunAsync(globals: new { Context = context });
                if (result.Exception == null && result.ReturnValue != null)
                {
                    resultSettings.Add(Path.GetFileNameWithoutExtension(scriptFile), result.ReturnValue);
                }
            }
            return resultSettings;
        }
    }
}
