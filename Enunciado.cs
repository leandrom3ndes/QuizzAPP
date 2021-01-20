using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace QuizAppWPF
{

    public class Enunciado
    {
        public static List<Questao> Questoes = new List<Questao>();
        public static int pontuacaoMax = 0;
        public static Score scoreMenu = new Score();
        public static EscolherCategoria pageCategoria = new EscolherCategoria();
        public static Login loginMenu = new Login();
        //podemos colocar aqui todas as instancias das navegações, deixei o do login como exemplo
        public static void parseData(string data)
        {
            JObject jobject = (JObject)JsonConvert.DeserializeObject(data);
            foreach (JObject result in jobject["results"])
            {
                List<Resposta> ListaRespostas = new List<Resposta>();
                // get correct answer
                ListaRespostas.Add(new Resposta(System.Net.WebUtility.HtmlDecode((string)result["correct_answer"]), true));
                // get remaining answers
                foreach (string wrongAnswer in result["incorrect_answers"])
                {
                    ListaRespostas.Add(new Resposta(System.Net.WebUtility.HtmlDecode(wrongAnswer), false));
                }
                // instantiate question ( with answers )
                int difficulty = 0;
                switch ((string)result["difficulty"])
                {
                    case "easy":
                        difficulty = 1;
                        break;
                    case "medium":
                        difficulty = 2;
                        break;
                    case "hard":
                        difficulty = 4;
                        break;

                }
                Questao questao = new Questao(System.Net.WebUtility.HtmlDecode((string)result["question"]), difficulty, ListaRespostas, (string)result["type"]);
                Questoes.Add(questao);
                pontuacaoMax = pontuacaoMax + difficulty;

            }
        }
    }
    public class Resposta
    {
        public string value { get; set; }
        public bool correctAnswer { get; set; }
        public Resposta(string value, bool correctAnswer)
        {
            this.value = value;
            this.correctAnswer = correctAnswer;
        }

    }
    public class Questao
    {
        public string value { get; set; }
        public int pontuacao { get; set; }
        public string type { get; set; }
        public List<Resposta> Respostas { get; set; }
        public Questao(string value, int pontuacao, List<Resposta> Respostas, string type)
        {
            this.value = value;
            this.pontuacao = pontuacao;
            this.Respostas = Respostas;
            this.type = type;
        }
    }


}
