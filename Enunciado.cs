using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace QuizAppWPF
{

    public class Enunciado
    {

        public static Score scoreMenu = new Score();
        public static EscolherCategoria pageCategoria = new EscolherCategoria();
        public static Login loginMenu = new Login();
        //podemos colocar aqui todas as instancias das navegações, deixei o do login como exemplo

        private readonly List<Questao> questoes = new List<Questao>();

        public List<Questao> Questoes
        {
            get { return questoes;  }
        }

        private int pontuacaoMax = 0;

        public int PontuacaoMax
        {
            get { return pontuacaoMax; }
            set { pontuacaoMax = value; }
        }

        private readonly static IDictionary<string, int> DifficultyValues = new Dictionary<string, int>() {
            { "easy", 1 },
            { "medium", 2 },
            { "hard", 4 }
        };

        public Enunciado( string data )
        {
            ParseData(data);
        }

        public void ParseData( string data )
        {
            JObject jobject = (JObject)JsonConvert.DeserializeObject(data);
            foreach (JObject result in jobject["results"])
            {
                List<Resposta> ListaRespostas = new List<Resposta>
                {
                    // get correct answer
                    new Resposta(System.Net.WebUtility.HtmlDecode((string)result["correct_answer"]), true)
                };

                // get remaining answers
                foreach (string wrongAnswer in result["incorrect_answers"]) ListaRespostas.Add(new Resposta(System.Net.WebUtility.HtmlDecode(wrongAnswer), false));

                // instantiate question ( with answers )  
                int difficulty = DifficultyValues[(string)result["difficulty"]];
                Questao questao = new Questao(System.Net.WebUtility.HtmlDecode((string)result["question"]), difficulty, ListaRespostas, (string)result["type"]);
                questoes.Add(questao);
                pontuacaoMax += difficulty;

            }
        }
    }
    public class Resposta
    {
        public string Value { get; set; }
        public bool CorrectAnswer { get; set; }
        public Resposta(string value, bool correctAnswer)
        {
            Value = value;
            CorrectAnswer = correctAnswer;
        }

    }
    public class Questao
    {
        public string Value { get; set; }
        public int Pontuacao { get; set; }
        public string Type { get; set; }
        public List<Resposta> Respostas { get; set; }
        public Questao(string value, int pontuacao, List<Resposta> Respostas, string type)
        {
            Value = value;
            Pontuacao = pontuacao;
            this.Respostas = Respostas;
            Type = type;
        }
    }
}
