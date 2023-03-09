using System.Collections.Generic;

namespace SiteImprove.Optimizely.Plugin.Models
{
    public class SettingsViewModel
    {
        public string Token { get; set; }

        public bool Recheck { get; set; }

        public string ApiUser { get; set; }

        public string ApiKey { get; set; }

        public bool PrepublishCheckEnabled { get; set; }

        public bool PrepublishError { get; set; }

        public IDictionary<string, string> UrlMap { get; set; }

        public string PluginVersion { get; set; }

        public string PluginUrl { get; set; }
    }
}