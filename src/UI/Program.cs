using System;
using System.IO;
using VaultRedirector.Core;
using VaultRedirector.Engine;
using VaultRedirector.Core.Configuration;

namespace VaultRedirector.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFile = "crash_log.txt";
            string configPath = "rules.json";

            try
            {
                if (args.Length >= 1)
                {
                    if (args[0].Equals("add", StringComparison.OrdinalIgnoreCase))
                    {
                        ConfigCLI.AddRule(configPath);
                        return;
                    }
                    if (args[0].Equals("scan", StringComparison.OrdinalIgnoreCase))
                    {
                        ScannerCLI.RunScan(configPath);
                        return;
                    }
                }

                string? source = null;
                string? target = null;

                if (args.Length >= 2)
                {
                    source = args[0];
                    target = args[1];
                }
                else
                {
                    Console.WriteLine("Vault Redirector - Command Center");
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("No arguments provided.");
                    Console.WriteLine("Commands:");
                    Console.WriteLine("  add               - Add a new redirect rule via wizard.");
                    Console.WriteLine("  scan              - Scan for junk folders and auto-create rules.");
                    Console.WriteLine("  [source] [target] - Manually redirect a folder once.");
                    Console.WriteLine("  run-service       - Start the watcher service (Simulated in CLI for now).");
                    Console.WriteLine();
                    Console.WriteLine("Interactive Mode:");

                    Console.WriteLine("1. Manual Redirect (Enter paths)");
                    Console.WriteLine("2. Add Rule (Wizard)");
                    Console.WriteLine("3. Run Watcher Mode");
                    Console.WriteLine("4. Scan for Junk Folders");
                    Console.Write("Enter choice: ");
                    var choice = Console.ReadLine();

                    if (choice == "2")
                    {
                        ConfigCLI.AddRule(configPath);
                        return;
                    }
                    else if (choice == "3")
                    {
                        RunWatcherMode(configPath);
                        return;
                    }
                    else if (choice == "4")
                    {
                        ScannerCLI.RunScan(configPath);
                        return;
                    }

                    Console.Write("Please enter the Source path (folder to move): ");
                    source = Console.ReadLine();
                    while (string.IsNullOrWhiteSpace(source))
                    {
                        Console.WriteLine("Source path cannot be empty. Please enter Source path:");
                        source = Console.ReadLine();
                    }

                    Console.Write("Please enter the Target path (destination on Vault drive): ");
                    target = Console.ReadLine();
                    while (string.IsNullOrWhiteSpace(target))
                    {
                         Console.WriteLine("Target path cannot be empty. Please enter Target path:");
                         target = Console.ReadLine();
                    }
                }

                // Ensure non-null for logic (loops above ensure this, but compiler needs help or explicit check)
                if (source == null || target == null)
                {
                     throw new InvalidOperationException("Source or Target path is null.");
                }

                Log($"Starting redirection process. Source: {source}, Target: {target}", logFile);

                // Dependency Injection (Manual for now)
                IFileSystem fileSystem = new WindowsFileSystem();
                var redirector = new Redirector(fileSystem);

                redirector.RedirectFolder(source, target);

                Log("Success! Junction created.", logFile);
                Console.WriteLine("Success! Junction created.");
            }
            catch (Exception ex)
            {
                string errorMessage = $"CRITICAL ERROR: {ex.Message}\nStack Trace: {ex.StackTrace}";
                Log(errorMessage, logFile);
                Console.WriteLine(errorMessage);
                Console.WriteLine($"Full details written to {Path.GetFullPath(logFile)}");
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                try { Console.ReadKey(); } catch { } // ReadKey might fail in non-interactive environments
            }
        }

        static void RunWatcherMode(string configPath)
        {
            Console.WriteLine("Starting Watcher Service (Simulated)... Press Ctrl+C to stop.");
            // In a real service, this would be in Worker.cs
            // Here we just demonstrate loading config and starting sentinels.

            var configManager = new JsonConfigManager(configPath);
            var rules = configManager.LoadRulesAsync().Result;

            if (rules.Count == 0)
            {
                Console.WriteLine("No rules found. Run 'scan' or 'add' first.");
                return;
            }

            var fileSystem = new WindowsFileSystem();
            var sentinels = new System.Collections.Generic.List<VaultRedirector.Core.Sentinel.FolderSentinel>();

            foreach (var rule in rules)
            {
                if (!rule.IsEnabled) continue;

                try
                {
                    var watcher = new WindowsFileWatcher();
                    var sentinel = new VaultRedirector.Core.Sentinel.FolderSentinel(watcher, fileSystem, rule);
                    sentinel.Start(); // This starts the FileSystemWatcher
                    sentinels.Add(sentinel);
                    Console.WriteLine($"Started sentinel for {rule.AppName} ({rule.SourcePath})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start sentinel for {rule.AppName}: {ex.Message}");
                }
            }

            Console.WriteLine("Watchers active. Waiting for events...");
            // Keep alive
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        static void Log(string message, string logFile)
        {
            try
            {
                string logEntry = $"[{DateTime.Now}] {message}\n";
                File.AppendAllText(logFile, logEntry);
            }
            catch
            {
                Console.WriteLine($"[Logging Failed]: {message}");
            }
        }
    }
}
