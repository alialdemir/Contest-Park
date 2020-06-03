namespace ContestPark.Mobile.Extensions
{
    public static class UrlExtension
    {
        public static bool IsUrl(this string url) => !string.IsNullOrEmpty(url) && url.StartsWith("http://") || url.StartsWith("https://");
    }
}
