using System;
using System.Collections.Generic;
using System.IO;
using VaultRedirector.Engine;

namespace VaultRedirector.Tests
{
    public class MockFileSystem : IFileSystem
    {
        public HashSet<string> Directories { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Junctions { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
        }

        public bool DirectoryExists(string path)
        {
            return Directories.Contains(Normalize(path));
        }

        public void CreateDirectory(string path)
        {
            var normalized = Normalize(path);
            if (Directories.Contains(normalized)) return;
            Directories.Add(normalized);
        }

        public void MoveDirectory(string sourceDirName, string destDirName)
        {
            var source = Normalize(sourceDirName);
            var dest = Normalize(destDirName);

            if (!Directories.Contains(source))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {source}");
            }
            if (Directories.Contains(dest))
            {
                throw new IOException($"Destination directory already exists: {dest}");
            }

            Directories.Remove(source);
            Directories.Add(dest);
        }

        public void CreateJunction(string junctionPath, string targetPath)
        {
            var junction = Normalize(junctionPath);
            var target = Normalize(targetPath);

            if (Directories.Contains(junction))
            {
                throw new IOException($"Cannot create junction at {junction} because it already exists.");
            }

            Junctions[junction] = target;
            // A junction also acts as a directory entry
            Directories.Add(junction);
        }
    }
}
