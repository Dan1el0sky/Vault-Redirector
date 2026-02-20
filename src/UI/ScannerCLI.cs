using System;
using System.IO;
using System.Linq; // Missing LINQ
using System.Collections.Generic;
using VaultRedirector.Core.Configuration;
using VaultRedirector.Core.Scanning;
using VaultRedirector.Engine;

namespace VaultRedirector.UI
{
    public static class ScannerCLI
    {
        public static void RunScan(string configPath)
        {
            Console.WriteLine("Scanning for known junk folders...");
            // Use WindowsFileSystem or appropriate IFileSystem
            IFileSystem fileSystem = new WindowsFileSystem();
            var presetManager = new PresetManager(fileSystem);

            // This scans for folders that ACTUALLY exist on disk
            var detected = presetManager.ScanForExistingFolders();

            if (detected.Count == 0)
            {
                Console.WriteLine("No common junk folders found (checked Temp, Chrome Cache, Spotify).");
                return;
            }

            Console.WriteLine($"Found {detected.Count} potential folders to redirect:");
            for (int i = 0; i < detected.Count; i++)
            {
                var rule = detected[i];
                Console.WriteLine($"[{i + 1}] {rule.AppName}");
                Console.WriteLine($"    Source: {rule.SourcePath}");
                Console.WriteLine($"    Target: {rule.TargetPath}");
            }

            Console.WriteLine();
            Console.Write("Enter numbers to add (comma separated, e.g. 1,3) or 'all': ");
            string input = Console.ReadLine() ?? "";

            var configManager = new JsonConfigManager(configPath);
            var existingRules = configManager.LoadRulesAsync().Result;
            int addedCount = 0;

            if (string.Equals(input.Trim(), "all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var rule in detected)
                {
                    if (!existingRules.Any(r => string.Equals(r.SourcePath, rule.SourcePath, StringComparison.OrdinalIgnoreCase)))
                    {
                        existingRules.Add(rule);
                        addedCount++;
                    }
                }
            }
            else
            {
                var parts = input.Split(',');
                foreach (var part in parts)
                {
                    if (int.TryParse(part.Trim(), out int index) && index > 0 && index <= detected.Count)
                    {
                        var rule = detected[index - 1];
                        if (!existingRules.Any(r => string.Equals(r.SourcePath, rule.SourcePath, StringComparison.OrdinalIgnoreCase)))
                        {
                            existingRules.Add(rule);
                            addedCount++;
                        }
                    }
                }
            }

            if (addedCount > 0)
            {
                configManager.SaveRulesAsync(existingRules).Wait();
                Console.WriteLine($"Added {addedCount} rules. Config saved to {Path.GetFullPath(configPath)}");
            }
            else
            {
                Console.WriteLine("No new rules added.");
            }
        }
    }
}
