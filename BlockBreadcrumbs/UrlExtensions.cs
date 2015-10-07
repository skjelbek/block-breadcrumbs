using EPiServer;

namespace BlockBreadcrumbs
{
    internal static class UrlExtensions
    {
        public static string AddQueryParameter(this string url, string name, string value)
        {
            return UriSupport.AddQueryString(url, name, value);
        }
    }
}
