using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GlobalMethods
{
    public static class GlobalMethods
    {
        public static string BaseUrl = "https://opentdb.com/api.php?amount=REPLACENUMBER&category=REPLACE_CATEGORY";    
        public static async Task<string> GetData( string url )
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("URL inválido!");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static string ReplaceString(string baseString, string expressionToReplace, string replacementValue)
        {
            return baseString.Replace(expressionToReplace, replacementValue);
        }
    }
}


