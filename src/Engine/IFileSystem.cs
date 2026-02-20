namespace VaultRedirector.Engine
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        void MoveDirectory(string sourceDirName, string destDirName);
        void CreateJunction(string junctionPath, string targetPath);
    }
}
