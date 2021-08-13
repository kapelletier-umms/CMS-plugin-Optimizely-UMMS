using EPiServer.Shell.Modules;
using SiteImprove.Optimizely.Plugin;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection //this namespace is used to follow convention
{
    public static class SiteimproveServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Siteimprove module to the Admin view
        /// </summary>
        public static IServiceCollection AddSiteImprove(this IServiceCollection services)
        {
            services.AddAuthorization(options => { options.AddPolicy(Constants.SiteImproveAuthorizationPolicy, p => p.RequireRole(Constants.SiteImproveAuthorizationPolicyRoles)); });

            services.Configure<ProtectedModuleOptions>(
                pm =>
                {
                    if (!pm.Items.Any(i => i.Name.Equals(Constants.SiteImproveModuleName, StringComparison.OrdinalIgnoreCase)))
                    {
                        pm.Items.Add(new ModuleDetails { Name = Constants.SiteImproveModuleName });
                    }
                });

            return services;
        }
    }
}
