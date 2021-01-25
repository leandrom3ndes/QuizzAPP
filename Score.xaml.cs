using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Google.Cloud.Firestore;
using static GlobalMethods.GlobalMethods;

namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para Score.xam
    /// </summary>
    public partial class Score : Page
    {
        public Score()
        {
            InitializeComponent();
        }

        private async Task getScores()
        {
            string userNameAux = Login.username;
            CollectionReference coll = firedatabase.Collection("Scores");
            Query scoresQuery = coll.WhereEqualTo("Username", userNameAux); //meter aqui em vez do nome do bolha, colocar 
            QuerySnapshot scoresSnap = await scoresQuery.GetSnapshotAsync();
            //QuerySnapshot scoresSnap = await coll.GetSnapshotAsync();
            List<string> items = new List<string>();
            int pontuacao;
            string categoria;
            //foreach (DocumentSnapshot document in scoresSnap.Documents)
            foreach (DocumentSnapshot document in scoresSnap.Documents)
            {
                pontuacao = document.GetValue<int>("Pontuacao");
                categoria = document.GetValue<string>("categoria");
                items.Add("Pontuação: " + pontuacao.ToString() + "             " + "Categoria: " + categoria);
            }
            lvDataBinding.ItemsSource = items;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;
            switch (objname)
            {
                case "ShowScore":
                    await getScores();
                    title.Visibility = Visibility.Visible;
                    lvDataBinding.Visibility = Visibility.Visible;
                    break;
                case "HomeButton":
                    title.Visibility = Visibility.Hidden;
                    lvDataBinding.Visibility = Visibility.Hidden;
                    NavigationService.Navigate(optMenu);
                    break;
            }
        }
    }
}
