using System;
using System.Threading.Tasks;
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
        public Total_EscolherNumeroPerguntas Obj { get; set; }
        public static Total_EscolherNumeroPerguntas globalObj;

        public EscolherNumeroPerguntas(string idCategoria, string dificuldade)
        {
            Obj = new Total_EscolherNumeroPerguntas(idCategoria, dificuldade);
            globalObj = Obj;
            InitializeComponent();
        }

        private async void NumeroPerguntasEscolhida(object sender, RoutedEventArgs e)
        {
            string numeroPerguntas = await Obj.NumeroPerguntasEscolhida(sender, e);
            Game openGame = new Game(int.Parse(numeroPerguntas));
            NavigationService.Navigate(openGame);
        }

    }

    // for TDD
    public class Total_EscolherNumeroPerguntas
    {

        public string IdCategoria { get; set; }
        public string Dificuldade { get; set; }

        public Total_EscolherNumeroPerguntas(string idCategoria, string dificuldade)
        {
            IdCategoria = idCategoria;
            Dificuldade = dificuldade;
        }
        public async Task<string> NumeroPerguntasEscolhida(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            string numeroPerguntas = senderButton.Name.Remove(0, 1);

            string url = AtualizaURL(numeroPerguntas, IdCategoria, Dificuldade);

            string result = await GetData(url);
            _ = new Enunciado(result);

            return numeroPerguntas;
        }
        public string AtualizaURL(string nrPerguntas, string idCategoria, string dificuldade)
        {
            bool isNumber = int.TryParse(nrPerguntas, out int _);
            bool isNumberFromCat = int.TryParse(idCategoria, out int _);

            if (!isNumber || !isNumberFromCat) throw new ArgumentException("Número inválido!");

            string url = ReplaceString(BaseUrl, "REPLACENUMBER", nrPerguntas);

            url = ReplaceString(url, "REPLACE_CATEGORY", idCategoria);

            if (dificuldade != "random") url += "&difficulty=" + dificuldade;
            return url;
        }
    }
}
