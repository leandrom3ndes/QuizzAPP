namespace QuizAppWPF
{
    class Utilizador
    {
        public string NomeUtilizador { get; set; }
        public string NomeCurso { get; set; }
        public string Curso { get; set; }
        public string NrAluno { get; set; }
        public string Password { get; set; }

        private static string _error = "Dados incorretos. Tente novamente";

        public static void ShowError()
        {
            System.Windows.Forms.MessageBox.Show(_error);
        }

        public static bool IsEqual(Utilizador User1, Utilizador User2)
        {
            if (User1 == null || User2 == null) { return false; }

            if (User1.NomeUtilizador != User2.NomeUtilizador)
            {
                _error = "Nome de utilizador incorreto!";
                return false;
            }

            else if (User1.Password != User2.Password)
            {
                _error = "Palavra passe incorreta!";
                return false;
            }
            return true;
        }

        public static bool IsEqualName(Utilizador User1, Utilizador User2)
        {
            if (User1 == null || User2 == null) { return false; }

            if (User1.NomeUtilizador == User2.NomeUtilizador)
            {
                _error = "Nome de utilizador já existe";
                return true;
            }
            return true;
        }
    }

}
