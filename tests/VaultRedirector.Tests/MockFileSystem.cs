using System;
using System.Collections.Generic;
using System.IO;
using VaultRedirector.Engine;

namespace VaultRedirector.Tests
{
    public class MockFileSystem : IFileSystem
    {
        public HashSet<string> Directories { get; } = new HashSet<string>();
        public Dictionary<string, string> Junctions { get; } = new Dictionary<string, string>(); // JunctionPath -> TargetPath

        public bool DirectoryExists(string path)
        {
            return Directories.Contains(path);
        }

        public void CreateDirectory(string path)
        {
            if (Directories.Contains(path)) return;
            Directories.Add(path);
        }

        public void MoveDirectory(string sourceDirName, string destDirName)
        {
            if (!Directories.Contains(sourceDirName))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirName}");
            }
            if (Directories.Contains(destDirName))
            {
                throw new IOException($"Destination directory already exists: {destDirName}");
            }

            Directories.Remove(sourceDirName);
            Directories.Add(destDirName);
        }

        public void CreateJunction(string junctionPath, string targetPath)
        {
            if (Directories.Contains(junctionPath))
            {
                throw new IOException($"Cannot create junction at {junctionPath} because it already exists.");
            }

            Junctions[junctionPath] = targetPath;
            // A junction also acts as a directory entry
            Directories.Add(junctionPath);
        }
    }
}
