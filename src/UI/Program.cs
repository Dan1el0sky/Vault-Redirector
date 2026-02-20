using System;
using VaultRedirector.Core;
using VaultRedirector.Engine;

namespace VaultRedirector.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: VaultRedirector.UI <source_path> <target_path>");
                Console.WriteLine("Example: VaultRedirector.UI \"C:\\Users\\Me\\AppData\\Local\\Temp\" \"D:\\Vault\\Temp\"");
                return;
            }

            string source = args[0];
            string target = args[1];

            Console.WriteLine($"Redirecting '{source}' to '{target}'...");

            try
            {
                // Dependency Injection (Manual for now)
                IFileSystem fileSystem = new WindowsFileSystem();
                var redirector = new Redirector(fileSystem);

                redirector.RedirectFolder(source, target);

                Console.WriteLine("Success! Junction created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
