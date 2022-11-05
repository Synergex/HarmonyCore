using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace HarmonyCore.CliTool
{
    public class ProjectInfo
    {
        private static HashSet<string> ObsoletePackages = new HashSet<string>
        {
            "Microsoft.AspNetCore.Session",
            "Microsoft.AspNetCore.StaticFiles",
            "Microsoft.AspNetCore.Mvc.Core",
            "Microsoft.AspNetCore.Mvc",
        };

        private static HashSet<string> WebReferenceProjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Services.Controllers"
        };

        private static HashSet<string> CodeDomProjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Services.Controllers",
            "Services",
            "Services.Host",
            "Services.Test",
            "Services.Isolated"
        };
        public string FileName { get; set; }
        public XmlDocument ProjectDoc { get; set; }
        public Project MSBuildProject { get; set; }

        public ProjectInfo(string path, Project msbuildProject)
        {
            MSBuildProject = msbuildProject;
            FileName = path;
            ProjectDoc = new XmlDocument { PreserveWhitespace = true };
            ProjectDoc.Load(path);
        }

        IEnumerable<string> _sourceFiles;
        public IEnumerable<string> SourceFiles 
        { 
            get
            {
                if(_sourceFiles == null )
                {
                    _sourceFiles = MSBuildProject.GetItems("Compile").Select(itm =>
                    {
                        if(Path.IsPathRooted(itm.EvaluatedInclude))
                            return itm.EvaluatedInclude;
                        else
                            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(FileName), itm.EvaluatedInclude));
                    }).ToList();
                }
                return _sourceFiles;
            }
        }

        public void AddRemoveFiles(IEnumerable<string> toAdd, IEnumerable<string> toRemove)
        {
            foreach (var item in toAdd)
                MSBuildProject.AddItem("Compile", item);

            foreach (var item in toRemove)
                foreach(var msbuildItem in MSBuildProject.GetItemsByEvaluatedInclude(item).ToList())
                    MSBuildProject.RemoveItem(msbuildItem);
        }

        public void PatchKnownIssues(VersionTargetingInfo versionInfo)
        {
            var cleanFileName = Path.GetFileNameWithoutExtension(FileName);
            //look for bad .net core version stuff, <RuntimeFrameworkVersion> shouldn't be here
            var runtimeFrameworkVersion = ProjectDoc.GetElementsByTagName("RuntimeFrameworkVersion").OfType<XmlNode>().FirstOrDefault();
            if (runtimeFrameworkVersion != null)
            {
                runtimeFrameworkVersion.ParentNode.RemoveChild(runtimeFrameworkVersion);
            }

            var importNodes = new List<XmlNode>();
            foreach (var import in ProjectDoc.GetElementsByTagName("Import").OfType<XmlNode>().ToList())
            {
                //obviously a duplicate remove it now
                var projectPath = import.Attributes["Project"]?.Value ?? "";
                if (projectPath.EndsWith("Common.props"))
                {
                    importNodes.Add(import);
                }
            }

            var existingImport = importNodes.FirstOrDefault();
            if (existingImport != null)
            {
                existingImport.Attributes["Project"].Value = "$(SolutionDir)Common.props";
            }

            foreach (var import in importNodes.Skip(1))
            {
                import.ParentNode.RemoveChild(import);
            }

            FixRPS(ProjectDoc, versionInfo.BuildPackageVersion, cleanFileName);


            //switch .net standard 2.0 projects to netcoreapp3.1
            var targetFramework = ProjectDoc.GetElementsByTagName("TargetFramework").OfType<XmlNode>().FirstOrDefault();

            if (targetFramework != null)
            {
                targetFramework.InnerText = versionInfo.TargetFramework;
            }

            var firstItemGroup = ProjectDoc.GetElementsByTagName("ItemGroup").OfType<XmlNode>().FirstOrDefault();
            if (firstItemGroup == null)
            {
                firstItemGroup = ProjectDoc.CreateElement("ItemGroup");
                ProjectDoc.DocumentElement.AppendChild(firstItemGroup);
            }

            XmlNode firstPropertyGroup = EnsurePropertyGroup(ProjectDoc);

            //upgrade Host and test to Web sdk if needed
            //Add SDK reference to Services.Controllers

            if (WebReferenceProjects.Contains(cleanFileName))
            {
                var hasFramworkReference = ProjectDoc.GetElementsByTagName("FrameworkReference").OfType<XmlNode>()
                    .Any(node => node.Attributes["Include"]?.Value == "Microsoft.AspNetCore.App");
                if (!hasFramworkReference)
                {
                    var webReference = ProjectDoc.CreateElement("FrameworkReference");
                    var webReferenceName = ProjectDoc.CreateAttribute("Include");
                    webReferenceName.Value = "Microsoft.AspNetCore.App";
                    webReference.Attributes.Append(webReferenceName);
                    firstItemGroup.AppendChild(webReference);
                }

                var hasOdataVersioning = ProjectDoc.GetElementsByTagName("PackageReference").OfType<XmlNode>()
                        .Any(node => node.Attributes["Include"]?.Value == "Microsoft.AspNetCore.OData.Versioning.ApiExplorer");
                if (!hasOdataVersioning && versionInfo.NugetReferences.ContainsKey("Microsoft.AspNetCore.OData.Versioning.ApiExplorer"))
                {
                    var versioningReference = ProjectDoc.CreateElement("PackageReference");
                    var versioningReferenceName = ProjectDoc.CreateAttribute("Include");
                    versioningReferenceName.Value = "Microsoft.AspNetCore.OData.Versioning.ApiExplorer";
                    var versioningReferenceVersion = ProjectDoc.CreateAttribute("Version");
                    versioningReferenceVersion.Value = versionInfo.NugetReferences["Microsoft.AspNetCore.OData.Versioning.ApiExplorer"];
                    versioningReference.Attributes.Append(versioningReferenceName);
                    versioningReference.Attributes.Append(versioningReferenceVersion);
                    firstItemGroup.AppendChild(versioningReference);
                }
            }

            if (CodeDomProjects.Contains(cleanFileName))
            {
                var codeDomInjector = ProjectDoc.GetElementsByTagName("PackageReference").OfType<XmlNode>()
                .FirstOrDefault(node => node.Attributes["Include"]?.Value == "HarmonyCore.CodeDomProvider");
                if (codeDomInjector != null)
                {
                    codeDomInjector.ParentNode.RemoveChild(codeDomInjector);
                }

                var hasRazorPartsDisabled = ProjectDoc.GetElementsByTagName("GenerateMvcApplicationPartsAssemblyAttributes").Count > 0;
                if (!hasRazorPartsDisabled)
                {
                    var partsNode = ProjectDoc.CreateElement("GenerateMvcApplicationPartsAssemblyAttributes");
                    partsNode.AppendChild(ProjectDoc.CreateTextNode("false"));
                    firstPropertyGroup.AppendChild(partsNode);
                }
            }
            if (string.Compare(cleanFileName, "Services", true) == 0)
            {
                var hasNewtonsoft = ProjectDoc.GetElementsByTagName("PackageReference").OfType<XmlNode>()
                .Any(node => node.Attributes["Include"]?.Value == "Microsoft.AspNetCore.Mvc.NewtonsoftJson");
                if (!hasNewtonsoft)
                {
                    var fRef = ProjectDoc.CreateElement("PackageReference");
                    var fRefName = ProjectDoc.CreateAttribute("Include");
                    fRefName.Value = "Microsoft.AspNetCore.Mvc.NewtonsoftJson";
                    var fRefVersion = ProjectDoc.CreateAttribute("Version");
                    fRefVersion.Value = versionInfo.NugetReferences["Microsoft.AspNetCore.Mvc.NewtonsoftJson"];
                    fRef.Attributes.Append(fRefName);
                    fRef.Attributes.Append(fRefVersion);
                    firstItemGroup.AppendChild(fRef);
                }
            }

            if (string.Compare(cleanFileName, "Services.Test", true) == 0 || string.Compare(cleanFileName, "Services.Host", true) == 0)
            {
                var hasGenerateMain = ProjectDoc.GetElementsByTagName("ProvidesMainMethod").OfType<XmlNode>().Any();
                if (!hasGenerateMain)
                {
                    var provideMainNode = ProjectDoc.CreateElement("ProvidesMainMethod");
                    provideMainNode.InnerText = "true";
                    firstPropertyGroup.AppendChild(provideMainNode);
                }
            }

            foreach (var refToRemove in versionInfo.RemoveNugetReferences)
            {
                var actualRef = ProjectDoc.GetElementsByTagName("PackageReference").OfType<XmlNode>()
                    .FirstOrDefault(node => string.Compare(node.Attributes["Include"]?.Value, refToRemove, true) == 0);
                if (actualRef != null)
                {
                    actualRef.ParentNode.RemoveChild(actualRef);
                }
            }
        }

        private static XmlNode EnsurePropertyGroup(XmlDocument projectDoc)
        {
            var firstPropertyGroup = projectDoc.GetElementsByTagName("PropertyGroup").OfType<XmlNode>().FirstOrDefault();
            if (firstPropertyGroup == null)
            {
                firstPropertyGroup = projectDoc.CreateElement("PropertyGroup", projectDoc.DocumentElement.NamespaceURI);
                projectDoc.DocumentElement.AppendChild(firstPropertyGroup);
            }

            return firstPropertyGroup;
        }

        public static void FixRPS(XmlDocument projectDoc, string targetVersion, string cleanFileName)
        {
            //look for bad explicit nuget pathing
            //switch to this style of rps target
            //<Import Condition="Exists('$(MSBuildExtensionsPath)\Synergex\dbl\Synergex.SynergyDE.Repository.targets')" Project="$(MSBuildExtensionsPath)\Synergex\dbl\Synergex.SynergyDE.Repository.targets" />
            //<Import Condition="!Exists('$(MSBuildExtensionsPath)\Synergex\dbl\Synergex.SynergyDE.Repository.targets')" Project="$(MSBuildProgramFiles32)\MSBuild\Synergex\dbl\Synergex.SynergyDE.Repository.targets" />
            if (string.Compare(cleanFileName, "Repository", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var targetMonikerElement = projectDoc.GetElementsByTagName("NugetTargetMoniker").OfType<XmlNode>().FirstOrDefault();
                XmlNode firstPropertyGroup = EnsurePropertyGroup(projectDoc);

                if(targetMonikerElement == null)
                {
                    targetMonikerElement = projectDoc.CreateElement("NugetTargetMoniker", projectDoc.DocumentElement.NamespaceURI);
                    targetMonikerElement.InnerText = "RPS,Version=1.0";
                    firstPropertyGroup.AppendChild(targetMonikerElement);
                }

                var hasRepositoryTargets = false;
                var imports = projectDoc.GetElementsByTagName("Import").OfType<XmlNode>().ToList();
                foreach (var import in imports)
                {
                    var projectPath = import.Attributes["Project"]?.Value ?? "";
                    if (projectPath.Contains("Synergex.SynergyDE.Build.targets") || projectPath.Contains("Synergex.SynergyDE.Repository.targets"))
                    {
                        import.ParentNode.RemoveChild(import);
                    }
                    //else if (string.Compare(projectPath ,
                    //    "$(MSBuildProgramFiles32)\\MSBuild\\Synergex\\dbl\\Synergex.SynergyDE.Repository.targets", 
                    //    StringComparison.OrdinalIgnoreCase) == 0)
                    //{
                    //    hasRepositoryTargets = true;
                    //}
                    //else if (string.Compare(projectPath,
                    //    "$(ProgramFilesx86)\\MSBuild\\Synergex\\dbl\\Synergex.SynergyDE.Repository.targets",
                    //    StringComparison.OrdinalIgnoreCase) == 0)
                    //{
                    //    hasRepositoryTargets = true;
                    //    import.Attributes["Project"].Value = "$(MSBuildProgramFiles32)\\MSBuild\\Synergex\\dbl\\Synergex.SynergyDE.Repository.targets";
                    //    import.Attributes["Condition"].Value = "!Exists('$(MSBuildExtensionsPath)\\Synergex\\dbl\\Synergex.SynergyDE.Repository.targets')";
                    //}
                }

                var targets = projectDoc.GetElementsByTagName("Target").OfType<XmlNode>().ToList();
                foreach (var target in targets)
                {
                    var targetNameAttr = target.Attributes["Name"];
                    if (string.Compare(targetNameAttr?.Value, "EnsureNuGetPackageBuildImports",
                        StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        target.ParentNode.RemoveChild(target);
                    }
                }

                var importFragment = projectDoc.CreateElement("Import", projectDoc.DocumentElement.NamespaceURI);
                var projectAttr = projectDoc.CreateAttribute("Project");
                projectAttr.Value = $"$(USERPROFILE)\\.nuget\\packages\\synergex.synergyde.build\\{targetVersion}\\build\\rps\\Synergex.SynergyDE.Build.targets";
                var conditionAttr = projectDoc.CreateAttribute("Condition");
                conditionAttr.Value = $"Exists('$(USERPROFILE)\\.nuget\\packages\\synergex.synergyde.build\\{targetVersion}\\build\\rps\\Synergex.SynergyDE.Build.targets')";
                importFragment.Attributes.Append(projectAttr);
                importFragment.Attributes.Append(conditionAttr);
                var targetFragment = projectDoc.CreateElement("Project", projectDoc.DocumentElement.NamespaceURI);
                targetFragment.InnerXml = $"<Target Name=\"EnsureNuGetPackageBuildImports\" BeforeTargets=\"PrepareForBuild\" xmlns=\"{projectDoc.DocumentElement.NamespaceURI}\">\r\n" +
                                          "<PropertyGroup>\r\n" +
                                          "<ErrorText>This project references NuGet package(s) that are missing on this computer. Use NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=322105. The missing file is {0}.</ErrorText>\r\n" +
                                          "</PropertyGroup>\r\n" +
                                          $"<Error Condition=\"!Exists('$(USERPROFILE)\\.nuget\\packages\\synergex.synergyde.build\\{targetVersion}\\build\\rps\\Synergex.SynergyDE.Build.targets')\" Text=\"$([System.String]::Format('$(ErrorText)', '$(USERPROFILE)\\.nuget\\packages\\synergex.synergyde.build\\{targetVersion}\\build\\rps\\Synergex.SynergyDE.Build.targets'))\" />\r\n" +
                                          "</Target>";

                var importedElement = projectDoc.DocumentElement.AppendChild(importFragment);
                var targetElement = projectDoc.DocumentElement.AppendChild(targetFragment.FirstChild);
            }
        }

        private XmlNode AttributeOrChild(XmlNode node, string name)
        {
            var attribute = node.Attributes.GetNamedItem(name);
            if (attribute == null)
            {
                return node.ChildNodes.OfType<XmlNode>().FirstOrDefault(childNode => childNode.Name == name);
            }
            else
            {
                return attribute;
            }
        }

        public static bool _hasAlerted = false;

        public void PatchNugetVersions(VersionTargetingInfo versionInfo)
        {
            var packageReferences = ProjectDoc.GetElementsByTagName("PackageReference").OfType<XmlNode>().ToList();
            foreach (var node in packageReferences)
            {
                var includeValue = AttributeOrChild(node, "Include")?.Value?.Trim();
                //if we have a target version see what version we're currently at
                if (versionInfo.NugetReferences.TryGetValue(includeValue, out var targetVersion))
                {
                    var versionNode = AttributeOrChild(node, "Version");
                    var versionValue = versionNode?.InnerText;
                    if (string.IsNullOrWhiteSpace(versionValue) || targetVersion != versionValue)
                    {
                        if (includeValue.Contains("Harmony.Core"))
                        {
                            if (!_hasAlerted && !string.IsNullOrWhiteSpace(versionValue) && !versionInfo.HCRegenRequiredVersions.All((ver) => string.Compare(versionValue, ver) >= 0))
                            {
                                _hasAlerted = true;
                                Console.WriteLine("Upgrading Harmony Core to version {0} from version {1} of packages requires you to regenerate from codegen template. \r\n\r\nPlease type YES to acknowledge and continue package upgrade", versionValue, versionInfo.HCBuildVersion);
                                if (string.Compare(Console.ReadLine(), "yes", true) != 0)
                                {
                                    Console.WriteLine("exiting");
                                    Environment.Exit(1);
                                }
                            }
                        }

                        if (versionNode == null)
                        {
                            versionNode = ProjectDoc.CreateAttribute("Version");
                            node.Attributes.Append((XmlAttribute)versionNode);
                        }
                        versionNode.InnerText = targetVersion;
                        
                    }
                }
                //remove obsolete nuget packages
                else if (ObsoletePackages.Contains(includeValue))
                {
                    try
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    
                }
            }

        }

        public void Save()
        {
            var xw = XmlWriter.Create(FileName, new XmlWriterSettings {NamespaceHandling = NamespaceHandling.OmitDuplicates, OmitXmlDeclaration = true});
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            try
            {
                ProjectDoc.WriteTo(xw);
                xw.Flush();
            }
            finally
            {
                xw.Close();
            }
            
        }
    }
}