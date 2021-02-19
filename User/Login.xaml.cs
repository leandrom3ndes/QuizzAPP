using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para Login.xam
    /// </summary>
    /// 

    public partial class Login : Page
    {
        public static string Username { get; set; }
        private IFirebaseClient _firebaseClient;
        private FirebaseResponse _firebaseResponse;

        public Login()
        { 
            InitializeComponent();
        }

        private async void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;
            switch (objname)
            {
                case "LoginBtn":
                    LoadingCursor.StartLoadingCursor();
                    await AsyncLogin();
                    LoadingCursor.StopLoadingCursor();
                    DatabaseAPI.FirebaseConnection();
                    break;
                case "regBtn":
                    Registo();
                    break;
            }
        }

        private void Registo()
        {
            Registo registo = new Registo();
            this.NavigationService.Navigate(registo);
        }

        private async Task<Task> AsyncLogin()
        {

            try
            {
                _firebaseClient = new FireSharp.FirebaseClient(DatabaseAPI.ifc);
            }
            catch
            {
                MessageBox.Show("Problema de conexão!");
            }

            if (string.IsNullOrWhiteSpace(UsernameTbox.Text) ||
               string.IsNullOrWhiteSpace(passTbox.Password))
            {
                MessageBox.Show("Preencha todos os campos!");
            }
            LoadingCursor.StartLoadingCursor();
            _firebaseResponse = await _firebaseClient.GetAsync(@"Utilizadores/" + UsernameTbox.Text);
            LoadingCursor.StopLoadingCursor();
            
            await VerifyLogin();
            return Task.CompletedTask;
        }

        private Task VerifyLogin()
        {
            Utilizador CurUser = new Utilizador()               // Informação do utilizador
            {
                NomeUtilizador = UsernameTbox.Text,
                Password = passTbox.Password
            };
            Utilizador ResUser = _firebaseResponse.ResultAs<Utilizador>();    // Resultado da base de dados

            //Caso os dados estejam corretos é redireccionado para a APP
            if (Utilizador.IsEqual(ResUser, CurUser))
            {
                Username = UsernameTbox.Text;
                OptionMenu oP = new OptionMenu();
                NavigationService.Navigate(oP);
            }

            else
            {
                Utilizador.ShowError();
            }
            return Task.CompletedTask;
        }

    }

}
