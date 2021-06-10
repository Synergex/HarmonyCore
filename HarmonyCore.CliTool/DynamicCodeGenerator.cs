using CodeGen.Engine;
using HarmonyCoreGenerator.Generator;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                .WithReferences(new Assembly[] { typeof(GeneratorBase).Assembly, typeof(CodeGenTask).Assembly, typeof(List<string>).Assembly })
                .WithImports("HarmonyCoreGenerator.Generator", "HarmonyCoreGenerator.Model", "System.Collections.Generic", "System", "CodeGen.Engine");

            foreach (var scriptFile in Directory.EnumerateFiles(path, "*.csx"))
            {
                var scriptContents = File.ReadAllText(scriptFile);
                var script = CSharpScript.Create<GeneratorBase>(scriptContents, scriptOptions);
                var result = await script.RunAsync();
                if (result.Exception == null && result.ReturnValue != null)
                {
                    resultGenerators.Add(Path.GetFileNameWithoutExtension(scriptFile), result.ReturnValue);
                }
            }
            return resultGenerators;
        }
    }
}
