using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

using static GlobalMethods.GlobalMethods;

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

        private IFirebaseClient client;
        private FirebaseResponse res;

        private async void RegBtnClick(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;
            switch (objname)
            {
                case "regBtn":
                    await Register();
                    break;
                case "HomeButton123":
                    NavigationService.Navigate(Enunciado.loginMenu);
                    break;

            }
            #region Condition

        }

        private async Task<Task> Register()
        {
            StartLoadingCursor();
            
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
                StopLoadingCursor();
                MessageBox.Show("Preencha todos os campos!");
                return Task.CompletedTask;
            }

            #endregion
            #endregion
            res = await client.GetAsync(@"Utilizadores/" + UsernameTbox.Text);

            await InsertRealtime();

            return Task.CompletedTask;
        }

        private async Task<Task> InsertRealtime()
        {
            Utilizador user = new Utilizador()
            {
                nomeUtilizador = UsernameTbox.Text,
                nomeCurso = cursoTbox.Text,
                Curso = TipoCbox.Text,
                nrAluno = nrAlunoTbox.Text,
                Password = passTbox.Password,
            };
            Utilizador ResUser = res.ResultAs<Utilizador>();    // Resultado da base de dados
            if (Utilizador.IsEqualName(ResUser, user))
            {
                StopLoadingCursor();
                MessageBox.Show("O utilizador já existe! Escolha outro nome.");
            }
            else
            {
                await client.SetAsync(@"Utilizadores/" + UsernameTbox.Text, user);
                StopLoadingCursor();
                MessageBox.Show("Registo efetuado com sucesso!");
                NavigationService.Navigate(Enunciado.loginMenu);
            }
            return Task.CompletedTask;
        }
    }

}
