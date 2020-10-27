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

        private static Dictionary<string, string> TargetFramework = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"Services", "netcoreapp3.1"},
            {"Services.Test", "netcoreapp3.1"},
            {"Services.Host", "netcoreapp3.1"},
            {"Services.Models", "netcoreapp3.1"},
            {"Services.Controllers", "netcoreapp3.1"},
            {"Services.Isolated", "netcoreapp3.1"},
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
            "Services.Test"
        };
        public string FileName { get; set; }
        public XmlDocument ProjectDoc { get; set; }

        public ProjectInfo(string path)
        {
            FileName = path;
            ProjectDoc = new XmlDocument { PreserveWhitespace = true };
            ProjectDoc.Load(path);
        }

        public void PatchKnownIssues()
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
            
            
            //look for bad explicit nuget pathing
            //switch to this style of rps target
            //<Import Condition="Exists('$(MSBuildExtensionsPath)\Synergex\dbl\Synergex.SynergyDE.Repository.targets')" Project="$(MSBuildExtensionsPath)\Synergex\dbl\Synergex.SynergyDE.Repository.targets" />
            //<Import Condition="!Exists('$(MSBuildExtensionsPath)\Synergex\dbl\Synergex.SynergyDE.Repository.targets')" Project="$(MSBuildProgramFiles32)\MSBuild\Synergex\dbl\Synergex.SynergyDE.Repository.targets" />
            if (string.Compare(cleanFileName, "Repository", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var hasRepositoryTargets = false;
                var imports = ProjectDoc.GetElementsByTagName("Import").OfType<XmlNode>().ToList();
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

                var targets = ProjectDoc.GetElementsByTagName("Target").OfType<XmlNode>().ToList();
                foreach (var target in targets)
                {
                    var targetNameAttr = target.Attributes["Name"];
                    if (string.Compare(targetNameAttr?.Value, "EnsureNuGetPackageBuildImports",
                        StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        target.ParentNode.RemoveChild(target);
                    }
                }

                var importFragment = ProjectDoc.CreateElement("Import", ProjectDoc.DocumentElement.NamespaceURI);
                var projectAttr = ProjectDoc.CreateAttribute("Project");
                projectAttr.Value = $"$(USERPROFILE)\\.nuget\\packages\\synergex.synergyde.build\\{Program.BuildPackageVersion}\\build\\rps\\Synergex.SynergyDE.Build.targets";
                var conditionAttr = ProjectDoc.CreateAttribute("Condition");
                conditionAttr.Value = $"Exists('$(USERPROFILE)\\.nuget\\packages\\synergex.synergyde.build\\{Program.BuildPackageVersion}\\build\\rps\\Synergex.SynergyDE.Build.targets')";
                importFragment.Attributes.Append(projectAttr);
                importFragment.Attributes.Append(conditionAttr);
                var targetFragment = ProjectDoc.CreateElement("Project", ProjectDoc.DocumentElement.NamespaceURI);
                targetFragment.InnerXml = $"<Target Name=\"EnsureNuGetPackageBuildImports\" BeforeTargets=\"PrepareForBuild\" xmlns=\"{ProjectDoc.DocumentElement.NamespaceURI}\">\r\n" +
                                          "<PropertyGroup>\r\n" +
                                          "<ErrorText>This project references NuGet package(s) that are missing on this computer. Use NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=322105. The missing file is {0}.</ErrorText>\r\n" +
                                          "</PropertyGroup>\r\n" +
                                          $"<Error Condition=\"!Exists('$(USERPROFILE)\\.nuget\\packages\\synergex.synergyde.build\\{Program.BuildPackageVersion}\\build\\rps\\Synergex.SynergyDE.Build.targets')\" Text=\"$([System.String]::Format('$(ErrorText)', '$(USERPROFILE)\\.nuget\\packages\\synergex.synergyde.build\\{Program.BuildPackageVersion}\\build\\rps\\Synergex.SynergyDE.Build.targets'))\" />\r\n" +
                                          "</Target>";
                
                var importedElement = ProjectDoc.DocumentElement.AppendChild(importFragment);
                var targetElement = ProjectDoc.DocumentElement.AppendChild(targetFragment.FirstChild);
            }
            
            
            //switch .net standard 2.0 projects to netcoreapp3.1
            var targetFramework = ProjectDoc.GetElementsByTagName("TargetFramework").OfType<XmlNode>().FirstOrDefault();
            
            if (TargetFramework.TryGetValue(Path.GetFileNameWithoutExtension(FileName), out var newTargetFramework))
            {
                targetFramework.InnerText = newTargetFramework;
            }

            var firstItemGroup = ProjectDoc.GetElementsByTagName("ItemGroup").OfType<XmlNode>().FirstOrDefault();
            if (firstItemGroup == null)
            {
                firstItemGroup = ProjectDoc.CreateElement("ItemGroup");
                ProjectDoc.DocumentElement.AppendChild(firstItemGroup);
            }

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
                if (!hasOdataVersioning)
                {
                    var versioningReference = ProjectDoc.CreateElement("PackageReference");
                    var versioningReferenceName = ProjectDoc.CreateAttribute("Include");
                    versioningReferenceName.Value = "Microsoft.AspNetCore.OData.Versioning.ApiExplorer";
                    var versioningReferenceVersion = ProjectDoc.CreateAttribute("Version");
                    versioningReferenceVersion.Value = Program.LatestNugetReferences["Microsoft.AspNetCore.OData.Versioning.ApiExplorer"];
                    versioningReference.Attributes.Append(versioningReferenceName);
                    versioningReference.Attributes.Append(versioningReferenceVersion);
                    firstItemGroup.AppendChild(versioningReference);
                }
            }

            if (CodeDomProjects.Contains(cleanFileName))
            {
                var hasCodeDomInjector = ProjectDoc.GetElementsByTagName("PackageReference").OfType<XmlNode>()
                .Any(node => node.Attributes["Include"]?.Value == "HarmonyCore.CodeDomProvider");
                if (!hasCodeDomInjector)
                {
                    var codeDomReference = ProjectDoc.CreateElement("PackageReference");
                    var codeDomReferenceName = ProjectDoc.CreateAttribute("Include");
                    codeDomReferenceName.Value = "HarmonyCore.CodeDomProvider";
                    var codeDomReferenceVersion = ProjectDoc.CreateAttribute("Version");
                    codeDomReferenceVersion.Value = Program.CodeDomProviderVersion;
                    codeDomReference.Attributes.Append(codeDomReferenceName);
                    codeDomReference.Attributes.Append(codeDomReferenceVersion);
                    firstItemGroup.AppendChild(codeDomReference);
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
                    fRefVersion.Value = Program.LatestNugetReferences["Microsoft.AspNetCore.Mvc.NewtonsoftJson"];
                    fRef.Attributes.Append(fRefName);
                    fRef.Attributes.Append(fRefVersion);
                    firstItemGroup.AppendChild(fRef);
                }
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

        public void PatchNugetVersions(Dictionary<string, string> packageToVersionMapping)
        {
            var packageReferences = ProjectDoc.GetElementsByTagName("PackageReference").OfType<XmlNode>().ToList();
            foreach (var node in packageReferences)
            {
                var includeValue = AttributeOrChild(node, "Include")?.Value;
                //if we have a target version see what version we're currently at
                if (packageToVersionMapping.TryGetValue(includeValue, out var targetVersion))
                {
                    var versionNode = AttributeOrChild(node, "Version");
                    var versionValue = versionNode?.InnerText;
                    if (string.IsNullOrWhiteSpace(versionValue) || targetVersion != versionValue)
                    {
                        if (includeValue.Contains("Harmony.Core"))
                        {
                            if (!_hasAlerted && !string.IsNullOrWhiteSpace(versionValue) && !Program.HCRegenRequiredVersions.All((ver) => string.Compare(versionValue, ver) >= 0))
                            {
                                _hasAlerted = true;
                                Console.WriteLine("Upgrading Harmony Core to version {0} from version {1} of packages requires you to regenerate from codegen template. \r\n\r\nPlease type YES to acknowledge and continue package upgrade", versionValue, Program.HCBuildVersion);
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