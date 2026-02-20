using System;
using System.IO;

namespace VaultRedirector.Engine
{
    public class WindowsFileWatcher : IFileWatcher
    {
        private FileSystemWatcher? _watcher;
        private Action<string>? _onCreated;

        public void Start(string path, string filter, Action<string> onCreated)
        {
            if (!Directory.Exists(path))
            {
                // If directory doesn't exist, we can't watch it yet.
                throw new DirectoryNotFoundException($"Cannot watch non-existent directory: {path}");
            }

            _onCreated = onCreated;
            _watcher = new FileSystemWatcher(path);
            _watcher.Filter = filter;
            _watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName; // Watch for new directories/files
            _watcher.IncludeSubdirectories = false; // Usually we watch the root Junk folder

            _watcher.Created += OnCreated;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _onCreated?.Invoke(e.FullPath);
        }

        public void Stop()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Created -= OnCreated;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
