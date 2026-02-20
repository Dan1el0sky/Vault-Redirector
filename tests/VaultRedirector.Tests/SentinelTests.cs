using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using VaultRedirector.Core.Configuration;
using VaultRedirector.Core.Sentinel;
using VaultRedirector.Engine;

namespace VaultRedirector.Tests
{
    public class MockFileWatcher : IFileWatcher
    {
        public bool IsWatching { get; private set; }
        public string WatchedPath { get; private set; } = string.Empty;
        public string Filter { get; private set; } = string.Empty;
        private Action<string>? _onCreated;

        public void Start(string path, string filter, Action<string> onCreated)
        {
            IsWatching = true;
            WatchedPath = path;
            Filter = filter;
            _onCreated = onCreated;
        }

        public void TriggerCreate(string fullPath)
        {
            _onCreated?.Invoke(fullPath);
        }

        public void Stop()
        {
            IsWatching = false;
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public class SentinelTests
    {
        [Fact]
        public void Start_ShouldStartWatcher_WhenParentDirExists()
        {
            // Arrange
            var mockFs = new MockFileSystem();
            var mockWatcher = new MockFileWatcher();
            var source = Path.Combine("C:", "Temp", "TestApp");
            var target = Path.Combine("D:", "Vault", "TestApp");
            var rule = new RedirectRule
            {
                AppName = "TestApp",
                SourcePath = source,
                TargetPath = target
            };

            // Setup: Parent of Source must exist for watcher to start
            mockFs.CreateDirectory(Path.Combine("C:", "Temp"));

            var sentinel = new FolderSentinel(mockWatcher, mockFs, rule);

            // Act
            sentinel.Start();

            // Assert
            Assert.True(mockWatcher.IsWatching);
            Assert.Equal(mockFs.Normalize(Path.Combine("C:", "Temp")), mockFs.Normalize(mockWatcher.WatchedPath));
        }

        [Fact]
        public async Task TriggerCreate_ShouldCallRedirector()
        {
            // Arrange
            var mockFs = new MockFileSystem();
            var mockWatcher = new MockFileWatcher();
            var source = Path.Combine("C:", "Temp", "TestApp");
            var target = Path.Combine("D:", "Vault", "TestApp");
            var rule = new RedirectRule
            {
                AppName = "TestApp",
                SourcePath = source,
                TargetPath = target
            };

            mockFs.CreateDirectory(Path.Combine("C:", "Temp"));
            var sentinel = new FolderSentinel(mockWatcher, mockFs, rule);
            sentinel.Start();

            // Simulate the app creating the folder
            // In a real scenario, the app creates the folder.
            // Our test trigger simulates the watcher event.
            // However, Redirector.RedirectFolder expects the source to exist before it can move/junction it.
            mockFs.CreateDirectory(source);

            // Act
            mockWatcher.TriggerCreate(source);

            // Wait for async task in Sentinel (it has a delay of 500ms)
            await Task.Delay(1000);

            // Assert
            // The folder should now be a junction pointing to TargetPath
            var normalizedSource = mockFs.Normalize(source);
            var normalizedTarget = mockFs.Normalize(target);

            Assert.True(mockFs.Junctions.ContainsKey(normalizedSource), $"Junction should be created at {normalizedSource}. Existing: {string.Join(",", mockFs.Junctions.Keys)}");
            Assert.Equal(normalizedTarget, mockFs.Junctions[normalizedSource]);
        }
    }
}
