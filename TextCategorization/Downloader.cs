using System;
using System.Net;

namespace TextCategorization
{
    public static class Downloader
    {
        public static string DownloadText(string url)
        {
        Again:
            try
            {
                var client = new WebClient();
                return client.DownloadString(url);
            }
            catch (Exception)
            {
                goto Again;
            }
        }
    }
}
