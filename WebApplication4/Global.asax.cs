using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using WebApplication4.Modules;
using WebApplication4.Services;
using WebApplication4.SignalIR;

namespace WebApplication4
{
    public class MvcApplication : System.Web.HttpApplication
    {

        string con = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new Autofac.ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();

            builder.RegisterModule(new RepositoryModule());
            builder.RegisterModule(new EFModule());
            builder.RegisterFilterProvider();
            var container = builder.Build();
            //Start SqlDependency with application initialization

            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                Debug.WriteLine(eventArgs.Exception.ToString());
            };
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            System.Data.SqlClient.SqlDependency.Stop(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        }
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                HttpApplication application = (HttpApplication)sender;
                HttpContext context = application.Context;
                if(context.Session != null) {
                var user = User.Identity.GetUserId();
                var userProfileService = new UserProfileService();
                var notificationService = new NotificationService();
                
                var userProfile = userProfileService.GetUserProfileByUserId(new Guid(user));
                var notifications = notificationService.GetNewNotifications(userProfile.Id);
                var notificationsTotal = notificationService.GetUnseenNotificationsCount(userProfile.Id);
                var requests = notificationService.GetAllUnansweredRequests(userProfile.Id);
                
                if (notifications != null)
                {
                    context.Session["notifications"] = notifications;
                    context.Session["notificationsTotal"] = notificationsTotal;
                }

                if (requests != null)
                {
                    context.Session["requests"] = requests;
                }
                context.Session["userAddress"] = userProfile.UserAddress;
                context.Session["userName"] = userProfile.Name;
                context.Session["userId"] = userProfile.User.Id;
                context.Session["userProfileId"] = userProfile.Id;
                }

            }
        }
    }
}
