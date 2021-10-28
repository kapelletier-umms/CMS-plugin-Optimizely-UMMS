namespace SiteImprove.Optimizely.Plugin.Models
{
    public class SettingsViewModel
    {
        public string Token { get; set; }
        
        public bool Recheck { get; set; }

        public string ApiUser { get; set; }

        public string ApiKey { get; set; }

        public bool PrepublishCheckEnabled { get; set; }
    }
}
