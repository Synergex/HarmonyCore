using CodeGen.Engine;
using System;
using System.Collections.Generic;
using System.IO;
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
            var result = _solutionInfo.CodeGenSolution.GenerateSolution(Logger,
                CancellationToken.None, 
                DynamicCodeGenerator.LoadDynamicGenerators(Path.Combine(_solutionInfo.SolutionDir, "Generators", "Enabled")).Result);

            foreach (var error in result.ValidationErrors)
            {
                Console.WriteLine(error);
            }
            return 0;
        }

        private void Logger(CodeGenTask tsk, string message)
        {
            if(!string.IsNullOrWhiteSpace(message))
                Console.WriteLine("{0} : {1}", string.Join(',', tsk.Templates), message);
        }
    }
}
