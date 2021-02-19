using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace QuizAppWPF
{
    /// <summary>
    /// Interaction logic for Registo.xaml
    /// </summary>
    public partial class Registo : Page
    {

        private IFirebaseClient _firebaseClient;
        private FirebaseResponse _firebaseResponse;
        public Registo()
        {
            InitializeComponent();
        }

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

        }

        private async Task<Task> Register()
        {
            LoadingCursor.StartLoadingCursor();
            
            try
            {
                _firebaseClient = new FireSharp.FirebaseClient(DatabaseAPI.ifc);
            }

            catch
            {
                MessageBox.Show("Problema de conexão!");
            }
            _firebaseResponse = await _firebaseClient.GetAsync(@"Utilizadores/" + UsernameTbox.Text);

            await InsertRealtime();

            return Task.CompletedTask;
        }

        private async Task<Task> InsertRealtime()
        {
            Utilizador user = new Utilizador()
            {
                NomeUtilizador = UsernameTbox.Text,
                NomeCurso = cursoTbox.Text,
                Curso = TipoCbox.Text,
                NrAluno = nrAlunoTbox.Text,
                Password = passTbox.Password,
            };
            Utilizador ResUser = _firebaseResponse.ResultAs<Utilizador>();    // Resultado da base de dados
            if (Utilizador.IsEqualName(ResUser, user))
            {
                LoadingCursor.StopLoadingCursor();
                MessageBox.Show("O utilizador já existe! Escolha outro nome.");
            }
            else
            {
                await _firebaseClient.SetAsync(@"Utilizadores/" + UsernameTbox.Text, user);
                LoadingCursor.StopLoadingCursor();
                MessageBox.Show("Registo efetuado com sucesso!");
                NavigationService.Navigate(Enunciado.loginMenu);
            }
            return Task.CompletedTask;
        }
    }

}
