using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace VaultRedirector.Core.Configuration
{
    public interface IConfigManager
    {
        Task<List<RedirectRule>> LoadRulesAsync();
        Task SaveRulesAsync(List<RedirectRule> rules);
    }

    public class JsonConfigManager : IConfigManager
    {
        private readonly string _configPath;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonConfigManager(string configPath = "rules.json")
        {
            _configPath = configPath;
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        }

        public async Task<List<RedirectRule>> LoadRulesAsync()
        {
            if (!File.Exists(_configPath))
            {
                return new List<RedirectRule>();
            }

            try
            {
                string json = await File.ReadAllTextAsync(_configPath);
                var rules = JsonSerializer.Deserialize<List<RedirectRule>>(json);
                return rules ?? new List<RedirectRule>();
            }
            catch
            {
                // In a real app, log this error. For now, return empty.
                return new List<RedirectRule>();
            }
        }

        public async Task SaveRulesAsync(List<RedirectRule> rules)
        {
            string json = JsonSerializer.Serialize(rules, _jsonOptions);
            await File.WriteAllTextAsync(_configPath, json);
        }
    }
}
