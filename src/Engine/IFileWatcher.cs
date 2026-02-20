using System;

namespace VaultRedirector.Engine
{
    public interface IFileWatcher : IDisposable
    {
        void Start(string path, string filter, Action<string> onCreated);
        void Stop();
    }
}
