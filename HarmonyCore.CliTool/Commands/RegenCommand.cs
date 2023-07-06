using CodeGen.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using HarmonyCoreGenerator.Model;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.Commands
{
    class RegenCommand
    {
        private readonly Lazy<Task<SolutionInfo>> _loader;
        SolutionInfo _solutionInfo => _loader.Value.Result;
        public RegenCommand(Func<Task<SolutionInfo>> solutionInfo)
        {
            _loader = new Lazy<Task<SolutionInfo>>(solutionInfo);
            GenerationEvents = new Solution.SolutionGenerationEvents() { Message = Logger, Error = Logger };
        }
        //Add a filesystem watcher with callbacks and percentages
        //

        public Solution.SolutionGenerationEvents GenerationEvents { get; set; }

        public Action<string> CallerLogger { get; set; } = (str) => Console.WriteLine(str);
        private List<string> AddedFiles { get; } = new List<string>();
        private List<string> UpdatedFiles { get; } = new List<string>();
        public CancellationToken CancelToken { get; set; }

        public bool IsGeneratedFile(string projectPath, string sourceFileName)
        {
            var targetFile = File.ReadLines(sourceFileName).Take(40);
            if (targetFile.Any(line => line.Contains("WARNING: GENERATED CODE!", StringComparison.OrdinalIgnoreCase)))
                return true;
            else
                return false;
        }

        public int Run(RegenOptions opts)
        {
            Dictionary<string, HashSet<string>> syncAddedFiles = new Dictionary<string, HashSet<string>>();
            Dictionary<string, HashSet<string>> syncRemovedFiles = new Dictionary<string, HashSet<string>>();
            var result = Run(opts, syncAddedFiles, syncRemovedFiles);
            if(result == 0 && opts.Sync)
            {
                foreach (var syncTpl in syncAddedFiles)
                    _solutionInfo.Projects.First(projInfo => string.Compare(projInfo.FileName, syncTpl.Key) == 0).AddRemoveFiles(syncTpl.Value, Enumerable.Empty<string>());

                foreach (var syncTpl in syncRemovedFiles)
                    _solutionInfo.Projects.First(projInfo => string.Compare(projInfo.FileName, syncTpl.Key) == 0).AddRemoveFiles(Enumerable.Empty<string>(), syncTpl.Value);
            }
            return result;
        }

        public int Run(RegenOptions opts, Dictionary<string, HashSet<string>> syncAddedFiles, Dictionary<string, HashSet<string>> syncRemovedFiles)
        {
            if (opts.Interfaces.Count() > 0)
            {
                var onlyAllowInterfaces = new HashSet<string>(opts.Interfaces, StringComparer.OrdinalIgnoreCase);
                _solutionInfo.CodeGenSolution.ExtendedInterfaces.RemoveAll(iface => !onlyAllowInterfaces.Contains(iface.Name));
            }

            using (var fsw = new FileSystemWatcher(_solutionInfo.SolutionDir, "*.dbl") { EnableRaisingEvents = true, IncludeSubdirectories = true })
            {
                fsw.Created += Fsw_Created;
                fsw.Changed += Fsw_Changed;

                var result = _solutionInfo.CodeGenSolution.GenerateSolution(GenerationEvents,
                    CancelToken,
                    DynamicCodeGenerator.LoadDynamicGenerators(Path.Combine(_solutionInfo.SolutionDir, "Generators", "Enabled")).Result);

                foreach (var error in result.ValidationErrors)
                {
                    CallerLogger(error);
                }

                var sourceFileLookup = new Dictionary<string, HashSet<string>>();
                foreach(var project in _solutionInfo.Projects)
                {
                    sourceFileLookup.Add(project.FileName, new HashSet<string>(project.SourceFiles, StringComparer.OrdinalIgnoreCase));
                }

                //var modifiedButNotAdded = new Dictionary<string, HashSet<string>>();
                var modifiedOrAdded = new HashSet<string>(UpdatedFiles.Concat(AddedFiles));

                foreach(var updatedFile in UpdatedFiles)
                {
                    var closestProject = FindClosestProject(sourceFileLookup, updatedFile);

                    if (closestProject != null && !sourceFileLookup[closestProject].Contains(updatedFile))
                        AddOrInsert(syncAddedFiles, closestProject, updatedFile);
                }
                //Traditional bridge is configured to output in 'source' folder, bad bat read
                //test gen not enabled, looks like its missing from regen.bat
                foreach (var updatedFile in AddedFiles)
                {
                    var closestProject = FindClosestProject(sourceFileLookup, updatedFile);

                    if (closestProject != null && !sourceFileLookup[closestProject].Contains(updatedFile))
                        AddOrInsert(syncAddedFiles, closestProject, updatedFile);
                }

                foreach(var file in Directory.GetFiles(_solutionInfo.SolutionDir, "*.dbl", SearchOption.AllDirectories))
                {
                    var closestProject = FindClosestProject(sourceFileLookup, file);
                    if (closestProject != null && !modifiedOrAdded.Contains(file) && IsGeneratedFile(closestProject, file))
                        AddOrInsert(syncRemovedFiles, closestProject, file);
                }

                if (syncAddedFiles.Any())
                {
                    CallerLogger("*** Files that need to be added to projects ***");
                    foreach(var kvp in syncAddedFiles)
                    {
                        foreach (var file in kvp.Value)
                            CallerLogger(file);
                    }
                }
                if (syncRemovedFiles.Any())
                {
                    CallerLogger("*** Files that look like they need to be deleted/removed from projects ***");
                    foreach (var kvp in syncRemovedFiles)
                    {
                        foreach (var file in kvp.Value)
                            CallerLogger(file);
                    }
                }
            }
            return 0;
        }

        private string FindClosestProject(Dictionary<string, HashSet<string>> projectLookup, string targetFile)
        {
            var closestProject = projectLookup
                        .Where(kvp => targetFile.StartsWith(Path.GetDirectoryName(kvp.Key), StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(kvp => kvp.Key.Length)
                        .FirstOrDefault();
            //KeyValuePair is a value type so only the members will be null if there was no match
            return closestProject.Key;
        }

        private void AddOrInsert(Dictionary<string, HashSet<string>> destination, string key, string item)
        {
            if (destination.TryGetValue(key, out var result))
            {
                if (!result.Contains(item))
                    result.Add(item);
            }
            else
                destination.Add(key, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { item });
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            UpdatedFiles.Add(e.FullPath);
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            AddedFiles.Add(e.FullPath);
        }

        private void Logger(CodeGenTask tsk, string message)
        {
            if(!string.IsNullOrWhiteSpace(message))
                if(CallerLogger != null)
                {
                    CallerLogger(string.Format("{0} : {1}", string.Join(',', tsk.Templates), message));
                }
        }
    }
}
