using System.Web;
using System.Web.Optimization;

namespace MasterISS_Agent_Website
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.


            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/all-page-css").IncludeDirectory("~/Content/css/", "*.css"));

            bundles.Add(new ScriptBundle("~/Scripts/new-customer-operations").IncludeDirectory("~/Scripts/Customer/", "*.js"));
            bundles.Add(new ScriptBundle("~/Scripts/address-query").IncludeDirectory("~/Scripts/QueryAddress/", "*.js"));
            bundles.Add(new ScriptBundle("~/Scripts/generic-alert").IncludeDirectory("~/Scripts/Alert/", "*.js"));
            bundles.Add(new ScriptBundle("~/Scripts/home-operations").IncludeDirectory("~/Scripts/Home/", "*.js"));

            bundles.Add(new ScriptBundle("~/Scripts/all-page-js").IncludeDirectory("~/Scripts/AllPages/", "*.js"));

        }
    }
}
