using System.Collections.Generic;
using System.Linq;

namespace HarmonyCore.CliTool
{
    public class SolutionInfo
    {
        public SolutionInfo(IEnumerable<string> projectPaths, string solutionDir)
        {
            SolutionDir = solutionDir;
            Projects = projectPaths.Select(path => new ProjectInfo(path)).ToList();
        }

        public List<ProjectInfo> Projects { get; }
        public string SolutionDir { get; }
    }
}