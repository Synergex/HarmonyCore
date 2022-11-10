using HarmonyCoreGenerator.Generator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CodeGen.RepositoryAPI;
using HarmonyCoreGenerator.Model;
using CodeGen.MethodCatalogAPI;

namespace HarmonyCore.CliTool.Commands
{
    class CodegenCommand
    {
        private readonly Lazy<SolutionInfo> _loader;
        SolutionInfo _solutionInfo => _loader.Value;
        Dictionary<string, RpsStructure> _structureLookup;
        Dictionary<string, StructureEx> _extendedStructureLookup;
        Dictionary<string, SmcInterface> _interfaceLookup;
        Dictionary<string, InterfaceEx> _extendedInterfaceLookup;

        public CodegenCommand(Func<SolutionInfo> solutionInfo)
        {
            _loader = new Lazy<SolutionInfo>(solutionInfo);
        }

        private void LazyLoadLookups()
        {
            if (_structureLookup == null)
                _structureLookup = _solutionInfo.CodeGenSolution.RPS.Structures.ToDictionary(strc => strc.Name, StringComparer.OrdinalIgnoreCase);
            if (_extendedStructureLookup == null)
                _extendedStructureLookup = _solutionInfo.CodeGenSolution.ExtendedStructures.ToDictionary(strc => strc.Name, StringComparer.OrdinalIgnoreCase);

            if (_interfaceLookup == null)
                _interfaceLookup = _solutionInfo.CodeGenSolution.TraditionalBridge.Smc.Interfaces.ToDictionary(iface => iface.Name, StringComparer.OrdinalIgnoreCase);

            if (_extendedInterfaceLookup == null)
                _extendedInterfaceLookup = _solutionInfo.CodeGenSolution.ExtendedInterfaces.ToDictionary(iface => iface.Name, StringComparer.OrdinalIgnoreCase);
        }

        public int Add(CodegenAddOptions opts)
        {
            LazyLoadLookups();
            if (opts.Interface)
            {
                foreach (var interfaceName in opts.Items)
                {
                    if (!_interfaceLookup.TryGetValue(interfaceName, out var rpsIFace))
                    {
                        Console.WriteLine("failed to find interface {0} in smc", interfaceName);
                        return -1;
                    }

                    if (!_extendedInterfaceLookup.TryGetValue(interfaceName, out var interfaceEx))
                    {
                        interfaceEx = new HarmonyCoreGenerator.Model.InterfaceEx { Name = interfaceName };
                        _extendedInterfaceLookup.Add(interfaceName, interfaceEx);
                        _solutionInfo.CodeGenSolution.ExtendedInterfaces.Add(interfaceEx);
                    }

                    interfaceEx.GenerateWebAPIController = opts.TBWebApi;
                    interfaceEx.GenerateSignalRHub = opts.TBSignalR;
                    interfaceEx.GenerateInterface = true;
                }
            }
            else if (opts.Structure)
            {
                foreach (var structureName in opts.Items)
                {
                    string alias = null;
                    string name = null;
                    //if we have an alias spec split it
                    if (structureName.Contains(":"))
                    {
                        var aliasSplit = structureName.Split(':');
                        name = aliasSplit[0];
                        alias = aliasSplit[1];
                    }
                    else
                    {
                        name = structureName;
                    }

                    if (!_structureLookup.TryGetValue(name, out var rpsStruct))
                    {
                        Console.WriteLine("failed to find structure {0} in repository", name);
                        return -1;
                    }

                    if (!_extendedStructureLookup.TryGetValue(name, out var structEx))
                    {
                        structEx = new HarmonyCoreGenerator.Model.StructureEx { Name = name, Aliases = new List<string> { alias } };
                        _extendedStructureLookup.Add(name, structEx);
                        _solutionInfo.CodeGenSolution.ExtendedStructures.Add(structEx);
                    }
                    else
                    {
                        structEx.Aliases.Add(alias);
                    }

                    if (structEx.EnabledGenerators == null)
                        structEx.EnabledGenerators = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    if (opts.OData)
                    {
                        if (!structEx.EnabledGenerators.Contains(nameof(ODataGenerator)))
                            structEx.EnabledGenerators.Add(nameof(ODataGenerator));
                        if (!structEx.EnabledGenerators.Contains(nameof(EFCoreGenerator)))
                            structEx.EnabledGenerators.Add(nameof(EFCoreGenerator));
                        if (!structEx.EnabledGenerators.Contains(nameof(ModelGenerator)))
                            structEx.EnabledGenerators.Add(nameof(ModelGenerator));
                    }

                    if (opts.Custom)
                    {
                        if (!structEx.EnabledGenerators.Contains(nameof(ModelGenerator)))
                            structEx.EnabledGenerators.Add(nameof(ODataGenerator));
                    }

                    if (opts.Ef)
                    {
                        if (!structEx.EnabledGenerators.Contains(nameof(ModelGenerator)))
                            structEx.EnabledGenerators.Add(nameof(ModelGenerator));
                        if (!structEx.EnabledGenerators.Contains(nameof(EFCoreGenerator)))
                            structEx.EnabledGenerators.Add(nameof(EFCoreGenerator));
                    }
                }
            }
            else
            {
                Console.WriteLine("invalid arguments");
                return -1;
            }
            _solutionInfo.SaveSolution();
            return 0;
        }

        public int Remove(CodegenRemoveOptions opts)
        {
            LazyLoadLookups();
            if (opts.Interface)
            {
                foreach (var interfaceName in opts.Items)
                {
                    if (!_interfaceLookup.TryGetValue(interfaceName, out var rpsInterface))
                    {
                        Console.WriteLine("failed to find interface {0} in smc", interfaceName);
                        return -1;
                    }

                    if (_extendedInterfaceLookup.TryGetValue(interfaceName, out var interfaceEx))
                    {
                        _solutionInfo.CodeGenSolution.ExtendedInterfaces.Remove(interfaceEx);
                        _extendedInterfaceLookup.Remove(interfaceName);
                    }
                }
            }
            else if (opts.Structure)
            {
                foreach (var structureName in opts.Items)
                {
                    if (!_structureLookup.TryGetValue(structureName, out var rpsStructure))
                    {
                        Console.WriteLine("failed to find structure {0} in repository", structureName);
                        return -1;
                    }

                    if (_extendedStructureLookup.TryGetValue(structureName, out var structEx))
                    {
                        _solutionInfo.CodeGenSolution.ExtendedStructures.Remove(structEx);
                        _extendedStructureLookup.Remove(structureName);
                    }
                }
            }
            _solutionInfo.SaveSolution();
            return 0;
        }

        public int List(CodegenListOptions opts)
        {
            foreach (var structure in _solutionInfo.CodeGenSolution.ExtendedStructures)
            {
                if ((structure.Aliases?.Count ?? 0) > 0)
                {
                    Console.WriteLine("{0} -> {1} : {2}", structure.Name, string.Join("|", structure.Aliases), string.Join("|", structure.EnabledGenerators));
                }
                else
                    Console.WriteLine("{0} : {1}", structure.Name, string.Join("|", structure.EnabledGenerators));
            }
            return 0;
        }
    }
}
