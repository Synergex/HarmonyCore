using CodeGen.Engine;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace HarmonyCore.CliTool.Commands
{
    class XMLGenCommand
    {
        public int Run(XMLGenOptions xmlGenOptions)
        {
            // Generate Models
            {
                CodeGenTaskSet taskSet = new CodeGenTaskSet
                {
                    EchoCommands = true,
                    ListGeneratedFiles = true
                };
                CodeGenTask task = new CodeGenTask
                {
                    Structures = new ObservableCollection<string>(xmlGenOptions.Structures),
                    Templates = new ObservableCollection<string>
                    {
                        "TraditionalModel",
                        "TraditionalMetadata"
                    },
                    TemplateFolder = @"Templates\TraditionalBridge",
                    OutputFolder = @"TraditionalBridge.Test\Models",
                    Namespace = @"TraditionalBridge.Test.Models",
                    ReplaceFiles = true
                };
                taskSet.Tasks.Add(task);
                taskSet.Messages.CollectionChanged += Messages_CollectionChanged;

                CodeGenerator codeGenerator = new CodeGenerator(taskSet);
                if (!codeGenerator.GenerateCode())
                    return 1;
            }

            // Generate from SMC xmls
            {
                foreach (string item in Directory.GetFiles(xmlGenOptions.XMLFolder, "*.xml", SearchOption.AllDirectories))
                {
                    string xmlFileName = Path.GetFileNameWithoutExtension(item);

                    // InterfaceDispatcher
                    {
                        CodeGenTaskSet taskSet = new CodeGenTaskSet
                        {
                            EchoCommands = true,
                            ListGeneratedFiles = true
                        };
                        CodeGenTask task = new CodeGenTask
                        {
                            MethodCatalogFile = item,
                            MethodCatalogInterface = xmlFileName,
                            TemplateFolder = @"Templates\TraditionalBridge",
                            Templates = new ObservableCollection<string> { "InterfaceDispatcher" },
                            OutputFolder = @"TraditionalBridge.Test\Dispatcher",
                            Namespace = "TraditionalBridge.Test",
                            UserTokens = new ObservableCollection<UserToken>
                            {
                                new UserToken("MODELS_NAMESPACE", "TraditionalBridge.Test.Models"),
                                new UserToken("DTOS_NAMESPACE", "TraditionalBridge.Test.Models")
                            },
                            ReplaceFiles = true
                        };
                        taskSet.Tasks.Add(task);
                        taskSet.Messages.CollectionChanged += Messages_CollectionChanged;

                        CodeGenerator codeGenerator = new CodeGenerator(taskSet);
                        if (!codeGenerator.GenerateCode())
                            return 1;
                    }

                    // InterfaceMethodDispatchers
                    {
                        CodeGenTaskSet taskSet = new CodeGenTaskSet
                        {
                            EchoCommands = true,
                            ListGeneratedFiles = true
                        };
                        CodeGenTask task = new CodeGenTask
                        {
                            MethodCatalogFile = item,
                            MethodCatalogInterface = xmlFileName,
                            TemplateFolder = @"Templates\TraditionalBridge",
                            Templates = new ObservableCollection<string> { "InterfaceMethodDispatchers" },
                            OutputFolder = @"TraditionalBridge.Test\MethodDispatchers",
                            Namespace = "TraditionalBridge.Test",
                            UserTokens = new ObservableCollection<UserToken>
                            {
                                new UserToken("MODELS_NAMESPACE", "TraditionalBridge.Test.Models"),
                                new UserToken("DTOS_NAMESPACE", "TraditionalBridge.Test.Models")
                            },
                            ReplaceFiles = true
                        };
                        taskSet.Tasks.Add(task);
                        taskSet.Messages.CollectionChanged += Messages_CollectionChanged;

                        CodeGenerator codeGenerator = new CodeGenerator(taskSet);
                        if (!codeGenerator.GenerateCode())
                            return 1;
                    }

                    // Continue after error on these tasks
                    {
                        CodeGenTaskSet taskSet = new CodeGenTaskSet
                        {
                            EchoCommands = true,
                            ListGeneratedFiles = true,
                            ContinueAfterError = true
                        };
                        // Generate the request and response models for the service class methods
                        CodeGenTask multiInterfaceServiceModelsTask = new CodeGenTask
                        {
                            MethodCatalogFile = item,
                            MethodCatalogInterface = xmlFileName,
                            TemplateFolder = @"Templates\TraditionalBridge",
                            Templates = new ObservableCollection<string> { "MultiInterfaceServiceModels" },
                            OutputFolder = @"TraditionalBridge.TestClient\Client",
                            Namespace = "TraditionalBridge.TestClient",
                            UserTokens = new ObservableCollection<UserToken>
                            {
                                new UserToken("MODELS_NAMESPACE", "TraditionalBridge.Models"),
                                new UserToken("DTOS_NAMESPACE", $"TraditionalBridge.TestClient.{xmlFileName}")
                            },
                            ReplaceFiles = true
                        };
                        taskSet.Tasks.Add(multiInterfaceServiceModelsTask);

                        // Generate the service class
                        CodeGenTask interfaceServiceTask = new CodeGenTask
                        {
                            MethodCatalogFile = item,
                            MethodCatalogInterface = xmlFileName,
                            TemplateFolder = @"Templates\TraditionalBridge",
                            Templates = new ObservableCollection<string> { "InterfaceService" },
                            OutputFolder = @"TraditionalBridge.TestClient\Client",
                            Namespace = "TraditionalBridge.TestClient",
                            UserTokens = new ObservableCollection<UserToken>
                            {
                                new UserToken("MODELS_NAMESPACE", "TraditionalBridge.Models"),
                                new UserToken("DTOS_NAMESPACE", $"TraditionalBridge.TestClient.{xmlFileName}")
                            },
                            ReplaceFiles = true
                        };
                        taskSet.Tasks.Add(interfaceServiceTask);

                        // Generate model and metadata classes
                        CodeGenTask oDataTask = new CodeGenTask
                        {
                            MethodCatalogStructureMode = true,
                            MethodCatalogFile = item,
                            MethodCatalogInterface = xmlFileName,
                            TemplateFolder = @"Templates\TraditionalBridge",
                            Templates = new ObservableCollection<string>
                            {
                                "ODataModel",
                                "ODataMetaData"
                            },
                            OutputFolder = @"TraditionalBridge.Models\Models",
                            Namespace = "TraditionalBridge.Models",
                            UserTokens = new ObservableCollection<UserToken>
                            {
                                new UserToken("DTOS_NAMESPACE", $"TraditionalBridge.Models")
                            },
                            ReplaceFiles = true
                        };
                        taskSet.Tasks.Add(oDataTask);

                        taskSet.Messages.CollectionChanged += Messages_CollectionChanged;

                        CodeGenerator codeGenerator = new CodeGenerator(taskSet);
                        _ = codeGenerator.GenerateCode();
                    }
                }
            }

            Console.WriteLine();
            return 0;
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (LogEntry item in notifyCollectionChangedEventArgs.NewItems)
                {
                    Console.WriteLine(item.Message);
                }
            }
        }
    }
}
