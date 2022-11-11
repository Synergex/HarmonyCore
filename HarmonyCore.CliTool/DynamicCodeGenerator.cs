using CodeGen.Engine;
using HarmonyCoreGenerator.Generator;
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
using HarmonyCoreGenerator.Model;

namespace HarmonyCore.CliTool
{
    class DynamicCodeGenerator
    {
        public static async Task<Dictionary<string, GeneratorBase>> LoadDynamicGenerators(string path)
        {
            var resultGenerators = new Dictionary<string, GeneratorBase>();

            if (String.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return resultGenerators;

            var scriptOptions = ScriptOptions.Default
                .WithEmitDebugInformation(true)
                .WithReferences(new Assembly[] { typeof(GeneratorBase).Assembly, typeof(CodeGenTask).Assembly, typeof(List<string>).Assembly, typeof(ObservableCollection<>).Assembly })
                .WithImports("HarmonyCoreGenerator.Generator", "HarmonyCoreGenerator.Model", "System.Collections.Generic", 
                    "System", "System.IO", "System.Linq", "CodeGen.Engine", "System.Collections.ObjectModel");

            foreach (var scriptFile in Directory.EnumerateFiles(path, "*.csx"))
            {
                using var scriptContents = File.Open(scriptFile, FileMode.Open);
                var script = CSharpScript.Create<GeneratorBase>(scriptContents, scriptOptions.WithFilePath(scriptFile));
                var result = await script.RunAsync();
                if (result.Exception == null && result.ReturnValue != null)
                {
                    resultGenerators.Add(Path.GetFileNameWithoutExtension(scriptFile), result.ReturnValue);
                }
            }
            return resultGenerators;
        }

        public static async Task<Func<string, string, Action<string>, Task<Solution>>> LoadDynamicConfig(string path)
        {
            Func<string, string, Action<string>, Task<Solution>> defaultGenerator = (jsonFile, solutionFile, logger) => Task.FromResult(Solution.LoadSolution(jsonFile, solutionFile));

            if (String.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return defaultGenerator;

            var scriptOptions = ScriptOptions.Default
                .WithEmitDebugInformation(true)
                .WithReferences(new Assembly[] { typeof(DynamicCodeGenerator).Assembly, typeof(GeneratorBase).Assembly, typeof(CodeGenTask).Assembly, typeof(List<string>).Assembly, typeof(ObservableCollection<>).Assembly })
                .WithImports("HarmonyCoreGenerator.Generator", "HarmonyCoreGenerator.Model", "HarmonyCore.CliTool.TUI.Helpers", "System.Collections.Generic",
                    "System", "System.Threading.Tasks", "System.IO", "System.Linq", "CodeGen.Engine", "System.Collections.ObjectModel");

            var scriptResults = new List<Func<string, string, Action<string>, Task<Solution>>>();
            foreach (var scriptFile in Directory.EnumerateFiles(path, "LoadSolution.csx"))
            {
                using var scriptContents = File.Open(scriptFile, FileMode.Open);
                var script = CSharpScript.Create<Func<string, string, Action<string>, Task<Solution>>>(scriptContents, scriptOptions.WithFilePath(scriptFile));
                var result = await script.RunAsync();
                if (result.Exception == null && result.ReturnValue != null)
                {
                    scriptResults.Add(result.ReturnValue);
                }
            }

            return scriptResults.FirstOrDefault() ?? defaultGenerator;
        }
    }
}
