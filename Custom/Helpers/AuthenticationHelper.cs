using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class AuthenticationHelper
    {
        public static string GenerateToken(string username, string password, string domainUrl)
        {
            FormUrlEncodedContent requestBody = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("wrap_name", username),
                new KeyValuePair<string, string>("wrap_password", password)
            });

            var client = new HttpClient();
            client.BaseAddress = new Uri(domainUrl);
            var loginResult = client.PostAsync("/Sitefinity/Authenticate/SWT", requestBody).Result;
            var responseText = loginResult.Content.ReadAsStringAsync().Result;

            var responseParameters = System.Web.HttpUtility.ParseQueryString(responseText);
            string response = responseParameters["wrap_access_token"];

            //string tokenId = System.Web.HttpUtility.ParseQueryString(response)["TokenId"];
            //int validity_secs = Convert.ToInt32(responseParameters["wrap_access_token_expires_in"]);
            return response;
        }
    }
}