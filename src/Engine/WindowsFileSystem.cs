using System;
using System.Diagnostics;
using System.IO;

namespace VaultRedirector.Engine
{
    public class WindowsFileSystem : IFileSystem
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void MoveDirectory(string sourceDirName, string destDirName)
        {
            try
            {
                // Attempt standard move (rename)
                Directory.Move(sourceDirName, destDirName);
            }
            catch (IOException)
            {
                // Fallback to Copy-Delete strategy for cross-volume moves or other issues
                // Note: Redirector ensures target doesn't exist, so collision shouldn't be the cause here
                CopyDirectory(sourceDirName, destDirName);
                Directory.Delete(sourceDirName, true);
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
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

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new IOException($"Failed to create junction. Exit code: {process.ExitCode}. Error: {error}");
            }
        }
    }
}
