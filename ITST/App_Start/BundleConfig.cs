using System.Web;
using System.Web.Optimization;

namespace ITST
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.dataTables.js",
                        "~/Scripts/jquery-1.12.3.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/DataTables/jquery.dataTables.js",
                        "~/Scripts/DataTables/jquery.dataTables.min.js",
                        "~/Scripts/DataTables/dataTables.responsive.min.js",
                        "~/Scripts/DataTables/dataTables.responsive.min.js",
                        "~/Scripts/DataTables/dataTables.tableTools.js",
                        "~/Scripts/myApps/datetime-moment.js",
                        "~/Scripts/myApps/moment.min.js",
                        "~/Scripts/DataTables/buttons.print.min.js",
                        "~/Scripts/DataTables/pdfmake.min.js",
                        "~/Scripts/DataTables/jszip.min.js",
                        "~/Scripts/DataTables/buttons.flash.min.js",
                        "~/Scripts/DataTables/dataTables.buttons.min.js"));
                        //"~/Scripts/DataTables/dataTables.bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/Chart.js",
                      "~/Scripts/Chart.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/datatables/css/buttons.dataTables.min.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/datepicker.css",
                        "~/Content/themes/base/jquery-ui.min.css",
                        "~/Content/themes/base/jquery-ui.css",
                        "~/Content/themes/base/jquery-ui.structure.min.css",
                        "~/Content/themes/base/jquery-ui.structure.css",
                        "~/Content/themes/base/jquery-ui.theme.min.css",
                        "~/Content/themes/base/jquery-ui.theme.css",
                        "~/Content/DataTables/css/jquery.dataTables.css",
                        "~/Content/DataTables/css/responsive.dataTables.css",
                        "~/Content/DataTables/css/jquery.dataTables.min.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}
