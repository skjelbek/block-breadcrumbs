using System.Web;
using System.Web.Mvc;
using EPiServer.Core;

namespace BlockBreadcrumbs
{
    public class ContentAreaRenderer : EPiServer.Web.Mvc.Html.ContentAreaRenderer
    {
        protected string CssClass { get; set; }

        public ContentAreaRenderer()
        {
            CssClass = "highlighted";
        }

        private const string Css = @"
.{0} {{
position: relative;
}}

.{0}:after {{
content: '';
position: absolute;
width: 100%;
height: 100%;
left: 0;
top: 0;
border: 20px solid #23b4e9;
}}";

        private const string Js = @"
(function(){{
    document.addEventListener('DOMContentLoaded', function(event) {{
        var highlighted = document.querySelectorAll('.{0}');
        if (highlighted.length > 0){{
            var first = highlighted[0];
            if (first.getBoundingClientRect){{
                var scrollTop = first.getBoundingClientRect().top;
                if (scrollTop > 0){{
                    if (window.scrollTo) window.scrollTo(0,scrollTop);
                }}
            }}
        }}
    }});
}})();
";

        public override void Render(HtmlHelper htmlHelper, ContentArea contentArea)
        {
            var writer = htmlHelper.ViewContext.Writer;
            writer.Write("<style>");
            writer.Write(Css, CssClass);
            writer.Write("</style>");
            writer.Write("<script>");
            writer.Write(Js, CssClass);
            writer.Write("</script>");
            base.Render(htmlHelper, contentArea);
        }

        protected override void BeforeRenderContentAreaItemStartTag(TagBuilder tagBuilder, ContentAreaItem contentAreaItem)
        {
            var highLight = HttpContext.Current.Request[Constants.HighlightQueryStringKey];
            if (highLight != null && highLight.Equals(contentAreaItem.GetContent().ContentGuid.ToString()))
                tagBuilder.AddCssClass(CssClass);
            base.BeforeRenderContentAreaItemStartTag(tagBuilder, contentAreaItem);
        }
    }
}
