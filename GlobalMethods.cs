using QuizAppWPF;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System;

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

        public static Score scoreMenu = new Score();
        public static EscolherCategoria pageCategoria = new EscolherCategoria();
        public static Login loginMenu = new Login();
        public static OptionMenu optMenu = new OptionMenu();


        public static string firebasePath = "https://quizz-login-default-rtdb.europe-west1.firebasedatabase.app/Utilizadores.json";

        public static FirestoreDb firedatabase;
        //Ligação com a Fire Store Database
        public static string path = AppDomain.CurrentDomain.BaseDirectory + @"quizzConnect.json";

        public static void firebaseConnection()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            firedatabase = FirestoreDb.Create("quizz-login");
        }

    }
}


