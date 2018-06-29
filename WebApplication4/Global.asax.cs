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
using System.Globalization;
using System.Threading;

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
        protected void Application_AcquireRequestState(object sender, EventArgs e)

     {

          //It's important to check whether session object is ready

          if (HttpContext.Current.Session != null)

          {

              CultureInfo ci = (CultureInfo)this.Session["Culture"];

              //Checking first if there is no value in session 

              //and set default language 

              //this can happen for first user's request

           if (ci == null)

           {

               //Sets default culture to english invariant

               string langName = "en";

  

               //Try to get values from Accept lang HTTP header

               if (HttpContext.Current.Request.UserLanguages != null && 

           HttpContext.Current.Request.UserLanguages.Length != 0)

               {

                   //Gets accepted list 

                   langName = HttpContext.Current.Request.UserLanguages[0].Substring(0, 2);

               }

               ci = new CultureInfo(langName);

               this.Session["Culture"] = ci;

           }

           //Finally setting culture for each request

           Thread.CurrentThread.CurrentUICulture = ci;

           Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);

       }

  }
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                HttpApplication application = (HttpApplication)sender;
                HttpContext context = application.Context;
                if (context.Session != null)
                {
                    var user = User.Identity.GetUserId();
                    var userProfileService = new UserProfileService();
                    var notificationService = new NotificationService();

                    var userProfile = userProfileService.GetUserProfileByUserId(new Guid(user));
                    if (userProfile != null)
                    {
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
}
