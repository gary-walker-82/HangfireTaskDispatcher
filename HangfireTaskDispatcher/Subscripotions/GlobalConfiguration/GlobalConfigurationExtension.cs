using System;
using System.Reflection;
using BLS.EmailSubscriptions.HangfireExtension.Pages;
using Hangfire;
using Hangfire.Dashboard;

namespace BLS.EmailSubscriptions.HangfireExtension.GlobalConfiguration
{
    public static class GlobalConfigurationExtension
    {

        public static IGlobalConfiguration UseEmailSubscriptionUi(this IGlobalConfiguration config, EmailSubscriptionOptions options = null)
        {
//            options = options ?? new EmailSubscriptionOptions();
            CreateTopNavMenuItem();
//            CreateWebViews(options);

            return config;
        }

        private static void CreateWebViews(EmailSubscriptionOptions options)
        {
          
        }

        private static void CreateTopNavMenuItem()
        {
            DashboardRoutes.Routes.AddRazorPage(EmailSubscriptionDetailsPage.UrlRoute, x => new EmailSubscriptionDetailsPage());
            NavigationMenu.Items.Add(page => new MenuItem(EmailSubscriptionDetailsPage.Title, page.Url.To(EmailSubscriptionDetailsPage.UrlRoute))
            {
                Active = page.RequestPath.StartsWith(EmailSubscriptionDetailsPage.UrlRoute)
            });
        }
    }
}
