using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace SiteImprove.Optimizely.Plugin.Models
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class Settings
    {
        public Identity Id { get; set; }

        public string Token { get; set; }

        public bool Recheck { get; set; }

        public string ApiUser { get; set; }

        public string ApiKey { get; set; }
    }
}
