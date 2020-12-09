using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text.RegularExpressions;

namespace QuizAppWPF
{
    /// <summary>
    /// Interaction logic for EscolherCategoria.xaml
    /// </summary>
    public partial class EscolherCategoria : Page
    {
        public EscolherCategoria()
        {
            InitializeComponent();
        }

        class Resposta
        {
            public string value { get; set; }
            public bool correctAnswer { get; set; }
            public Resposta(string value, bool correctAnswer)
            {
                this.value = value;
                this.correctAnswer = correctAnswer;
            }

        }
        class Questao
        {
            public string value { get; set; }
            public int pontuacao { get; set; }
            public List<Resposta> Respostas { get; set; }
            public Questao(string value, int pontuacao, List<Resposta> Respostas)
            {
                this.value = value;
                this.pontuacao = pontuacao;
                this.Respostas = Respostas;
            }
        }

        static int pontuacao = 0;

        static List<Questao> Questoes = new List<Questao>();

        private static string BaseUrl = "https://opentdb.com/api.php?amount=10&category=REPLACE_CATEGORY&difficulty=easy";

        private static string categoriesUrl = "https://opentdb.com/api_category.php";

        private static IDictionary<int, int> categories = new Dictionary<int, int>();


        static async Task<string> getData(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        static void parseQuizzData(string data)
        {
            JObject jobject = (JObject)JsonConvert.DeserializeObject(data);
            foreach (JObject result in jobject["results"])
            {

                List<Resposta> ListaRespostas = new List<Resposta>();

                // get correct answer
                ListaRespostas.Add(new Resposta((string)result["correct_answer"], true));

                // get remaining answers
                foreach (string wrongAnswer in result["incorrect_answers"])
                {
                    ListaRespostas.Add(new Resposta(wrongAnswer, false));
                }

                // instantiate question ( with answers )
                Questao questao = new Questao((string)result["question"], 4, ListaRespostas);

                Questoes.Add(questao);
            }
        }

        private async void CategoriaEscolhida(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            string idCategoria = senderButton.Name.Remove(0, 2);

            Regex rgx = new Regex("REPLACE_CATEGORY");
            string url = rgx.Replace(BaseUrl, idCategoria);

            string result = await getData(url);


            senderButton.Content = result;

        }
    }
}
