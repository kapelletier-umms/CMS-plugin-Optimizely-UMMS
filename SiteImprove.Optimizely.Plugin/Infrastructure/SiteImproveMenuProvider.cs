using EPiServer.Framework.Modules;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Navigation;
using System.Collections.Generic;

namespace SiteImprove.Optimizely.Plugin.Infrastructure
{
    [MenuProvider]
    public class SiteImproveMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            string url = ServiceLocator.Current.GetInstance<IModuleResourceResolver>().ResolvePath(Constants.SiteImproveModuleName, "SiteimproveAdmin");
            UrlMenuItem urlMenuItem = new UrlMenuItem("SiteImprove", MenuPaths.Global + "/cms/admin/siteimprove", url)
            {
                SortIndex = SortIndex.Late,
                Alignment = MenuItemAlignment.Left,
                AuthorizationPolicy = Constants.SiteImproveAuthorizationPolicy
            };

            return new List<MenuItem>
            {
                urlMenuItem
            };
        }
    }
}
