using System;
using System.Collections.Generic;
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
            var result = _solutionInfo.CodeGenSolution.GenerateSolution((tsk, message) => Console.WriteLine("{0} : {1}", string.Join(',', tsk.Templates), message), CancellationToken.None);

            foreach (var error in result.ValidationErrors)
            {
                Console.WriteLine(error);
            }
            return 0;
        }
    }
}
