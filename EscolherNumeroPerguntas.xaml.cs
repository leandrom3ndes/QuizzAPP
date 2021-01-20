using System;
using System.Windows;
using System.Windows.Controls;
using static GlobalMethods.GlobalMethods;

namespace QuizAppWPF
{
    /// <summary>
    /// Interaction logic for EscolherNumeroPerguntas.xaml
    /// </summary>
    public partial class EscolherNumeroPerguntas : Page
    {
        public string idCategoria { get; set; }
        public string dificuldade { get; set; }
        public EscolherNumeroPerguntas(string idCategoria, string dificuldade)
        {
            this.idCategoria = idCategoria;
            this.dificuldade = dificuldade;
            InitializeComponent();
        }

        //static int pontuacao = 0;
        

        private async void NumeroPerguntasEscolhida(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            string numeroPerguntas = senderButton.Name.Remove(0, 1);

            string url = atualizaURL(numeroPerguntas, idCategoria , dificuldade);

            string result = await getData(url);

            Enunciado.parseData(result);

            Game openGame = new Game(Int32.Parse(numeroPerguntas));
            this.NavigationService.Navigate(openGame);

        }
        public string atualizaURL(string nrPerguntas, string idCategoria, string dificuldade)
        {
            

            string url = getReplaceRegex(BaseUrl, "REPLACENUMBER", nrPerguntas);

            url = getReplaceRegex(url, "REPLACE_CATEGORY", idCategoria);

            if (dificuldade != "random")
            {
                url = url + "&difficulty=" + dificuldade;
            }
            return url;
        }
    }
}
