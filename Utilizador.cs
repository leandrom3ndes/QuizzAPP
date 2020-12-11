using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppWPF
{
    class Utilizador
    {
        public string nomeUtilizador { get; set; }
        public string nomeCurso { get; set; }
        public string Curso { get; set; }
        public string nrAluno { get; set; }
        public string Password { get; set; }

        private static string error = "Dados incorretos. Tente novamente";

        public static void ShowError()
        {
            System.Windows.Forms.MessageBox.Show(error);
        }

        public static bool IsEqual(Utilizador user1, Utilizador user2)
        {
            if (user1 == null || user2 == null) { return false; }

            if (user1.nomeUtilizador != user2.nomeUtilizador)
            {
                error = "Nome de utilizador incorreto!";
                return false;
            }

            else if (user1.Password != user2.Password)
            {
                error = "Palavra passe incorreta!";
                return false;
            }

            else if (user1.nomeUtilizador == user2.nomeUtilizador)
            {
                error = "Nome de utilizador já existe";
                return true;
            }

            return true;
        }
        /*public static bool IsEqualName(Utilizador user1, Utilizador user2)
        {
            if (user1 == null || user2 == null) { return false; }

            if (user1.nomeUtilizador == user2.nomeUtilizador)
            {
                error = "Nome de utilizador já existe";
                return true;
            }
            return true;
        }*/
    }

}
