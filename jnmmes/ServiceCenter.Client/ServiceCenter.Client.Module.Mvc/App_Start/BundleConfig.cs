using System.Web;
using System.Web.Optimization;

namespace ServiceCenter.Client.Mvc
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                            .Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui")
                            .Include("~/Scripts/jquery-ui.js")
                            .Include("~/Scripts/jquery-ui-addon-timepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                            .Include("~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-form")
                            .Include("~/Scripts/jquery.form.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-jqGrid")
                            .Include("~/Scripts/jquery.jqGrid.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryunobtrusive")
                            .Include("~/Scripts/jquery.unobtrusive*"));

            bundles.Add(new ScriptBundle("~/bundles/colorpicker")
                            .Include("~/Scripts/evol.colorpicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/imagegallery")
                            .Include("~/Scripts/bootstrap-image-gallery.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery-blueimpgallery")
                            .Include("~/Scripts/jquery.blueimp-gallery.min.js"));
            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                            .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                            .Include("~/Scripts/bootstrap.js","~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                            .Include("~/Content/bootstrap.css","~/Content/site.css","~/Content/jquery-ui*"));
            bundles.Add(new StyleBundle("~/Content/css/jqgrid")
                            .Include("~/Content/ui.jqgrid.css"));
            bundles.Add(new StyleBundle("~/Content/css/colorpicker")
                            .Include("~/Content/evol.colorpicker.css"));

            bundles.Add(new StyleBundle("~/Content/css/blueimpgallery")
                            .Include("~/Content/blueimp-gallery.min.css"));
            bundles.Add(new StyleBundle("~/Content/css/imagegallery")
                            .Include("~/Content/bootstrap-image-gallery.css"));

        }
    }
}
