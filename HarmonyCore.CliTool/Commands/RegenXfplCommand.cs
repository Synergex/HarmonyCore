using CodeGen.Engine;
using CodeGen.MethodCatalogAPI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace HarmonyCore.CliTool.Commands
{
    class RegenXfplCommand
    {
        public int Run(RegenXFPLOptions opts)
        {
            int errorCode;

            // Generate models
            {
                Console.WriteLine("Generating TraditionalModel and TraditionalMetaData");

                CodeGenTask codeGenTask = new CodeGenTask
                {
                    Namespace = $"{opts.Namespace}.Models",
                    ReplaceFiles = true,
                    Structures = new ObservableCollection<string>(opts.Structures),
                    Templates = new ObservableCollection<string> { "TraditionalModel", "TraditionalMetaData" },
                    TemplateFolder = @"Templates\TraditionalBridge"
                };
                CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                {
                    ListGeneratedFiles = true,
                    OutputFolder = $@"{opts.Project}\Models"
                };
                codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                codeGenTaskSet.Tasks.Add(codeGenTask);

                if (!Directory.Exists(codeGenTaskSet.OutputFolder))
                    Directory.CreateDirectory(codeGenTaskSet.OutputFolder);

                new CodeGenerator(codeGenTaskSet).GenerateCode();
                errorCode = codeGenTaskSet.Complete ? 0 : 1;
            }

            if (errorCode == 0)
            {
                // Generate rest of the files
                Directory.GetFiles(opts.XMLDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList().ForEach(item =>
                {
                    string interfaceName = default;

                    // Read the xml to figure out what the interface name is
                    using (XmlReader xmlReader = XmlReader.Create(item))
                    {
                        while (xmlReader.ReadToFollowing("interface"))
                        {
                            //Extract the value of the Name attribute
                            if (!xmlReader.GetAttribute("name").Equals("xfTest"))
                            {
                                interfaceName = xmlReader.GetAttribute("name");
                                break;
                            }
                        }
                    }

                    // Wasn't able to find the interface name
                    if (interfaceName == null)
                    {
                        errorCode = 1;
                        Console.WriteLine($"Could not find interface name in {Path.GetFileName(item)}");
                        return;
                    }

                    // Generate InterfaceDispatcher
                    {
                        Console.WriteLine($"{Environment.NewLine}Generating InterfaceDispatcher");

                        CodeGenTask codeGenTask = new CodeGenTask
                        {
                            Interface = interfaceName,
                            Namespace = $"{opts.Namespace}.Dispatchers",
                            ReplaceFiles = true,
                            Templates = new ObservableCollection<string> { "InterfaceDispatcher" },
                            TemplateFolder = @"Templates\TraditionalBridge",
                            UserTokens = new ObservableCollection<UserToken> { new UserToken("MODELS_NAMESPACE", $"{opts.Namespace}.Models"), new UserToken("DTOS_NAMESPACE", $"{opts.Namespace}.Models") }
                        };
                        CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                        {
                            ListGeneratedFiles = true,
                            MethodCatalog = new Smc(item),
                            OutputFolder = $@"{opts.Project}\Dispatchers"
                        };
                        codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                        codeGenTaskSet.Tasks.Add(codeGenTask);

                        if (!Directory.Exists(codeGenTaskSet.OutputFolder))
                            Directory.CreateDirectory(codeGenTaskSet.OutputFolder);

                        new CodeGenerator(codeGenTaskSet).GenerateCode();
                        errorCode = codeGenTaskSet.Complete ? 0 : 1;
                    }

                    if (errorCode != 0)
                        return;

                    // Generate InterfaceMethodDispatchers
                    {
                        Console.WriteLine($"{Environment.NewLine}Generating InterfaceMethodDispatchers");

                        CodeGenTask codeGenTask = new CodeGenTask
                        {
                            Interface = interfaceName,
                            Namespace = $"{opts.Namespace}.Dispatchers",
                            ReplaceFiles = true,
                            Templates = new ObservableCollection<string> { "InterfaceMethodDispatchers" },
                            TemplateFolder = @"Templates\TraditionalBridge",
                            UserTokens = new ObservableCollection<UserToken> { new UserToken("MODELS_NAMESPACE", $"{opts.Namespace}.Models"), new UserToken("DTOS_NAMESPACE", $"{opts.Namespace}.Models") }
                        };
                        CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                        {
                            ListGeneratedFiles = true,
                            MethodCatalog = new Smc(item),
                            OutputFolder = $@"{opts.Project}\Dispatchers"
                        };
                        codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                        codeGenTaskSet.Tasks.Add(codeGenTask);

                        if (!Directory.Exists(codeGenTaskSet.OutputFolder))
                            Directory.CreateDirectory(codeGenTaskSet.OutputFolder);

                        new CodeGenerator(codeGenTaskSet).GenerateCode();
                        errorCode = codeGenTaskSet.Complete ? 0 : 1;
                    }

                    if (errorCode != 0)
                        return;

                    // The below can error out and it is okay

                    // Generate MultiInterfaceServiceModels
                    {
                        Console.WriteLine($"{Environment.NewLine}Generating MultiInterfaceServiceModels");

                        string testName = Path.GetFileNameWithoutExtension(item);
                        CodeGenTask codeGenTask = new CodeGenTask
                        {
                            Interface = interfaceName,
                            Namespace = $"{ opts.Namespace}.Models",
                            ReplaceFiles = true,
                            Templates = new ObservableCollection<string> { "MultiInterfaceServiceModels" },
                            TemplateFolder = @"Templates\TraditionalBridge",
                            UserTokens = new ObservableCollection<UserToken> { new UserToken("MODELS_NAMESPACE", $"{opts.Namespace}.Models") }
                        };
                        CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                        {
                            ListGeneratedFiles = true,
                            MethodCatalog = new Smc(item),
                            OutputFolder = @$"{opts.Project}\Models"
                        };
                        codeGenTask.Structures.AddRange(codeGenTaskSet.MethodCatalog.Structures.Select(k => k.Name));
                        codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                        codeGenTaskSet.Tasks.Add(codeGenTask);

                        if (!Directory.Exists(codeGenTaskSet.OutputFolder))
                            Directory.CreateDirectory(codeGenTaskSet.OutputFolder);

                        new CodeGenerator(codeGenTaskSet).GenerateCode();
                    }

                    // Generate InterfaceService
                    {
                        Console.WriteLine($"{Environment.NewLine}Generating InterfaceService");

                        CodeGenTask codeGenTask = new CodeGenTask
                        {
                            Interface = interfaceName,
                            MultipleStructures = true,
                            Namespace = $"{opts.Namespace}.Client",
                            ReplaceFiles = true,
                            Templates = new ObservableCollection<string> { "InterfaceService" },
                            TemplateFolder = @"Templates\TraditionalBridge",
                            UserTokens = new ObservableCollection<UserToken> { new UserToken("MODELS_NAMESPACE", $"{opts.Namespace}.Models"), new UserToken("DTOS_NAMESPACE", $"{opts.Namespace}.{interfaceName}") }
                        };
                        CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                        {
                            ListGeneratedFiles = true,
                            MethodCatalog = new Smc(item),
                            OutputFolder = @$"{opts.Project}\Client"
                        };
                        codeGenTask.Structures.AddRange(codeGenTaskSet.MethodCatalog.Structures.Select(k => k.Name));
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
                            Interface = interfaceName,
                            MultipleStructures = true,
                            Namespace = $"{opts.Namespace}.Dispatchers",
                            ReplaceFiles = true,
                            Templates = new ObservableCollection<string> { "InterfaceDispatcherData" },
                            TemplateFolder = @"Templates\TraditionalBridge",
                            UserTokens = new ObservableCollection<UserToken> { new UserToken("SMC_INTERFACE", interfaceName), new UserToken("DTOS_NAMESPACE", $"{opts.Namespace}.Dispatchers") }
                        };
                        CodeGenTaskSet codeGenTaskSet = new CodeGenTaskSet
                        {
                            ListGeneratedFiles = true,
                            MethodCatalog = new Smc(item),
                            OutputFolder = @$"{opts.Project}\Dispatchers"
                        };
                        codeGenTask.Structures.AddRange(codeGenTaskSet.MethodCatalog.Structures.Select(k => k.Name));
                        codeGenTaskSet.TaskSetMessage += (msg) => { Console.WriteLine(msg); };
                        codeGenTaskSet.Tasks.Add(codeGenTask);

                        if (!Directory.Exists(codeGenTaskSet.OutputFolder))
                            Directory.CreateDirectory(codeGenTaskSet.OutputFolder);

                        new CodeGenerator(codeGenTaskSet).GenerateCode();
                    }

                    errorCode = 0;
                });
            }

            return errorCode;
        }
    }
}
