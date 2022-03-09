#pragma checksum "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e22cc36992bde88fd9cf965bb9bb8531c55c7d8c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Index.cshtml", typeof(AspNetCore.Views_Home_Index))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 2 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\_ViewImports.cshtml"
using SampleCoreApp.Models;

#line default
#line hidden
#line 21 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#line 22 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
using SampleCoreApp.Controllers;

#line default
#line hidden
#line 23 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
using SampleCoreApp;

#line default
#line hidden
#line 24 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
using System.Web;

#line default
#line hidden
#line 25 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
using Newtonsoft.Json;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e22cc36992bde88fd9cf965bb9bb8531c55c7d8c", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1a395d4b621647a2bde9f3c81ff2162e84bea4da", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<SampleCoreApp.Models.SamplesTreeViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
  
    var globalAppSettings = ViewBag.GlobalAppSettings as GlobalAppSettings;
    var serverUrl = globalAppSettings.EmbedDetails.Serverurl;
    var baseUrl = globalAppSettings.EmbedDetails.BaseUrl;
    var environment = globalAppSettings.EmbedDetails.Environment;
    var authorizeUrl = @Url.Action("GetDetails", "Home");
    var deleteItemUrl = @Url.Action("DeleteItem", "Home");
    var refreshItemUrl = @Url.Action("RefreshItem", "Home");
    var shareItemUrl = @Url.Action("ShareItem", "Home");
    var category = ViewBag.Category as ApiItems;
    var isEdit = ViewBag.IsEdit != null && (bool)ViewBag.IsEdit;
    var noResourceFound = ViewBag.NoResourceFound != null && (bool)ViewBag.NoResourceFound;
    var token = ViewBag.Token;
    var draftId = ViewBag.DraftId;
    var title = string.IsNullOrWhiteSpace(Model?.Title) ? "" : Model.Title;
    var dashboardPath = string.IsNullOrWhiteSpace(Model?.DashboardPath) ? "" : Model.DashboardPath;
    var parentId = Model != null && Model.ParentId > 0 ? Model.ParentId : 0;

#line default
#line hidden
            BeginContext(1045, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(1235, 255, true);
            WriteLiteral("<div id=\"dashboardparent\" class=\"tab-pane\" style=\"height:100%;width:100%; overflow:hidden\">\r\n    <div style=\"height:100%;width:100%;overflow: hidden !important;\" id=\"sample_dashboard\"></div>\r\n</div>\r\n<script type=\"text/javascript\">\r\n    document.title = \"");
            EndContext();
            BeginContext(1491, 5, false);
#line 30 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                 Write(title);

#line default
#line hidden
            EndContext();
            BeginContext(1496, 23, true);
            WriteLiteral("\";\r\n    var category = ");
            EndContext();
            BeginContext(1520, 47, false);
#line 31 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
              Write(Html.Raw(JsonConvert.SerializeObject(category)));

#line default
#line hidden
            EndContext();
            BeginContext(1567, 30, true);
            WriteLiteral(";\r\n    var noResourceFound = \"");
            EndContext();
            BeginContext(1599, 36, false);
#line 32 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                       Write(noResourceFound.ToString().ToLower());

#line default
#line hidden
            EndContext();
            BeginContext(1636, 38, true);
            WriteLiteral("\" === \"true\";\r\n    var designerUrl = \"");
            EndContext();
            BeginContext(1675, 31, false);
#line 33 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                  Write(Url.Action("Index", "Designer"));

#line default
#line hidden
            EndContext();
            BeginContext(1706, 20, true);
            WriteLiteral("\";\r\n    var mode = \"");
            EndContext();
            BeginContext(1728, 27, false);
#line 34 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
            Write(isEdit.ToString().ToLower());

#line default
#line hidden
            EndContext();
            BeginContext(1756, 76, true);
            WriteLiteral("\" === \"true\" ? BoldBI.Mode.Design : BoldBI.Mode.View;\r\n    var serverUrl = \"");
            EndContext();
            BeginContext(1833, 9, false);
#line 35 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                Write(serverUrl);

#line default
#line hidden
            EndContext();
            BeginContext(1842, 29, true);
            WriteLiteral("\";\r\n    var dashboardPath = \"");
            EndContext();
            BeginContext(1872, 13, false);
#line 36 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                    Write(dashboardPath);

#line default
#line hidden
            EndContext();
            BeginContext(1885, 27, true);
            WriteLiteral("\";\r\n    var environment = \"");
            EndContext();
            BeginContext(1913, 11, false);
#line 37 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                  Write(environment);

#line default
#line hidden
            EndContext();
            BeginContext(1924, 99, true);
            WriteLiteral("\" === \"cloud\" ? BoldBI.Environment.Cloud : BoldBI.Environment.Enterprise;\r\n    var authorizeUrl = \"");
            EndContext();
            BeginContext(2024, 12, false);
#line 38 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                   Write(authorizeUrl);

#line default
#line hidden
            EndContext();
            BeginContext(2036, 29, true);
            WriteLiteral("\";\r\n    var deleteItemUrl = \"");
            EndContext();
            BeginContext(2066, 13, false);
#line 39 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                    Write(deleteItemUrl);

#line default
#line hidden
            EndContext();
            BeginContext(2079, 30, true);
            WriteLiteral("\";\r\n    var refreshItemUrl = \"");
            EndContext();
            BeginContext(2110, 14, false);
#line 40 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                     Write(refreshItemUrl);

#line default
#line hidden
            EndContext();
            BeginContext(2124, 25, true);
            WriteLiteral("\";\r\n    var shareItem = \"");
            EndContext();
            BeginContext(2150, 12, false);
#line 41 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                Write(shareItemUrl);

#line default
#line hidden
            EndContext();
            BeginContext(2162, 21, true);
            WriteLiteral("\";\r\n    var token = \"");
            EndContext();
            BeginContext(2184, 16, false);
#line 42 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
            Write(Html.Raw(@token));

#line default
#line hidden
            EndContext();
            BeginContext(2200, 23, true);
            WriteLiteral("\";\r\n    var baseUrl = \"");
            EndContext();
            BeginContext(2224, 7, false);
#line 43 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
              Write(baseUrl);

#line default
#line hidden
            EndContext();
            BeginContext(2231, 18, true);
            WriteLiteral("\";\r\n    var id = \"");
            EndContext();
            BeginContext(2250, 7, false);
#line 44 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
         Write(draftId);

#line default
#line hidden
            EndContext();
            BeginContext(2257, 26, true);
            WriteLiteral("\";\r\n    var catagoryId = \"");
            EndContext();
            BeginContext(2284, 8, false);
#line 45 "D:\2022-sprint-task\testing\embedding-sample\EmbededShell\EmbeddedbiShell\Views\Home\Index.cshtml"
                 Write(parentId);

#line default
#line hidden
            EndContext();
            BeginContext(2292, 247, true);
            WriteLiteral("\";\r\n    var datasourceName = \"\";\r\n\r\n    $(document).ready(function () {https://gitlab.syncfusion.com/data-science/embedded-bi-shell/blob/development/EmbededShell/EmbeddedbiShell/Models/SamplesManager.cs\r\n        embedSample();\r\n    }); \r\n</script>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<SampleCoreApp.Models.SamplesTreeViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591