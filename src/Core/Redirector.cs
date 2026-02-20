using System;
using System.IO;
using VaultRedirector.Engine;

namespace VaultRedirector.Core
{
    public class Redirector
    {
        private readonly IFileSystem _fileSystem;

        public Redirector(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void RedirectFolder(string sourcePath, string targetPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("Source path cannot be null or empty.", nameof(sourcePath));

            if (string.IsNullOrWhiteSpace(targetPath))
                throw new ArgumentException("Target path cannot be null or empty.", nameof(targetPath));

            if (!_fileSystem.DirectoryExists(sourcePath))
            {
                throw new DirectoryNotFoundException($"Source directory does not exist: {sourcePath}");
            }

            if (_fileSystem.DirectoryExists(targetPath))
            {
                throw new IOException($"Target directory already exists: {targetPath}");
            }

            // Ensure the parent directory of the target exists
            var targetParent = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(targetParent) && !_fileSystem.DirectoryExists(targetParent))
            {
                _fileSystem.CreateDirectory(targetParent);
            }

            // 1. Move source to target
            _fileSystem.MoveDirectory(sourcePath, targetPath);

            // 2. Create Junction at source pointing to target
            _fileSystem.CreateJunction(sourcePath, targetPath);
        }
    }
}
