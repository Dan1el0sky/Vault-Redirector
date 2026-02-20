using System;
using System.IO;
using System.Threading.Tasks;
using VaultRedirector.Engine;
using VaultRedirector.Core.Configuration;

namespace VaultRedirector.Core.Sentinel
{
    public class FolderSentinel : IDisposable
    {
        private readonly IFileWatcher _watcher;
        private readonly RedirectRule _rule;
        private readonly Redirector _redirector;
        private readonly IFileSystem _fileSystem;

        public FolderSentinel(IFileWatcher watcher, IFileSystem fileSystem, RedirectRule rule)
        {
            _watcher = watcher;
            _fileSystem = fileSystem;
            _rule = rule;
            _redirector = new Redirector(_fileSystem);
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(_rule.SourcePath))
            {
                 Console.WriteLine("[Sentinel] Source path is empty. Skipping.");
                 return;
            }

            // We watch the parent directory for the creation of the target folder.
            string? parentDir = Path.GetDirectoryName(_rule.SourcePath);
            string folderName = Path.GetFileName(_rule.SourcePath);

            if (string.IsNullOrEmpty(parentDir))
            {
                 // Root drive? Rare case.
                 Console.WriteLine($"[Sentinel] Cannot watch root drive.");
                 return;
            }

             if (!_fileSystem.DirectoryExists(parentDir))
            {
                Console.WriteLine($"[Sentinel] Parent path not found: {parentDir}. Skipping.");
                // In a real app, we might poll for parent directory creation.
                return;
            }

            Console.WriteLine($"[Sentinel] Watching {parentDir} for creation of {folderName}...");
            _watcher.Start(parentDir, folderName, OnFolderCreated);
        }

        private void OnFolderCreated(string fullPath)
        {
            // The app just created the folder we want to redirect!
            Console.WriteLine($"[Sentinel] Detected creation of {fullPath}!");

            Task.Run(async () =>
            {
                // Exponential backoff logic would go here.
                // For MVP, wait 500ms and try.
                await Task.Delay(500);
                try
                {
                    _redirector.RedirectFolder(fullPath, _rule.TargetPath);
                    Console.WriteLine($"[Sentinel] Successfully redirected {fullPath} to {_rule.TargetPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Sentinel] Failed to redirect {fullPath}: {ex.Message}");
                }
            });
        }

        public void Stop()
        {
            _watcher.Stop();
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
