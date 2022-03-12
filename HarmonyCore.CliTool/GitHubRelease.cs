using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace HarmonyCore.CliTool
{
    public class GitHubRelease
    {
        public static async Task GetAndUnpackLatest(bool hasTraditionalBridge, string traditionalBridgeFolder, List<string> distinctTemplateFolders, SolutionInfo solution,
            string overrideVersionName = null, string overrideTargetUrl = null)
        {
            var client = new HttpClient();
            var octoClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Synergex"));
            var latestRelease = await octoClient.Repository.Release.GetLatest("Synergex", "HarmonyCore");
            var CurrentVersionTag = overrideVersionName ?? latestRelease.TagName;

            var targeturl = overrideTargetUrl ?? $"https://github.com/Synergex/HarmonyCore/archive/{CurrentVersionTag}.zip";
            var sourceDistStream = targeturl.StartsWith("https", StringComparison.OrdinalIgnoreCase) ? await client.GetStreamAsync(targeturl) : File.OpenRead(targeturl);
            var normalizer = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled);
            using (var zip = new ZipArchive(sourceDistStream, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.CompressedLength > 0 && entry.FullName.StartsWith($"HarmonyCore-{CurrentVersionTag}/Templates/"))
                    {
                        if (distinctTemplateFolders.Count > 0)
                        {
                            var targetFileName = Path.Combine(distinctTemplateFolders.First(), entry.FullName.Replace($"HarmonyCore-{CurrentVersionTag}/Templates/", "", StringComparison.CurrentCultureIgnoreCase).Replace("/", "\\").Replace("\\\\", "\\"));

                            if (targetFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                                continue;

                            Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                            if (File.Exists(targetFileName))
                                File.Delete(targetFileName);

                            using (var stream = entry.Open())
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    File.WriteAllText(targetFileName, normalizer.Replace(reader.ReadToEnd(), "\r\n"));
                                }
                            }
                        }
                    }
                    else if (entry.CompressedLength > 0 && hasTraditionalBridge && entry.FullName.StartsWith($"HarmonyCore-{CurrentVersionTag}/TraditionalBridge/") && entry.FullName.EndsWith(".dbl"))
                    {
                        var targetFileName = Path.Combine(traditionalBridgeFolder, Path.GetFileName(entry.FullName.Replace($"HarmonyCore-{CurrentVersionTag}", "", StringComparison.CurrentCultureIgnoreCase).Replace("/", "\\").Replace("\\\\", "\\")));
                        if (File.Exists(targetFileName))
                            File.Delete(targetFileName);

                        using (var stream = entry.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                File.WriteAllText(targetFileName, normalizer.Replace(reader.ReadToEnd(), "\r\n"));
                            }
                        }
                    }
                    else if (entry.CompressedLength > 0 && entry.FullName == $"HarmonyCore-{CurrentVersionTag}/regen.bat")
                    {
                        var targetFileName = Path.Combine(solution.SolutionDir, "regen.bat.example");
                        if (File.Exists(targetFileName))
                            File.Delete(targetFileName);

                        using (var stream = entry.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                File.WriteAllText(targetFileName, normalizer.Replace(reader.ReadToEnd(), "\r\n"));
                            }
                        }
                    }

                    else if (entry.CompressedLength > 0 && entry.FullName == $"HarmonyCore-{CurrentVersionTag}/UserDefinedTokens.tkn")
                    {
                        var targetFileName = Path.Combine(solution.SolutionDir, "UserDefinedTokens.tkn.example");
                        if (File.Exists(targetFileName))
                            File.Delete(targetFileName);

                        using (var stream = entry.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                File.WriteAllText(targetFileName, normalizer.Replace(reader.ReadToEnd(), "\r\n"));
                            }
                        }
                    }
                }
            }
        }
    }
}