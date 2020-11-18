using CodeGen.Engine;
using CodeGen.MethodCatalogAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HarmonyCore.CliTool.Commands
{
    class RegenCommand
    {
        SolutionInfo _solutionInfo;

        public RegenCommand(SolutionInfo solutionInfo)
        {
            _solutionInfo = solutionInfo;
        }

        public int Run(RegenOptions opts)
        {
            // Generate TraditionalBridge
            string traditionalBridgeNamespace = _solutionInfo.CodeGenSolution.TraditionalBridgeNamespace;
            string traditionalBridgeInterface = _solutionInfo.CodeGenSolution.TraditionalBridge.Smc.Interfaces.First().Name;
            string traditionalBridgeSMCPath = _solutionInfo.CodeGenSolution.TraditionalBridge.XFServerSMCPath;
            HashSet<string> traditionalBridgeStructures = new HashSet<string>(_solutionInfo.CodeGenSolution.TraditionalBridge.Smc.Structures.Select(k => k.Name));

            // Generate Models
            {
                Console.WriteLine("Generating TraditionalModel and TraditionalMetaData");

                CodeGenTask codeGenTask = new CodeGenTask
                {
                    Namespace = $"{traditionalBridgeNamespace}.Models",
                    ReplaceFiles = true,
                    Structures = new ObservableCollection<string>(_solutionInfo.CodeGenSolution.TraditionalBridge.Smc.Structures.Select(k => k.Name)),
                    Templates = new ObservableCollection<string> { "TraditionalModel", "TraditionalMetaData" },
                    TemplateFolder = @"Templates\TraditionalBridge"
                };
                CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                {
                    ListGeneratedFiles = true,
                    OutputFolder = $@"{traditionalBridgeNamespace}\Models"
                };
                codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                codeGenTaskSet.Tasks.Add(codeGenTask);

                if (!Directory.Exists(codeGenTaskSet.OutputFolder))
                    Directory.CreateDirectory(codeGenTaskSet.OutputFolder);

                new CodeGenerator(codeGenTaskSet).GenerateCode();
                if (!codeGenTaskSet.Complete)
                    return 1;
            }

            // Generate InterfaceDispatcher and InterfaceMethodDispatchers
            {
                Console.WriteLine($"{Environment.NewLine}Generating InterfaceDispatcher and InterfaceMethodDispatchers");

                CodeGenTask codeGenTask = new CodeGenTask
                {
                    Interface = traditionalBridgeInterface,
                    Namespace = $"{traditionalBridgeNamespace}.Dispatchers",
                    ReplaceFiles = true,
                    Templates = new ObservableCollection<string> { "InterfaceDispatcher", "InterfaceMethodDispatchers" },
                    TemplateFolder = @"Templates\TraditionalBridge",
                    UserTokens = new ObservableCollection<UserToken> { new UserToken("MODELS_NAMESPACE", $"{traditionalBridgeNamespace}.Models"), new UserToken("DTOS_NAMESPACE", $"{traditionalBridgeNamespace}.Models") }
                };
                CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                {
                    ListGeneratedFiles = true,
                    MethodCatalog = new Smc(traditionalBridgeSMCPath),
                    OutputFolder = $@"{traditionalBridgeNamespace}\Dispatchers"
                };
                codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                codeGenTaskSet.Tasks.Add(codeGenTask);

                if (!Directory.Exists(codeGenTaskSet.OutputFolder))
                    Directory.CreateDirectory(codeGenTaskSet.OutputFolder);

                new CodeGenerator(codeGenTaskSet).GenerateCode();
                if (!codeGenTaskSet.Complete)
                    return 1;
            }

            // Generate MultiInterfaceServiceModels
            {
                Console.WriteLine($"{Environment.NewLine}Generating MultiInterfaceServiceModels");

                CodeGenTask codeGenTask = new CodeGenTask
                {
                    Interface = traditionalBridgeInterface,
                    Namespace = $"{traditionalBridgeNamespace}.Models",
                    ReplaceFiles = true,
                    Structures = new ObservableCollection<string>(traditionalBridgeStructures),
                    Templates = new ObservableCollection<string> { "MultiInterfaceServiceModels" },
                    TemplateFolder = @"Templates\TraditionalBridge",
                    UserTokens = new ObservableCollection<UserToken> { new UserToken("MODELS_NAMESPACE", $"{traditionalBridgeNamespace}.Models") }
                };
                CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                {
                    ListGeneratedFiles = true,
                    MethodCatalog = new Smc(traditionalBridgeSMCPath),
                    OutputFolder = @$"{traditionalBridgeNamespace}\Models"
                };
                codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                codeGenTaskSet.Tasks.Add(codeGenTask);

                new CodeGenerator(codeGenTaskSet).GenerateCode();
            }

            // Generate InterfaceService
            {
                Console.WriteLine($"{Environment.NewLine}Generating InterfaceService");

                CodeGenTask codeGenTask = new CodeGenTask
                {
                    Interface = traditionalBridgeInterface,
                    MultipleStructures = true,
                    Namespace = $"{traditionalBridgeNamespace}.Client",
                    ReplaceFiles = true,
                    Structures = new ObservableCollection<string>(traditionalBridgeStructures),
                    Templates = new ObservableCollection<string> { "InterfaceService" },
                    TemplateFolder = @"Templates\TraditionalBridge",
                    UserTokens = new ObservableCollection<UserToken> { new UserToken("MODELS_NAMESPACE", $"{traditionalBridgeNamespace}.Models"), new UserToken("DTOS_NAMESPACE", $"{traditionalBridgeNamespace}.{traditionalBridgeInterface}") }
                };
                CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                {
                    ListGeneratedFiles = true,
                    MethodCatalog = new Smc(traditionalBridgeSMCPath),
                    OutputFolder = @$"{traditionalBridgeNamespace}\Client"
                };
                codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                codeGenTaskSet.Tasks.Add(codeGenTask);

                if (!Directory.Exists(codeGenTaskSet.OutputFolder))
                    Directory.CreateDirectory(codeGenTaskSet.OutputFolder);

                new CodeGenerator(codeGenTaskSet).GenerateCode();
            }

            // Generate InterfaceDispatcherData
            {
                Console.WriteLine($"{Environment.NewLine}Generating InterfaceDispatcherData");

                CodeGenTask codeGenTask = new CodeGenTask
                {
                    Interface = traditionalBridgeInterface,
                    MultipleStructures = true,
                    Namespace = $"{traditionalBridgeNamespace}.Dispatchers",
                    ReplaceFiles = true,
                    Structures = new ObservableCollection<string>(traditionalBridgeStructures),
                    Templates = new ObservableCollection<string> { "InterfaceDispatcherData" },
                    TemplateFolder = @"Templates\TraditionalBridge",
                    UserTokens = new ObservableCollection<UserToken> { new UserToken("SMC_INTERFACE", traditionalBridgeNamespace), new UserToken("DTOS_NAMESPACE", $"{traditionalBridgeNamespace}.Dispatchers") }
                };
                CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                {
                    ListGeneratedFiles = true,
                    MethodCatalog = new Smc(traditionalBridgeSMCPath),
                    OutputFolder = @$"{traditionalBridgeNamespace}\Dispatchers"
                };
                codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                codeGenTaskSet.Tasks.Add(codeGenTask);

                new CodeGenerator(codeGenTaskSet).GenerateCode();
            }

            // Generate rest of the solution
            var result = _solutionInfo.CodeGenSolution.GenerateSolution((tsk, message) => Console.WriteLine("{0} : {1}", string.Join(',', tsk.Templates), message), CancellationToken.None);

            foreach (var error in result.ValidationErrors)
            {
                Console.WriteLine(error);
            }
            return 0;
        }
    }
}
