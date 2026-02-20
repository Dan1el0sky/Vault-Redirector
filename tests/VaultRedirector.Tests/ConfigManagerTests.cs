using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using VaultRedirector.Core.Configuration;

namespace VaultRedirector.Tests
{
    public class ConfigManagerTests : IDisposable
    {
        private const string TestConfigPath = "test_rules.json";

        public ConfigManagerTests()
        {
            if (File.Exists(TestConfigPath))
            {
                File.Delete(TestConfigPath);
            }
        }

        public void Dispose()
        {
            if (File.Exists(TestConfigPath))
            {
                File.Delete(TestConfigPath);
            }
        }

        [Fact]
        public async Task LoadRules_ReturnsEmptyList_WhenFileDoesNotExist()
        {
            var manager = new JsonConfigManager(TestConfigPath);
            var rules = await manager.LoadRulesAsync();
            Assert.Empty(rules);
        }

        [Fact]
        public async Task SaveAndLoad_RoundTrip_Success()
        {
            var manager = new JsonConfigManager(TestConfigPath);
            var expectedRules = new List<RedirectRule>
            {
                new RedirectRule { AppName = "Chrome", SourcePath = "C:/Temp/Chrome", TargetPath = "D:/Vault/Chrome" },
                new RedirectRule { AppName = "Spotify", SourcePath = "C:/Spotify/Cache", TargetPath = "D:/Vault/Spotify" }
            };

            await manager.SaveRulesAsync(expectedRules);
            var loadedRules = await manager.LoadRulesAsync();

            Assert.Equal(2, loadedRules.Count);
            Assert.Equal("Chrome", loadedRules[0].AppName);
            Assert.Equal("Spotify", loadedRules[1].AppName);
        }
    }
}
