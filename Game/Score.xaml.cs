using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Google.Cloud.Firestore;

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
        string categoriaSelected = "";
        string searchTypeSelected = "";

        private async Task GetScores()
        {
            string userNameAux = Login.Username;
            CollectionReference coll = DatabaseAPI.firedatabase.Collection("Scores");
            Query scoresQuery;
            if (searchTypeSelected == "user")
            {
                if (categoriaSelected == "0")
                {
                    scoresQuery = coll.WhereEqualTo("Username", userNameAux).OrderBy("Pontuacao"); //todas as classificações obtidas pelo utilizador ordenada por pontuação crescente
                }
                else
                {
                    scoresQuery = coll.WhereEqualTo("Username", userNameAux).WhereEqualTo("categoria", categoriaSelected).OrderBy("Pontuacao"); //classificações obtidas pelo utilizador na categoria selecionada ordenada por pontuação crescente
                }
            }
            else
            {
                if (categoriaSelected == "0")
                {
                    scoresQuery = coll.OrderBy("Pontuacao"); //classificação geral ordenada por pontuação crescente
                }
                else
                {
                    scoresQuery = coll.WhereEqualTo("categoria", categoriaSelected).OrderBy("Pontuacao"); //classificação geral da categoria escolhida ordenada por pontuação crescente
                }
            }
            QuerySnapshot scoresSnap = await scoresQuery.GetSnapshotAsync();
            List<string> items = new List<string>();
            int pontuacao;
            string categoria;
            string utilizadorDoc;
            foreach (DocumentSnapshot document in scoresSnap.Documents)
            {
                pontuacao = document.GetValue<int>("Pontuacao");
                categoria = document.GetValue<string>("categoria");
                utilizadorDoc = document.GetValue<string>("Username");
                if (searchTypeSelected == "user")
                {
                    items.Add("Pontuação: " + pontuacao.ToString());
                }
                else
                {
                    items.Add("Pontuação: " + pontuacao.ToString() + "             " + "Utilizador: " + utilizadorDoc);
                }
            }
            lvDataBinding.ItemsSource = items;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;
            switch (objname)
            {
                case "HomeButton":
                    NavigationService.Navigate(PageNavigation.optMenu);
                    break;
                case "ShowScore":
                    LoadingCursor.StartLoadingCursor();
                    await GetScores();
                    LoadingCursor.StopLoadingCursor();
                    break;
            }
        }

        private void ComboCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e) //atualiza variável quando altera valor da comboBox
        {
            categoriaSelected = ((ComboBoxItem)comboCategoria.SelectedItem).Tag.ToString();
        }

        private void ComboUserGeral_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchTypeSelected = ((ComboBoxItem)comboUserGeral.SelectedItem).Tag.ToString();
        }
    }
}
