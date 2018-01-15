using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class UrlHelper
    {
        public static string GenerateContentUrl(string uniqueFieldValue)
        {
            return Regex.Replace(uniqueFieldValue.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");
        }

        public static string RandomIdentifierGenerator(int randomCharCount, string prefix = "", string suffix = "")
        {
            Random random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, randomCharCount)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            if (!string.IsNullOrEmpty(prefix))
                result = prefix + "-" + result;

            if (!string.IsNullOrEmpty(suffix))
                result += "-" + suffix;

            return result;
        }

       
    }
}