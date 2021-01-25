
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using static GlobalMethods.GlobalMethods;
using System.Windows.Input;

namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para Login.xam
    /// </summary>
    /// 

    public partial class Login : Page
    {
        public static string username { get; set; }
        private IFirebaseClient client;
        private FirebaseResponse res;

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
                    StartLoadingCursor();
                    await login();
                    StopLoadingCursor();
                    firebaseConnection();
                    break;
                case "regBtn":
                    Registo();
                    break;

            }
            #region Condition
            
        }
        private void Registo()
        {
            Registo registo = new Registo();
            this.NavigationService.Navigate(registo);
        }

        private async Task<Task> login()
        {

            try
            {
                client = new FireSharp.FirebaseClient(ifc);
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
            #endregion
            StartLoadingCursor();
            res = await client.GetAsync(@"Utilizadores/" + UsernameTbox.Text);
            StopLoadingCursor();
            
            await VerifyLogin();
            return Task.CompletedTask;
        }

        private Task VerifyLogin()
        {
            Utilizador CurUser = new Utilizador()               // Informação do utilizador
            {
                nomeUtilizador = UsernameTbox.Text,
                Password = passTbox.Password
            };
            Utilizador ResUser = res.ResultAs<Utilizador>();    // Resultado da base de dados

            //Caso os dados estejam corretos é redireccionado para a APP
            if (Utilizador.IsEqual(ResUser, CurUser))
            {
                username = UsernameTbox.Text;
                OptionMenu oP = new OptionMenu();
                this.NavigationService.Navigate(oP);
            }

            else
            {
                Utilizador.ShowError();
            }
            return Task.CompletedTask;
        }

    }

}
