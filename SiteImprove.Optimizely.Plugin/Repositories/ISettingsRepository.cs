using SiteImprove.Optimizely.Plugin.Models;
namespace SiteImprove.Optimizely.Plugin.Repositories
{
    public interface ISettingsRepository
    {
        string GetToken();
        void SaveToken(string token, bool noRecheck = false, string apiUser = null, string apiKey = null);
        Settings GetSetting();
    }
}
