using System;
using System.IO;
using VaultRedirector.Core;
using VaultRedirector.Engine;

namespace VaultRedirector.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFile = "crash_log.txt";
            try
            {
                string? source = null;
                string? target = null;

                if (args.Length >= 2)
                {
                    source = args[0];
                    target = args[1];
                }
                else
                {
                    Console.WriteLine("No arguments provided. Running in interactive mode.");
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
