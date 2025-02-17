﻿using MasterISS_Agent_Website_Business.Abstract;
using MasterISS_Agent_Website_Business.Concrete;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
[assembly: OwinStartup(typeof(MasterISS_Agent_Website.App_Start.Startup))]

namespace MasterISS_Agent_Website.App_Start
{
    public class Startup
    {
    
        public void Configuration(IAppBuilder app)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            CookieAuthenticationProvider provider = new CookieAuthenticationProvider();

            var originalHandler = provider.OnApplyRedirect;

            //Our logic to dynamically modify the path
            provider.OnApplyRedirect = context =>
            {
                var mvcContext = new HttpContextWrapper(HttpContext.Current);
                var routeData = RouteTable.Routes.GetRouteData(mvcContext);

                //Get the current language  
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("lang", routeData.Values["lang"]);

                //Reuse the RetrunUrl
                Uri uri = new Uri(context.RedirectUri);
                string returnUrl = HttpUtility.ParseQueryString(uri.Query)[context.Options.ReturnUrlParameter];
                routeValues.Add(context.Options.ReturnUrlParameter, returnUrl);

                //Overwrite the redirection uri
                context.RedirectUri = url.Action("SubUserSignIn", "Account", routeValues);
                originalHandler.Invoke(context);
            };

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(value: "/SubUserSignIn/SignIn"),
                //Set the Provider
                CookieName = "MasterISSAgentWebsite",
                Provider = provider,
                ExpireTimeSpan = Properties.Settings.Default.CookieExpiration,
                CookieSecure = CookieSecureOption.SameAsRequest
            });
        }
    }
}
