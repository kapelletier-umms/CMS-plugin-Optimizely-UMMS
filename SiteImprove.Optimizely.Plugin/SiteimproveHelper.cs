using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Newtonsoft.Json;

namespace SiteImprove.Optimizely.Plugin
{
    [ServiceConfiguration(typeof(ISiteimproveHelper))]
    public class SiteimproveHelper : ISiteimproveHelper
    {
        private static readonly ILogger _log = LogManager.GetLogger(typeof(SiteimproveHelper));

        public string GetVersion()
        {
            var optimizelyAssembly = Assembly.GetAssembly(typeof(EPiServer.Core.Licensing));
            var version = optimizelyAssembly.GetName().Version;
            return version.ToString();
        }

        public string GetExternalUrl(PageData page)
        {
            try
            {
                var internalUrl = ServiceLocator.Current.GetInstance<IUrlResolver>().GetUrl(page.ContentLink);

                if (internalUrl != null) //can be null for special pages like settings 
                {
                    var siteUrl = SiteDefinition.Current.SiteUrl;
                    var uriBuilder = new UrlBuilder(internalUrl)
                    {
                        Host = siteUrl.Host,
                        Port = siteUrl.Port,
                        Scheme = siteUrl.Scheme
                    };
                    return uriBuilder.Uri.ToString();
                }

                return null;
            }
            catch (Exception ex)
            {
                _log.Error("Could not resolve pageUrl. Perhaps SiteDefinition.Current cannot be resolved? Scheduled jobs requires a * binding to handle SiteDefinition.Current", ex);
                return null;
            }
        }

        public bool GetPrepublishCheckEnabled(string apiUser, string apiKey)
        {
            using (var client = new HttpClient())
            {
                bool enabled = false;

                var byteArray = Encoding.ASCII.GetBytes($"{apiUser}:{apiKey}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = client.GetAsync($"{Constants.SiteImproveApiUrl}/settings/content_checking").Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    enabled = JsonConvert.DeserializeObject<dynamic>(content)["is_ready"];
                }
                catch (Exception ex)
                {
                    _log.Error("Could not get prepublish check status.", ex);
                }

                return enabled;
            }
        }

        public void EnablePrepublishCheck(string apiUser, string apiKey)
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{apiUser}:{apiKey}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = client.PostAsync($"{Constants.SiteImproveApiUrl}/settings/content_checking", null).Result;
                }
                catch (Exception ex)
                {
                    _log.Error("Could not enable prepublish check.", ex);
                }
            }
        }

        public string RequestToken()
        {
            var response = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    // Request a token from Siteimprove
                    var version = GetVersion();
                    string data = client.GetStringAsync(string.Format("{0}?cms=Optimizely {1}", Constants.SiteImproveTokenUrl, version)).Result;
                    response = JsonConvert.DeserializeObject<dynamic>(data)["token"];
                }
            }
            catch (Exception ex)
            {
                _log.Error("An error occured requesting token. perhaps its a wrong version - a version is required for generating a token ", ex);
            }
            return response;
        }

        public void PassEvent(string type, string url, string token)
        {
            var data = new { url, type, token };
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    var response = client.PostAsync(Constants.SiteImproveRecheckUrl, content).Result;
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("An error occured requesting {0}. perhaps token is missing? - A token are required for this operation ", type), ex);
            }
        }
    }
}
