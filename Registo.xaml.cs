using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace QuizAppWPF
{
    /// <summary>
    /// Interaction logic for Registo.xaml
    /// </summary>
    public partial class Registo : Page
    {
        public Registo()
        {
            InitializeComponent();
        }

        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "fBAXrDx2fkycRdUVuFTdofM73afM5gfa5rbzTXry",
            BasePath = "https://quizz-login-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        IFirebaseClient client;

        private async void RegBtnClick(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;
            switch (objname)
            {
                case "regBtn":
                    await Register();
                    break;

            }
            #region Condition

        }

        private async Task Register()
        {

            try
            {
                client = new FireSharp.FirebaseClient(ifc);
            }

            catch
            {
                MessageBox.Show("Problema de conexão!");
            }
            #region Condition
            if (string.IsNullOrWhiteSpace(UsernameTbox.Text) ||
               string.IsNullOrWhiteSpace(passTbox.Password) ||
               string.IsNullOrWhiteSpace(TipoCbox.Text) ||
               string.IsNullOrWhiteSpace(cursoTbox.Text) ||
               string.IsNullOrWhiteSpace(nrAlunoTbox.Text))
            {
                MessageBox.Show("Preencha todos os campos!");
                return;
            }

            #endregion
            #endregion
            FirebaseResponse res = client.Get(@"Utilizadores/" + UsernameTbox.Text);
            Utilizador ResUser = res.ResultAs<Utilizador>();    // Resultado da base de dados

            Utilizador user = new Utilizador()
            {
                nomeUtilizador = UsernameTbox.Text,
                nomeCurso = cursoTbox.Text,
                Curso = TipoCbox.Text,
                nrAluno = nrAlunoTbox.Text,
                Password = passTbox.Password
            };

            if (Utilizador.IsEqualName(ResUser, user))
            {
                MessageBox.Show("O utilizador já existe! Escolha outro nome.");
            }
            else
            {
                client.Set(@"Utilizadores/" + UsernameTbox.Text, user);
                MessageBox.Show("Registo efetuado com sucesso!");
            }

        }

    }

}
