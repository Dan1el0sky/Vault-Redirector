using System;
using System.Diagnostics;
using System.IO;

namespace VaultRedirector.Engine
{
    public class WindowsFileSystem : IFileSystem
    {
        private const string LogFile = "debug_engine.log";

        private void Log(string message)
        {
            try
            {
                File.AppendAllText(LogFile, $"[{DateTime.Now}] {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Engine Logging Failed] {ex.Message}");
            }
        }

        public bool DirectoryExists(string path)
        {
            Log($"Checking if directory exists: {path}");
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Log($"Creating directory: {path}");
            Directory.CreateDirectory(path);
        }

        public void MoveDirectory(string sourceDirName, string destDirName)
        {
            Log($"Attempting to move directory from '{sourceDirName}' to '{destDirName}'");
            try
            {
                // Attempt standard move (rename)
                Directory.Move(sourceDirName, destDirName);
                Log("Standard Directory.Move succeeded.");
            }
            catch (IOException ex)
            {
                Log($"Standard Directory.Move failed: {ex.Message}. Attempting Copy-Delete strategy.");
                // Fallback to Copy-Delete strategy for cross-volume moves or other issues
                // Note: Redirector ensures target doesn't exist, so collision shouldn't be the cause here
                CopyDirectory(sourceDirName, destDirName);
                Directory.Delete(sourceDirName, true);
                Log("Copy-Delete strategy succeeded.");
            }
            catch (Exception ex)
            {
                 Log($"MoveDirectory critical failure: {ex}");
                 throw;
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            Log($"Copying content from '{sourceDir}' to '{destinationDir}'");
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }

        public void CreateJunction(string junctionPath, string targetPath)
        {
            Log($"Creating Junction at '{junctionPath}' pointing to '{targetPath}'");
            // Windows-specific implementation using mklink
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c mklink /J \"{junctionPath}\" \"{targetPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = new Process { StartInfo = startInfo };
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    string msg = $"Failed to create junction. Exit code: {process.ExitCode}. Error: {error}";
                    Log(msg);
                    throw new IOException(msg);
                }
                Log("Junction created successfully via mklink.");
            }
            catch (Exception ex)
            {
                Log($"CreateJunction failed: {ex}");
                throw;
            }
        }
    }
}
