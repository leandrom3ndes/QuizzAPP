using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
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
        private string idCategoria { get; set; }
        private string dificuldade { get; set; }
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
            logUrl.Content = url;

            string result = await getData(url);


            Enunciado.parseData(result);


            Game openGame = new Game(Int32.Parse(numeroPerguntas));
            this.NavigationService.Navigate(openGame);
            System.Diagnostics.Debug.WriteLine(result);

        }
        private string atualizaURL(string nrPerguntas, string idCategoria, string dificuldade)
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
