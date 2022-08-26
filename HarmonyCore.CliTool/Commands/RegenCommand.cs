using CodeGen.Engine;
using System;
using System.Collections.Generic;
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

        public Action<string> CallerLogger { get; set; } = (str) => Console.WriteLine(str);

        public int Run(RegenOptions opts)
        {
            if (opts.Interfaces.Count() > 0)
            {
                var onlyAllowInterfaces = new HashSet<string>(opts.Interfaces, StringComparer.OrdinalIgnoreCase);
                _solutionInfo.CodeGenSolution.ExtendedInterfaces.RemoveAll(iface => !onlyAllowInterfaces.Contains(iface.Name));
            }

            var result = _solutionInfo.CodeGenSolution.GenerateSolution(Logger,
                CancellationToken.None, 
                DynamicCodeGenerator.LoadDynamicGenerators(Path.Combine(_solutionInfo.SolutionDir, "Generators", "Enabled")).Result);

            foreach (var error in result.ValidationErrors)
            {
                CallerLogger(error);
            }

            return 0;
        }

        private void Logger(CodeGenTask tsk, string message)
        {
            if(!string.IsNullOrWhiteSpace(message))
                CallerLogger(string.Format("{0} : {1}", string.Join(',', tsk.Templates), message));
        }
    }
}
