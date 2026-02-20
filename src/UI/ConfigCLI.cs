using System;
using System.IO;
using VaultRedirector.Core.Configuration;

namespace VaultRedirector.UI
{
    public static class ConfigCLI
    {
        public static void AddRule(string configPath)
        {
            Console.WriteLine("Adding a new redirect rule...");
            Console.Write("Enter App Name (e.g., Spotify): ");
            string appName = Console.ReadLine() ?? "Unknown";

            Console.Write("Enter Source Path (e.g., %TEMP%\\Spotify): ");
            string sourcePath = Console.ReadLine() ?? "";

            Console.Write("Enter Target Path (e.g., D:\\Vault\\Spotify): ");
            string targetPath = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine("Source and Target paths are required.");
                return;
            }

            // Expand environment variables
            sourcePath = Environment.ExpandEnvironmentVariables(sourcePath);
            targetPath = Environment.ExpandEnvironmentVariables(targetPath);

            var rule = new RedirectRule
            {
                AppName = appName,
                SourcePath = sourcePath,
                TargetPath = targetPath,
                IsEnabled = true
            };

            var configManager = new JsonConfigManager(configPath);
            // Sync over async for CLI simplicity here
            var rules = configManager.LoadRulesAsync().Result;
            rules.Add(rule);
            configManager.SaveRulesAsync(rules).Wait();

            Console.WriteLine($"Rule added successfully! Config saved to {Path.GetFullPath(configPath)}");
        }
    }
}
