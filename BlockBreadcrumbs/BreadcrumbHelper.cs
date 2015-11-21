using System;
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
        private const string CssNamespace = "bbcrumbs";

        public static void RenderBlockBreadcrumbs(this HtmlHelper htmlHelper, BlockData currentBlock)
        {
            var currentBlockAsIContent = currentBlock as IContent;
            if (currentBlockAsIContent == null)
                return;

            htmlHelper.RenderBlockBreadcrumbs(currentBlockAsIContent);
        }

        public static void RenderBlockBreadcrumbs(this HtmlHelper htmlHelper, IContent currentBlock)
        {
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
                    CssNamespace);
                return;
            }

            var highlights = usages.Concat(new[] { currentBlock }).GetEnumerator();
            highlights.MoveNext();

            var breadCrumbWrapper = new TagBuilder("div");
            breadCrumbWrapper.AddCssClass(CssNamespace);

            writer.Write(breadCrumbWrapper.ToString(TagRenderMode.StartTag));

            if (topLevelNumberOfReferences > 1)
            {
                writer.Write("<div class=\"{0}-item\">" +
                             "<span class=\"{0}-title\" title=\"{1} usages...\">{1} usages...</span><div>" +
                             "<a class=\"{0}-usage {0}-usage--multiple\">...</a>" +
                             "<span class=\"{0}-arrow\">❯</span></div></div>",
                    CssNamespace,
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
                    CssNamespace
                    , usage.IsPublished() ? "" : string.Format("{0}-pending", CssNamespace)
                    , usage.Name
                    , url
                    , url.AddQueryParameter(Constants.HighlightQueryStringKey, highlights.Current.ContentGuid.ToString()));

                writer.Write(html);
            }

            writer.Write("<div class=\"{0}-item\"><span class=\"{0}-title {0}-this\">This block</span></div>", CssNamespace);
            writer.Write(breadCrumbWrapper.ToString(TagRenderMode.EndTag));
        }
    }
}
