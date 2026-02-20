namespace VaultRedirector.Core.Configuration
{
    public class RedirectRule
    {
        public string AppName { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
    }
}
