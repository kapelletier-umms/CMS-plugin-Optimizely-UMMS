using EPiServer.Core;

namespace SiteImprove.Optimizely.Plugin.Helper
{
    public interface ISiteimproveHelper
    {
        string GetOptimizelyVersion();
        string GetSiteimprovePluginVersion();
        string RequestToken();
        void PassEvent(string type, string url, string token);
        string GetExternalUrl(PageData page);
        bool GetPrepublishCheckEnabled(string apiUser, string apiKey);
        bool EnablePrepublishCheck(string apiUser, string apiKey);
    }
}