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
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: VaultRedirector.UI <source_path> <target_path>");
                    Console.WriteLine("Example: VaultRedirector.UI \"C:\\Users\\Me\\AppData\\Local\\Temp\" \"D:\\Vault\\Temp\"");
                    return;
                }

                string source = args[0];
                string target = args[1];

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
                // Also write to console for immediate visibility
                // Console.WriteLine(message); // Already doing this in Main for specific messages
            }
            catch
            {
                // If logging fails, just print to console
                Console.WriteLine($"[Logging Failed]: {message}");
            }
        }
    }
}
