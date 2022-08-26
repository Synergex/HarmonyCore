using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    internal class SolutionSettings : SingleItemSettingsBase
    {
        public SolutionSettings(SolutionInfo context) : base(context)
        {
            var solution = context.CodeGenSolution;
            BaseInterface.LoadSameProperties(solution);

            Name = "Solution";
        }

        public override void Save(SolutionInfo context)
        {
            BaseInterface.SaveSameProperties(context.CodeGenSolution);
        }

        

        [Prompt("Enable Newtonsoft JSON support")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? EnableNewtonsoftJson { get; set; }
        [Prompt("SignalR Hub Path")]
        public string SignalRPath { get; set; }
        [Prompt("Generated Controllers project folder")]
        public string ControllersFolder { get; set; }
        [Prompt("Data Folder")]
        public string DataFolder { get; set; }
        [Prompt("Generated Isolated project folder")]
        public string IsolatedFolder { get; set; }
        [Prompt("Generated Models project folder")]
        public string ModelsFolder { get; set; }
        [Prompt("Generated Self Host project folder")]
        public string SelfHostFolder { get; set; }
        [Prompt("Generated Services project folder")]
        public string ServicesFolder { get; set; }
        [Prompt("Solution (.sln) folder")]
        public string SolutionFolder { get; set; }
        [Prompt("Codegen template folder")]
        public string TemplatesFolder { get; set; }
        [Prompt("Generated Traditional Bridge project folder")]
        public string TraditionalBridgeFolder { get; set; }
        [Prompt("Generated Unit Test project folder")]
        public string UnitTestFolder { get; set; }
        [Prompt("Namespace for generated client unit test model classes")]
        public string ClientModelsNamespace { get; set; }
        [Prompt("Namespace for generated controller classes")]
        public string ControllersNamespace { get; set; }
        [Prompt("Namespace for generated model classes")]
        public string ModelsNamespace { get; set; }
        [Prompt("Namespace for generated self host classes")]
        public string SelfHostNamespace { get; set; }
        [Prompt("Namespace for generated service classes")]
        public string ServicesNamespace { get; set; }
        [Prompt("Namespace for generated traditional bridge classes")]
        public string TraditionalBridgeNamespace { get; set; }
        [Prompt("Namespace base for generated unit test classes")]
        public string UnitTestsBaseNamespace { get; set; }
        [Prompt("Namespace for generated unit test classes")]
        public string UnitTestsNamespace { get; set; }
    }
}
