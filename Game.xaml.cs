using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para Game.xam
    /// </summary>
    public partial class Game : Page
    {
        static int pontuacao = 0;
        static Random rand = new Random();//DateTime.Now.ToString().GetHashCode()
        int enunciadoQ = 0; //número da pergunta que o utilizador se encontra
        bool hasPressed = false;
        int questionsNumber = 10; //este valor deve ser definido de acordo com o a escolha do utilizador, para teste coloquei 10
        List<string> correctAnswerPositionList = new List<string>();
        static List<Questao> Questoes = new List<Questao>();
        List<string> positions = new List<string>() { "A", "B", "C", "D" };
        class Resposta
        {
            public string value { get; set; }
            public bool correctAnswer { get; set; }
         //   public string answerPosition { get; set; }
            public Resposta(string value, bool correctAnswer)
            {
                this.value = value;
                this.correctAnswer = correctAnswer;
           //     this.answerPosition = answerPosition;
            }

        }
        class Questao
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
        public Game()
        {
            InitializeComponent();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;
            switch (objname)
            {
                case "A":
                    if (hasPressed == false)
                    {
                        VerifyAnswer("A");
                        Next.Visibility = Visibility.Visible;
                        hasPressed = true;
                    }
                    else
                    {
                        MessageBox.Show("Para de tentar aldrabar pallhaço");
                    }
                    break;
                case "B":
                    if (hasPressed == false)
                    {
                        VerifyAnswer("B");
                        Next.Visibility = Visibility.Visible;
                        hasPressed = true;
                    }
                    else
                    {
                        MessageBox.Show("Para de tentar aldrabar pallhaço");
                    }
                    break;
                case "C":
                    if (hasPressed == false)
                    {
                        VerifyAnswer("C");
                        Next.Visibility = Visibility.Visible;
                        hasPressed = true;
                    }
                    else
                    {
                        MessageBox.Show("Para de tentar aldrabar pallhaço");
                    }
                    break;
                case "D":
                    if (hasPressed == false)
                    {
                        VerifyAnswer("D");
                        Next.Visibility = Visibility.Visible;
                        hasPressed = true;
                    }
                    else
                    {
                        MessageBox.Show("Para de tentar aldrabar pallhaço");
                    }
                    break;
                case "Next":
                    enunciadoQ++;
                    EnableButtons();
                    if (enunciadoQ < questionsNumber)
                    {
                        hasPressed = false;
                        A.ClearValue(Button.BackgroundProperty);
                        B.ClearValue(Button.BackgroundProperty);
                        C.ClearValue(Button.BackgroundProperty);
                        D.ClearValue(Button.BackgroundProperty);
                      //  Enable_DisableButtons(true);
                        ShowQuestion();
                    }
                    else
                    {
                        MessageBox.Show("Acabou palhaço");
                    }
                    Next.Visibility = Visibility.Hidden;
                    break;
                case "Start":
                    await getData();
                    CreateRandomSequence();
                    Start.Visibility = Visibility.Hidden;
                    ShowQuestion();
                    A.Visibility = Visibility.Visible;
                    B.Visibility = Visibility.Visible;
                    C.Visibility = Visibility.Visible;
                    D.Visibility = Visibility.Visible;
                    Question.Visibility = Visibility.Visible;
                    break;
            }
        }
        static async Task getData()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://opentdb.com/api.php?amount=10&category=21");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            parseData(responseBody);
        }

        static void parseData(string data)
        {
            //string path = "C:/Users/lenovo/source/repos/QuizAppMethodsTest/TriviaTest.json"; //apenas para teste
            //var data1 = File.ReadAllText(path); //apenas para teste
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
                Questao questao = new Questao((string)result["question"], difficulty, ListaRespostas, (string)result["type"]);
                Questoes.Add(questao);
            }
        }

        private void CreateRandomSequence()
        {
            for(int i = 0; i < questionsNumber; i++)
            {
                correctAnswerPositionList.Add(positions[rand.Next(0,4)]);  //adiciona a posição A,B,C ou D aleatoriamente numa lista 
            }
        }
        

        private void ShowQuestion()
        {
            Question.Content = Questoes[enunciadoQ].value;
            if(Questoes[enunciadoQ].type == "boolean")
            {
                // ShowAnswer();
                ShowBooleanAnswer();
                //MessageBox.Show("Chaves é bolha");
            }
            else if (Questoes[enunciadoQ].type == "multiple")
            {
                if (C.IsVisible == false)
                {
                    C.Visibility = Visibility.Visible;
                    D.Visibility = Visibility.Visible;
                    ShowAnswer();
                }
                else
                {
                    ShowAnswer();
                }
//                MessageBox.Show("Leandro é bolha");
            }
        }

        private void ShowBooleanAnswer()
        {
            C.Visibility = Visibility.Hidden;
            D.Visibility = Visibility.Hidden;
            A.Content = "True";
            B.Content = "False";
            for (int i = 0; i < 1; i++)
            {
                if(Questoes[enunciadoQ].Respostas[i].correctAnswer == true)
                {
                    if (Questoes[enunciadoQ].Respostas[i].value== "True") //se a resposta correta for true, afirmo que o botão A possui a resposta correta
                    {
                        correctAnswerPositionList[enunciadoQ] = "A";
                    }
                    else
                    {
                        correctAnswerPositionList[enunciadoQ] = "B";
                    }
                }
            }
        }

        private void ShowAnswer()
        {
            int j = 0;
            string correctPosition = correctAnswerPositionList[enunciadoQ]; //para teste, este valor deverá ser atribuído randomicamente
            List<string> Incorretpositions = positions.ToList();
            Incorretpositions.Remove(correctPosition); //lista das resposta incorretas
            foreach ( Resposta resposta in Questoes[enunciadoQ].Respostas) //percorre as respoostas da questão atual
            {
                if (resposta.correctAnswer == true) //mostrar resposta correta
                {
                    switch (correctPosition)
                    {
                        case "A":
                            A.Content = resposta.value;
                            break;
                        case "B":
                            B.Content = resposta.value;
                            break;
                        case "C":
                            C.Content = resposta.value;
                            break;
                        case "D":
                            D.Content = resposta.value;
                            break;
                    }
                }
                else //mostrar resposta incorreta
                {
                    if (j <= 2)
                    {
                        ShowIncorretAnswer(resposta, Incorretpositions, j);
                        j++;
                    }
                }
            }
        }

        private void ShowIncorretAnswer(Resposta resposta, List<string> Incorretpositions, int j)
        {
            switch (Incorretpositions[j])
            {
                case "A":
                    A.Content = resposta.value;
                    break;
                case "B":
                    B.Content = resposta.value;
                    break;
                case "C":
                    C.Content = resposta.value;
                    break;
                case "D":
                    D.Content = resposta.value;
                    break;
            }
        }

        private void VerifyAnswer(string selectedAnswer)
        {
            string correctPosition = correctAnswerPositionList[enunciadoQ];
            switch (correctPosition)
            {
                case "A":
                    A.Background = Brushes.Green;
                    if (selectedAnswer != correctPosition) 
                    {
                        VerifyAnswerAux(selectedAnswer);
                    }
                    else //atribuir pontuação
                    {
                        pontuacao = pontuacao + Questoes[enunciadoQ].pontuacao;
                    }
                    break;
                case "B":
                    B.Background = Brushes.Green;
                    if (selectedAnswer != correctPosition) 
                    {
                        VerifyAnswerAux(selectedAnswer); //caso resposta esteja errada coloca a escolhida a vermelho
                    }
                    else //atribuir pontuação
                    {
                        pontuacao = pontuacao + Questoes[enunciadoQ].pontuacao;
                    }
                    break;
                case "C":
                    C.Background = Brushes.Green;
                    if (selectedAnswer != correctPosition) 
                    {
                        VerifyAnswerAux(selectedAnswer);
                    }
                    else //atribuir pontuação
                    {
                        pontuacao = pontuacao + Questoes[enunciadoQ].pontuacao;
                    }
                    break;
                case "D":
                    D.Background = Brushes.Green;
                    if (selectedAnswer != correctPosition) 
                    {
                        VerifyAnswerAux(selectedAnswer);
                    }
                    else //atribuir pontuação
                    {
                        pontuacao = pontuacao + Questoes[enunciadoQ].pontuacao;
                    }
                    break;
            }
            DisableButtons(selectedAnswer, correctPosition);
            Score.Content =  pontuacao;
        }
        private void VerifyAnswerAux(string selectedAnswer)
        {
            switch (selectedAnswer) 
            {
                case "A":
                    A.Background = Brushes.Red;
                    break;
                case "B":
                    B.Background = Brushes.Red;
                    break;
                case "C":
                    C.Background = Brushes.Red;
                    break;
                case "D":
                    D.Background = Brushes.Red;
                    break;
            }
        }

        private void EnableButtons()
        {
            A.IsEnabled = true;
            B.IsEnabled = true;
            C.IsEnabled = true;
            D.IsEnabled = true;
        }

        private void DisableButtons(string selectedanswser, string correctanswer)
        {
            foreach (string i in positions)
            {
                switch (i)
                {
                    case "A":
                        if (i != selectedanswser && i != correctanswer)
                        {
                            A.IsEnabled = false;
                        }
                        break;
                    case "B":
                        if (i != selectedanswser && i != correctanswer)
                        {
                            B.IsEnabled = false;
                        }
                        break;
                    case "C":
                        if (i != selectedanswser && i != correctanswer)
                        {
                            C.IsEnabled = false;
                        }
                        break;
                    case "D":
                        if (i != selectedanswser && i != correctanswer)
                        {
                            D.IsEnabled = false;
                        }
                        break;
                }
            }
        }
    }
}
