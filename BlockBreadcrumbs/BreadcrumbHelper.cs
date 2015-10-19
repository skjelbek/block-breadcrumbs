using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;

namespace BlockBreadcrumbs
{
    public static class BreadcrumbHelper
    {
        private const string DefaultCssClass = "bbcrumbs";

        private const string Css = @"
.{0} {{
font-family: Verdana, Arial, Helvetica, sans-serif;
color: #333333;
height: 122px;
margin-bottom: 20px;
margin-top: 10px;
}}

.{0} .{0}-item {{
float: left;
width: 102px;
margin-right: 40px;
height: 100%;
position: relative;
}}
.{0} .{0}-title {{
font-size: 0.8125em;
overflow: hidden;
-ms-text-overflow: ellipsis;
-o-text-overflow: ellipsis;
text-overflow: ellipsis;
white-space: nowrap;
width: 100%;
display: inline-block;
}}

.{0} .{0}-title.{0}-this {{
position: absolute;
top: 50%;
margin-top: 5px;
}}

.{0} .{0}-usage {{
display: block;
border: 1px solid #898989;
height: 102px;
width: 102px;
position: relative;
background: white;
}}

.{0} .{0}-usage--multiple{{
line-height: 90px;
text-align: center;
font-size: 27px;
color: #6E6E6E;
}}

.{0} .{0}-usage--multiple:after {{
position: absolute;
content: '';
top: 5px;
left: 5px;
height: 100%;
width: 100%;
border: 1px solid #898989;
z-index: -1;
}}
.{0} .{0}-item.{0}-pending .{0}-usage {{
border-color: #23b4e9;
position: relative;
}}

.{0} .{0}-item.{0}-pending .{0}-usage:after {{
position: absolute;
content: 'Unpublished';
bottom: -9px;
right: 6px;
background: #23b4e9;
font-size: 8px;
color: white;
padding: 0 4px;
}}

.{0} .{0}-arrow {{
position: absolute;
right: -26px;
top: 50%;
margin-top: 5px;
color: #717171;
font-size: 18px;
}}

.{0} iframe {{
pointer-events: none;
-ms-zoom: 0.08547009;
-moz-transform: scale(0.08547009);
-moz-transform-origin: 0 0;
-o-transform: scale(0.08547009);
-o-transform-origin: 0 0;
-webkit-transform: scale(0.08547009);
transform: scale(0.08547009);
-webkit-transform-origin: 0 0;
-ms-transform-origin: 0 0;
transform-origin: 0 0;
}}
";

        public static void RenderBlockBreadcrumbs(this HtmlHelper htmlHelper, IContent currentBlock = null, string cssClass = null)
        {
            currentBlock = currentBlock ?? htmlHelper.ViewContext.RouteData.Values["currentBlock"] as IContent;
            if (currentBlock != null)
            {
                cssClass = cssClass ?? DefaultCssClass;
                var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
                var urlHelper = ServiceLocator.Current.GetInstance<UrlHelper>();
                var writer = htmlHelper.ViewContext.Writer;

                int topLevelNumberOfReferences;
                var usages = currentBlock.GetReferenceTree(repo, out topLevelNumberOfReferences).ToList();

                if (!usages.Any() && topLevelNumberOfReferences <= 1)
                {
                    writer.Write("<div class=\"{0}-item\">" +
                                     "<span class=\"{0}-title\">This block has no usages yet</span><div>" +
                                     "<a class=\"{0}-usage {0}-usage--multiple\" href=\"\"></a>" +
                                     "</div></div>",
                            cssClass);
                    return;
                }

                var highlights = usages.Concat(new[] { currentBlock }).GetEnumerator();
                highlights.MoveNext();

                var breadCrumbWrapper = new TagBuilder("div");
                breadCrumbWrapper.AddCssClass(cssClass);

                writer.Write("<style>");
                writer.Write(Css, cssClass);
                writer.Write("</style>");

                writer.Write(breadCrumbWrapper.ToString(TagRenderMode.StartTag));

                if (topLevelNumberOfReferences > 1)
                {
                    writer.Write("<div class=\"{0}-item\">" +
                                 "<span class=\"{0}-title\" title=\"{1} usages...\">{1} usages...</span><div>" +
                                 "<a class=\"{0}-usage {0}-usage--multiple\">...</a>" +
                                 "<span class=\"{0}-arrow\">❯</span></div></div>",
                        cssClass,
                        topLevelNumberOfReferences);
                }
                foreach (var usage in usages)
                {
                    highlights.MoveNext();
                    var url = urlHelper.ContentUrl(usage.ContentLink);

                    var html = string.Format("<div class=\"{0}-item {1}\">" +
                                             "<span class=\"{0}-title\" title=\"{2}\">{2}</span><div>" +
                                             "<a href=\"{3}\" class=\"{0}-usage\">" +
                                             "<iframe src=\"{4}\" width=\"1170\" height=\"1170\" scrolling=\"no\"></iframe>" +
                                             "</a><span class=\"{0}-arrow\">❯</span></div></div>",
                        cssClass
                        , usage.IsPublished() ? "" : string.Format("{0}-pending", cssClass)
                        , usage.Name
                        , url
                        , url.AddQueryParameter(Constants.HighlightQueryStringKey, highlights.Current.ContentGuid.ToString()));

                    writer.Write(html);
                }

                writer.Write("<div class=\"{0}-item\"><span class=\"{0}-title {0}-this\">This block</span></div>", cssClass);
                writer.Write(breadCrumbWrapper.ToString(TagRenderMode.EndTag));
            }
        }
    }
}
