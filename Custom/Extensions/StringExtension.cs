using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SitefinityWebApp.Custom.Extensions
{
    public static class StringExtension
    {
        public static string StripHtmlTags(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
        }

        public static string GetCorrectUrl(this string url)
        {
            Uri oldUri;
            bool isValidUri = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out oldUri);

            if (isValidUri == false)
            {
                return "Invalid URL";
            }

            if (oldUri.Scheme == Uri.UriSchemeHttps)
                url = url.Replace("http://", "https://");

            return url;
        }
    }
}