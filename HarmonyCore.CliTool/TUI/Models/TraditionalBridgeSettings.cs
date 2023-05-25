using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyCoreGenerator.Model;

namespace HarmonyCore.CliTool.TUI.Models
{
    internal class TraditionalBridgeSettings : SingleItemSettingsBase
    {
        public TraditionalBridgeSettings(SolutionInfo context) : base(context) 
        {
            LoadTraditionalBridgeSettings(context);
        }

        public void LoadTraditionalBridgeSettings(SolutionInfo context)
        {
            var solution = context.CodeGenSolution;
            BaseInterface.LoadSameProperties(context.CodeGenSolution);

            EnableOptionalParameters = solution.TraditionalBridge?.EnableOptionalParameters;
            EnableSampleDispatchers = solution.TraditionalBridge?.EnableSampleDispatchers;
            EnableXFServerPlusMigration = solution.TraditionalBridge?.EnableXFServerPlusMigration;
            XFServerSMCPath = solution.TraditionalBridge?.XFServerSMCPath;

            if (solution.TraditionalBridge != null)
                EnableTraditionalBridge = true;

            Name = "Traditional Bridge";
        }

        public override void Save(SolutionInfo context)
        {
            var solution = context.CodeGenSolution;
            BaseInterface.SaveSameProperties(solution);
            if (EnableTraditionalBridge)
            {
                if (solution.TraditionalBridge == null)
                    solution.TraditionalBridge = new TraditionalBridge();

                solution.TraditionalBridge.XFServerSMCPath = XFServerSMCPath;
                solution.TraditionalBridge.EnableOptionalParameters = EnableOptionalParameters;
                solution.TraditionalBridge.EnableSampleDispatchers = EnableSampleDispatchers;
                solution.TraditionalBridge.EnableXFServerPlusMigration = EnableXFServerPlusMigration;
            }
        }
        [Prompt("Enable Traditional Bridge")]
        public bool EnableTraditionalBridge { get; set; }

        [Prompt("Controllers project")]
        public string ControllersProject { get; set; }
        [Prompt("Isolated project")]
        public string IsolatedProject { get; set; }
        [Prompt("Models project")]
        public string ModelsProject { get; set; }
        [Prompt("Self host project")]
        public string SelfHostProject { get; set; }
        [Prompt("Services project")]
        public string ServicesProject { get; set; }
        [Prompt("Traditional Brige project")]
        public string TraditionalBridgeProject { get; set; }
        [Prompt("Unit test project")]
        public string UnitTestProject { get; set; }
        [Prompt("Enable optional parameters")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? EnableOptionalParameters { get; set; }
        [Prompt("Enable sample dispatchers")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? EnableSampleDispatchers { get; set; }
        [Prompt("Enable generation from SMC")]
        [NullableBoolInjector]
        [NullableBoolExtractor]
        [NullableBoolOptionsExtractor]
        public bool? EnableXFServerPlusMigration { get; set; }

        [Prompt("SMC path")]
        public string XFServerSMCPath { get; set; }
    }
}
