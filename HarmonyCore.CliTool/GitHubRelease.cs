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
        public static async Task<ValueTuple<ZipArchive, string>> GetLatestRelease(string releasePrefix, string overrideVersionName = null, string overrideTargetUrl = null)
        {
            var client = new HttpClient();
            var octoClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Synergex"));
            var allReleases = await octoClient.Repository.Release.GetAll("Synergex", "HarmonyCore");

            var latestRelease = allReleases.OrderByDescending(rel => rel.PublishedAt).FirstOrDefault(rel => rel.Name?.StartsWith(releasePrefix) ?? false);

            var CurrentVersionTag = overrideVersionName ?? latestRelease.TagName;

            var targeturl = overrideTargetUrl ?? $"https://github.com/Synergex/HarmonyCore/archive/{CurrentVersionTag}.zip";
            var targetFile = Path.Combine(Program.AppFolder, CurrentVersionTag + ".zip");
            Stream sourceDistStream = null;
            if (!File.Exists(targetFile))
            {
                await using var httpStream = await client.GetStreamAsync(targeturl);
                await using var writer = File.Open(targetFile, FileMode.Create);
                await httpStream.CopyToAsync(writer);
                writer.Close();
            }

            sourceDistStream = File.OpenRead(targetFile);
            try
            {
                return (new ZipArchive(sourceDistStream, ZipArchiveMode.Read), CurrentVersionTag);
            }
            catch (InvalidDataException e)
            {
                //if the zip archive was corrupt, try again
                await using var httpStream = await client.GetStreamAsync(targeturl);
                await using var writer = File.Open(targetFile, FileMode.Create);
                await httpStream.CopyToAsync(httpStream);
                writer.Close();
                return (new ZipArchive(File.OpenRead(targetFile), ZipArchiveMode.Read), CurrentVersionTag);
            }
            
        }

        private static Regex NewlineNormalizer = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled);

        public static async Task<string> GetCliToolVersions(bool skipCache, string overrideVersionName = null,
            string overrideTargetUrl = null)
        {
            if (skipCache ||
                !Program.AppSettings.TryGetValue("LastReleaseCheck", out var lastChecked) || 
                !Program.AppSettings.TryGetValue("LastReleaseVersions", out var lastReleaseVersions) ||
                DateTime.Parse(lastChecked).AddDays(5) < DateTime.Now)
            {
                lastReleaseVersions = await GetCliToolVersionsInternal(overrideVersionName, overrideTargetUrl);
                Program.AppSettings["LastReleaseCheck"] = DateTime.Now.ToString();
                Program.AppSettings["LastReleaseVersions"] = lastReleaseVersions;
                return lastReleaseVersions;
            }
            else
            {
                return lastReleaseVersions;
            }
        }

        static async Task<string> GetCliToolVersionsInternal(string overrideVersionName = null,
            string overrideTargetUrl = null)
        {
            if (File.Exists("cli-tool-versions.json"))
            {
                return File.ReadAllText("cli-tool-versions.json");
            }
            else
            {
                var (zip, versionTag) = await GetLatestRelease("net8", overrideVersionName, overrideTargetUrl);
                using (zip)
                {
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.CompressedLength > 0 &&
                            entry.FullName == $"HarmonyCore-{versionTag}/cli-tool-versions.json")
                        {
                            await using var stream = entry.Open();
                            using var reader = new StreamReader(stream);
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            return String.Empty;
        }

        public static async Task GetAndUnpackLatest(bool hasTraditionalBridge, string traditionalBridgeFolder, List<string> distinctTemplateFolders, SolutionInfo solution,
            string overrideVersionName = null, string overrideTargetUrl = null, string zipPath = null)
        {
            var (zip, CurrentVersionTag) = new ValueTuple<ZipArchive, string>();
            if (zipPath != null)
            {
                try
                {
                    var sourceDistStream = File.OpenRead(zipPath);
                    zip = new ZipArchive(sourceDistStream, ZipArchiveMode.Read);
                    CurrentVersionTag = "";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                (zip, CurrentVersionTag) = await GetLatestRelease("net8", overrideVersionName, overrideTargetUrl);
            }
            using (zip)
            {
                foreach (var entry in zip.Entries)
                {
                    var rootFolderName = "HarmonyCore-" + CurrentVersionTag + "/";
                    if (zipPath != null)
                    {
                        rootFolderName = "";
                    }
                    if (entry.CompressedLength > 0 && entry.FullName.StartsWith($"{rootFolderName}Templates/"))
                    {
                        if (distinctTemplateFolders.Count > 0)
                        {
                            var targetFileName = Path.Combine(distinctTemplateFolders.First(), entry.FullName.Replace($"{rootFolderName}Templates/", "", StringComparison.CurrentCultureIgnoreCase).Replace("/", "\\").Replace("\\\\", "\\"));

                            if (targetFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                                continue;

                            Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                            if (File.Exists(targetFileName))
                                File.Delete(targetFileName);

                            using (var stream = entry.Open())
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    await File.WriteAllTextAsync(targetFileName, NewlineNormalizer.Replace(reader.ReadToEnd(), "\r\n"));
                                }
                            }
                        }
                    }
                    else if (entry.CompressedLength > 0 && hasTraditionalBridge && entry.FullName.StartsWith($"{rootFolderName}TraditionalBridge/") && entry.FullName.EndsWith(".dbl"))
                    {
                        var targetFileName = "";
                        if (zipPath != null)
                        {
                            targetFileName = Path.Combine(traditionalBridgeFolder, Path.GetFileName(entry.FullName));
                        }
                        else
                        {
                            targetFileName = Path.Combine(traditionalBridgeFolder, Path.GetFileName(entry.FullName.Replace($"{rootFolderName}", "", StringComparison.CurrentCultureIgnoreCase).Replace("/", "\\").Replace("\\\\", "\\"))); ;
                        }

                        if (File.Exists(targetFileName))
                            File.Delete(targetFileName);

                        using (var stream = entry.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                await File.WriteAllTextAsync(targetFileName, NewlineNormalizer.Replace(reader.ReadToEnd(), "\r\n"));
                            }
                        }
                    }
                    else if (entry.CompressedLength > 0 && entry.FullName == $"{rootFolderName}regen.bat")
                    {
                        var targetFileName = Path.Combine(solution.SolutionDir, "regen.bat.example");
                        if (File.Exists(targetFileName))
                            File.Delete(targetFileName);

                        using (var stream = entry.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                await File.WriteAllTextAsync(targetFileName, NewlineNormalizer.Replace(await reader.ReadToEndAsync(), "\r\n"));
                            }
                        }
                    }

                    else if (entry.CompressedLength > 0 && entry.FullName == $"{rootFolderName}UserDefinedTokens.tkn")
                    {
                        var targetFileName = Path.Combine(solution.SolutionDir, "UserDefinedTokens.tkn.example");
                        if (File.Exists(targetFileName))
                            File.Delete(targetFileName);

                        using (var stream = entry.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                await File.WriteAllTextAsync(targetFileName, NewlineNormalizer.Replace(await reader.ReadToEndAsync(), "\r\n"));
                            }
                        }
                    }
                }
            }
        }
    }
}