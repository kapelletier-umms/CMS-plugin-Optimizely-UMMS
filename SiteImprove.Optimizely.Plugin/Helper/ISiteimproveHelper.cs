using EPiServer.Core;

namespace SiteImprove.Optimizely.Plugin.Helper
{
    public interface ISiteimproveHelper
    {
        string GetVersion();
        string RequestToken();
        void PassEvent(string type, string url, string token);
        string GetExternalUrl(PageData page);
        bool GetPrepublishCheckEnabled(string apiUser, string apiKey);
        void EnablePrepublishCheck(string apiUser, string apiKey);
    }
}