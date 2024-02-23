using EPiServer.Framework.Web.Resources;
using EPiServer.Shell;
using SiteImprove.Optimizely.Plugin.Repositories;
using System.Collections.Generic;

namespace SiteImprove.Optimizely.Plugin.Infrastructure
{
    [ClientResourceProvider]
    public class ClientResourceProvider : IClientResourceProvider
    {
        public readonly ISettingsRepository _settingsRepository;

        public ClientResourceProvider(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public IEnumerable<ClientResource> GetClientResources()
        {
            var clientResources = new List<ClientResource>();
            var settings = this._settingsRepository.GetSetting();
            if(settings == null || !settings.LatestUI)
            {
                clientResources.Add(new ClientResource
                {
                    Name = "siteimprove.smallbox",
                    Path = "https://cdn.siteimprove.net/cms/overlay-v1.js",
                    ResourceType = ClientResourceType.Script
                });
            }
            else
            {
                clientResources.Add(new ClientResource
                {
                    Name = "siteimprove.smallbox",
                    Path = "https://cdn.siteimprove.net/cms/overlay-latest.js",
                    ResourceType = ClientResourceType.Script
                });
            }
            return clientResources.ToArray();
        }
    }
}
