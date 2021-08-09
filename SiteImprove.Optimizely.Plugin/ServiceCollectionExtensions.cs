using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace SiteImprove.Optimizely.Plugin
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Siteimprove module to the Admin view
        /// </summary>
        /// <returns></returns>
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
