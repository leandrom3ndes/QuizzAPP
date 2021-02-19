using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuizAppWPF
{
    /// <summary>
    /// Interaction logic for EscolherNumeroPerguntas.xaml
    /// </summary>
    public partial class EscolherNumeroPerguntas : Page
    {
        public Total_EscolherNumeroPerguntas Obj { get; set; }
        public static Total_EscolherNumeroPerguntas globalObj;
        private Enunciado enunciado;

        public EscolherNumeroPerguntas(string idCategoria, string dificuldade)
        {
            Obj = new Total_EscolherNumeroPerguntas(idCategoria, dificuldade);
            globalObj = Obj;
            InitializeComponent();
        }

        private async void NumeroPerguntasEscolhida(object sender, RoutedEventArgs e)
        {
            enunciado = await Obj.NumeroPerguntasEscolhida(sender, e);
            Game openGame = new Game(enunciado);
            NavigationService.Navigate(openGame);
        }

    }

    // for TDD
    public class Total_EscolherNumeroPerguntas
    {

        public string IdCategoria { get; set; }
        public string Dificuldade { get; set; }

        private Enunciado enunciado;

        public Total_EscolherNumeroPerguntas(string idCategoria, string dificuldade)
        {
            IdCategoria = idCategoria;
            Dificuldade = dificuldade;
        }
        public async Task<Enunciado> NumeroPerguntasEscolhida(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            string numeroPerguntas = senderButton.Name.Remove(0, 1);

            string url = AtualizaURL(numeroPerguntas, IdCategoria, Dificuldade);

            string result = await TriviaInteractionAPI.GetData(url);
            enunciado = new Enunciado(result);

            return enunciado;
        }
        public string AtualizaURL(string nrPerguntas, string idCategoria, string dificuldade)
        {
            bool isNumber = int.TryParse(nrPerguntas, out int _);
            bool isNumberFromCat = int.TryParse(idCategoria, out int _);

            if (!isNumber || !isNumberFromCat) throw new ArgumentException("Número inválido!");

            string url = TriviaInteractionAPI.BaseUrl.Replace("REPLACENUMBER", nrPerguntas);

            url = url.Replace("REPLACE_CATEGORY", idCategoria);

            if (dificuldade != "random") url += "&difficulty=" + dificuldade;
            return url;
        }
    }
}
