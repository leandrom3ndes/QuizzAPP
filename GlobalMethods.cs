using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GlobalMethods
{
    public static class GlobalMethods
    {
        public static async Task<string> getData( string url )
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }


        public static string getReplaceRegex( string baseString, string expressionToReplace, string replacementValue )
        {
            Regex rgx1 = new Regex(expressionToReplace);
            return rgx1.Replace(baseString, replacementValue);
        }
    }

}


