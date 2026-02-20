using System;
using System.Collections.Generic;
using System.IO;
using VaultRedirector.Core.Configuration;
using VaultRedirector.Engine;

namespace VaultRedirector.Core.Scanning
{
    public class PresetManager
    {
        private readonly IFileSystem _fileSystem;

        public PresetManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public List<RedirectRule> GetDefaultPresets()
        {
            var detected = new List<RedirectRule>();

            // 1. Windows Temp
            AddRule(detected, "Windows Temp", "%TEMP%");

            // 2. Chrome Cache
            AddRule(detected, "Chrome Cache", "%LOCALAPPDATA%\\Google\\Chrome\\User Data\\Default\\Cache");

            // 3. Spotify Cache
            AddRule(detected, "Spotify Cache", "%LOCALAPPDATA%\\Spotify\\Storage");

            // 4. Firefox Cache (Literal path logic for MVP, might need wildcards later)
            // AddRule(detected, "Firefox Cache", "%LOCALAPPDATA%\\Mozilla\\Firefox\\Profiles\\*.default-release\\cache2");

            // 5. Visual Studio Project Assemblies
            // AddRule(detected, "VS ProjectAssemblies", "%LOCALAPPDATA%\\Microsoft\\VisualStudio\\17.0*\\ProjectAssemblies");

            return detected;
        }

        private void AddRule(List<RedirectRule> rules, string appName, string sourcePath)
        {
            // Expand environment variables immediately so we can check existence
            string expandedSource = Environment.ExpandEnvironmentVariables(sourcePath);

            // For target, we default to D:\Vault\<AppName>
            string targetPath = Path.Combine("D:\\Vault", appName.Replace(" ", ""));

            rules.Add(new RedirectRule
            {
                AppName = appName,
                SourcePath = expandedSource,
                TargetPath = targetPath,
                IsEnabled = true
            });
        }

        public List<RedirectRule> ScanForExistingFolders()
        {
            var detected = new List<RedirectRule>();
            var presets = GetDefaultPresets();

            foreach (var rule in presets)
            {
                // Simple existence check using abstract file system
                if (_fileSystem.DirectoryExists(rule.SourcePath))
                {
                    detected.Add(rule);
                }
            }

            return detected;
        }
    }
}
