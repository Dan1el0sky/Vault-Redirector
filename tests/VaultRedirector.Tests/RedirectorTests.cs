using Xunit;
using VaultRedirector.Core;
using VaultRedirector.Engine;
using System.IO;

namespace VaultRedirector.Tests
{
    public class RedirectorTests
    {
        [Fact]
        public void RedirectFolder_SuccessfullyMovesAndCreatesJunction()
        {
            // Arrange
            var mockFs = new MockFileSystem();
            var source = Path.Combine("C:", "Users", "Test", "AppData", "Local", "Temp");
            var target = Path.Combine("D:", "Vault", "Temp");

            mockFs.CreateDirectory(source); // Ensure source exists

            var redirector = new Redirector(mockFs);

            // Act
            redirector.RedirectFolder(source, target);

            // Assert
            var normalizedSource = mockFs.Normalize(source);
            var normalizedTarget = mockFs.Normalize(target);

            Assert.True(mockFs.DirectoryExists(source), "Source path should still exist (as a junction)");
            Assert.True(mockFs.DirectoryExists(target), "Target directory should exist");
            Assert.True(mockFs.Junctions.ContainsKey(normalizedSource), $"Junction should be created at source. Keys: {string.Join(", ", mockFs.Junctions.Keys)}");
            Assert.Equal(normalizedTarget, mockFs.Junctions[normalizedSource]); // Verify junction target
        }

        [Fact]
        public void RedirectFolder_ThrowsIfSourceDoesNotExist()
        {
            // Arrange
            var mockFs = new MockFileSystem();
            var source = Path.Combine("C:", "NonExistent");
            var target = Path.Combine("D:", "Vault", "Target");
            var redirector = new Redirector(mockFs);

            // Act & Assert
            var ex = Assert.Throws<DirectoryNotFoundException>(() => redirector.RedirectFolder(source, target));
            Assert.Contains(source, ex.Message);
        }

        [Fact]
        public void RedirectFolder_ThrowsIfTargetAlreadyExists()
        {
            // Arrange
            var mockFs = new MockFileSystem();
            var source = Path.Combine("C:", "Source");
            var target = Path.Combine("D:", "Vault", "Target");

            mockFs.CreateDirectory(source);
            mockFs.CreateDirectory(target); // Target already exists

            var redirector = new Redirector(mockFs);

            // Act & Assert
            var ex = Assert.Throws<IOException>(() => redirector.RedirectFolder(source, target));
            Assert.Contains(target, ex.Message);
        }

        [Fact]
        public void RedirectFolder_CreatesParentOfTarget()
        {
             // Arrange
            var mockFs = new MockFileSystem();
            var source = Path.Combine("C:", "Source");
            var target = Path.Combine("D:", "Vault", "Target");
            // Parent D:/Vault does not exist

            mockFs.CreateDirectory(source);

            var redirector = new Redirector(mockFs);

            // Act
            redirector.RedirectFolder(source, target);

            // Assert
            Assert.True(mockFs.DirectoryExists(Path.Combine("D:", "Vault")), "Parent directory should be created");
            Assert.True(mockFs.DirectoryExists(target), "Target directory should exist");
        }
    }
}
