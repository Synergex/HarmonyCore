using CodeGen.RepositoryAPI;
using HarmonyCoreExtensions;
using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HarmonyCoreExtensions.Helpers;

namespace HarmonyCore.CliTool.Commands
{
    class RPSCommand
    {
        private readonly Lazy<Task<SolutionInfo>> _loader;
        SolutionInfo _solutionInfo => _loader.Value.Result;
        public RPSCommand(Func<Task<SolutionInfo>> solutionInfo)
        {
            _loader = new Lazy<Task<SolutionInfo>>(solutionInfo);
        }

        public int Run(RpsOptions opts)
        {
            StructureEx selectedStructEx = null;
            RpsStructure selectedStruct = null;
            RpsField selectedField = null;

            if (!string.IsNullOrWhiteSpace(opts.Structure))
            {
                selectedStructEx = _solutionInfo.CodeGenSolution.ExtendedStructures.FirstOrDefault(strct => string.Compare(strct.Name, opts.Structure, true) == 0);
                selectedStruct = _solutionInfo.CodeGenSolution.RPS.GetStructure(opts.Structure);
            }

            if (!string.IsNullOrWhiteSpace(opts.Field))
            {
                selectedField = selectedStruct.Fields.FirstOrDefault(fld => string.Compare(fld.Name, opts.Field, true) == 0);
            }


            if (opts.ListStructures)
            {
                foreach (var structure in _solutionInfo.CodeGenSolution.RPS.Structures)
                {
                    if (String.IsNullOrEmpty(structure.Alias) || structure.Alias == structure.Name)
                        Console.WriteLine(structure.Name);
                    else
                        Console.WriteLine("{0}:{1}", structure.Name, structure.Alias);
                }
            }
            else if (opts.ListFields)
            {
                foreach (var field in selectedStruct.Fields)
                {
                    if (String.IsNullOrEmpty(field.AlternateName) || field.AlternateName == field.Name)
                        Console.WriteLine("{0}:{1}", field.Name, field.TypeName);
                    else
                        Console.WriteLine("{0}:{1}:{2}", field.Name, field.AlternateName, field.TypeName);
                }
            }
            else if (opts.ListKeys)
            {
                foreach (var key in selectedStruct.Keys)
                {
                    Console.WriteLine("{0}:{1}:{2}", key.Name, key.Size, string.Join('|', key.Segments.Select(seg => string.Format("{0}-{1}-{2}-{3}-{4}", seg.Field, seg.DataType, seg.Length, seg.LiteralValue ?? "null", seg.SegmentType))));
                }
            }
            else if (opts.ListRelations)
            {
                var loadedRelations = _solutionInfo.CodeGenSolution.ExtendedStructures.SelectMany(strct => strct.RelationsSpecs).ToList();

                var relationLookup = new RelationLookup(_solutionInfo.CodeGenSolution.RPS.Structures, loadedRelations);

                foreach (var rel in selectedStruct.Relations)
                {
                    var outRelSpec = relationLookup.FindRelation(rel.FromStructure, rel.ToStructure, rel.FromKey, rel.ToKey, false);
                    Console.WriteLine("{0}:{1}:{2}:{3}", outRelSpec.RelationName, outRelSpec.RelationType, outRelSpec.ToKey, outRelSpec.ToStructure);
                }
            }
            else if (opts.Properties != null)
            {
                if (selectedStruct != null && selectedStructEx == null)
                {
                    selectedStructEx = new StructureEx { Name = opts.Structure };
                    _solutionInfo.CodeGenSolution.ExtendedStructures.Add(selectedStructEx);
                }

                foreach (var prop in opts.Properties)
                {
                    var kvp = prop.Split(':', StringSplitOptions.None);
                    
                    if (!opts.RemoveProperties && kvp.Length != 2)
                        throw new InvalidOperationException("property value didnt match key:value format");
                    else if(opts.RemoveProperties && kvp.Length != 1)
                        throw new InvalidOperationException("property value didnt match key format");

                    if (selectedField != null)
                    {
                        if (!selectedStructEx.Fields.TryGetValue(selectedField.Name, out var props))
                        {
                            props = new Dictionary<string, object>();
                            selectedStructEx.Fields.Add(selectedField.Name, props);
                        }

                        if (opts.RemoveProperties)
                            props.Remove(kvp[0]);
                        else
                            props.Add(kvp[0], kvp[1]);
                    }
                    else if (selectedStructEx != null)
                    {
                        var value = opts.RemoveProperties ? false : bool.Parse(kvp[1]);
                        switch (kvp[0].ToUpper())
                        {
                            case "NO_GET_ALL_ENDPOINT":
                                selectedStructEx.EnableGetAll = value;
                                break;
                            case "NO_GET_ENDPOINT":
                                selectedStructEx.EnableGetOne = value;
                                break;
                            case "NO_POST_ENDPOINT":
                                selectedStructEx.EnablePost = value;
                                break;
                            case "NO_PUT_ENDPOINT":
                                selectedStructEx.EnablePut = value;
                                break;
                            case "NO_PATCH_ENDPOINT":
                                selectedStructEx.EnablePatch = value;
                                break;
                            case "NO_DELETE_ENDPOINT":
                                selectedStructEx.EnableDelete = value;
                                break;
                            case "NO_ALTERNATE_KEY_ENDPOINTS":
                                selectedStructEx.EnableAltGet = value;
                                break;
                            default:
                                throw new InvalidOperationException("unrecognized propery " + kvp[0]);
                        }
                    }
                    else
                        throw new InvalidOperationException("must specify a structure or field to operate on");
                }

                _solutionInfo.SaveSolution();
            }

            return 0;
        }
    }
}
