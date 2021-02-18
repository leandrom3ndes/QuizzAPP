using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace QuizAppWPF
{
    class TriviaInteractionAPI
    {
        public static string BaseUrl = "https://opentdb.com/api.php?amount=REPLACENUMBER&category=REPLACE_CATEGORY";

        public static async Task<string> GetData(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("URL inválido!");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }
}
