using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using FireSharp.Interfaces;
using FireSharp.Config;

namespace QuizAppWPF
{
    class DatabaseAPI
    {
        public static string firebasePath = "https://quizz-login-default-rtdb.europe-west1.firebasedatabase.app/Utilizadores.json";

        public static FirestoreDb firedatabase;
        //Ligação com a Fire Store Database
        public static string path = AppDomain.CurrentDomain.BaseDirectory + @"quizzConnect.json";

        public static void FirebaseConnection()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            firedatabase = FirestoreDb.Create("quizz-login");
        }

        //Ligação com a Realtime Database
        public static IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "fBAXrDx2fkycRdUVuFTdofM73afM5gfa5rbzTXry",
            BasePath = "https://quizz-login-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        public static async Task PostData(object data, string dbCollection)
        {
            CollectionReference coll = firedatabase.Collection(dbCollection);
            await coll.AddAsync(data);
        }

    }
}
