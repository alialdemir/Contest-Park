namespace ContestPark.Mobile.Extensions
{
    public static class UrlExtension
    {
        public static bool IsUrl(this string url) => url.StartsWith("http://") || url.StartsWith("https://");
    }
}
