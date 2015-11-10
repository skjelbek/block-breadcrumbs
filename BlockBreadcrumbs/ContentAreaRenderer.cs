using System.Web;
using System.Web.Mvc;
using EPiServer.Core;

namespace BlockBreadcrumbs
{
    public class ContentAreaRenderer : EPiServer.Web.Mvc.Html.ContentAreaRenderer
    {
        private const string HighlightedCssClass = "bbcrumbs-highlighted";

        protected override void BeforeRenderContentAreaItemStartTag(TagBuilder tagBuilder, ContentAreaItem contentAreaItem)
        {
            var highLight = HttpContext.Current.Request[Constants.HighlightQueryStringKey];
            if (highLight != null && highLight.Equals(contentAreaItem.GetContent().ContentGuid.ToString()))
                tagBuilder.AddCssClass(HighlightedCssClass);
            base.BeforeRenderContentAreaItemStartTag(tagBuilder, contentAreaItem);
        }
    }
}
