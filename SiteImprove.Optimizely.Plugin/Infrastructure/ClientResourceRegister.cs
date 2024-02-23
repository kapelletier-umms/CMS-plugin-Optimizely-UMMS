//using EPiServer.Framework.Web.Resources;
//using SiteImprove.Optimizely.Plugin.Repositories;
//using System.Collections.Generic;

//namespace SiteImprove.Optimizely.Plugin.Infrastructure
//{
//    [ClientResourceRegistrator]
//    public class ClientResourceRegister : IClientResourceRegistrator
//    {
//        public readonly ISettingsRepository _settingsRepository;

//        public ClientResourceRegister(ISettingsRepository settingsRepository)
//        {
//            _settingsRepository = settingsRepository;
//        }

//        public void RegisterResources(IRequiredClientResourceList requiredResources)
//        {
//            requiredResources.Require("styles").AtHeader();
//            requiredResources.Require("siteimprove.base").AtHeader();

//            var settings = _settingsRepository.GetSetting();
//            if (settings.LatestUI)
//            {
//                requiredResources.RequireScript("https://cdn.siteimprove.net/cms/overlay-latest.js").AtHeader();
//            }
//            else
//            {
//                requiredResources.RequireScript("https://cdn.siteimprove.net/cms/overlay-v1.js").AtHeader();
//            }
//        }
//    }
//}
