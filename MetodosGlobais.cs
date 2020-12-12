using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FireSharp.Config;
using FireSharp.Interfaces;

namespace QuizAppWPF
{
    class MetodosGlobais
    {
        public object firebaseConetion()
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "fBAXrDx2fkycRdUVuFTdofM73afM5gfa5rbzTXry",
                BasePath = "https://quizz-login-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            return ifc;
        }
    }
}
