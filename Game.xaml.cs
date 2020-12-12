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
        private int questionsNumber { get; set; }
        int pontuacao = 0;
        static Random rand = new Random();//DateTime.Now.ToString().GetHashCode()
        int enunciadoQ = 0; //número da pergunta que o utilizador se encontra
        bool hasPressed = false;
        public static List<string> correctAnswerPositionList = new List<string>();

        List<string> positions = new List<string>() { "A", "B", "C", "D" };

        public Game(int questionsNumber)
        {
            this.questionsNumber = questionsNumber;
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
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
                        ShowQuestion();
                    }
                    else
                    {
                        MessageBox.Show("Acabou palhaço");
                        PontuacaoGame pontGame = new PontuacaoGame(Enunciado.pontuacaoMax, pontuacao);
                        this.NavigationService.Navigate(pontGame);
                    }
                    Next.Visibility = Visibility.Hidden;
                    break;
                case "Start":
                 //   await getData();
                    CreateRandomSequence();
                    Start.Visibility = Visibility.Hidden;
                    ShowQuestion();
                    A.Visibility = Visibility.Visible;
                    B.Visibility = Visibility.Visible;
                    Question.Visibility = Visibility.Visible;
                    break;
                case "HomeButton":
                    MessageBox.Show("Os dados desta sessão foram eliminados.");
                    PontuacaoGame.ClearStats();
                    OptionMenu optionMenu1 = new OptionMenu();
                    this.NavigationService.Navigate(optionMenu1);
                    break;
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
            Question.Content = Enunciado.Questoes[enunciadoQ].value;
            if(Enunciado.Questoes[enunciadoQ].type == "boolean")
            {
                // ShowAnswer();
                ShowBooleanAnswer();
                //MessageBox.Show("Chaves é bolha");
            }
            else if (Enunciado.Questoes[enunciadoQ].type == "multiple")
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
                if(Enunciado.Questoes[enunciadoQ].Respostas[i].correctAnswer == true)
                {
                    if (Enunciado.Questoes[enunciadoQ].Respostas[i].value== "True") //se a resposta correta for true, afirmo que o botão A possui a resposta correta
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
            foreach ( Resposta resposta in Enunciado.Questoes[enunciadoQ].Respostas) //percorre as respoostas da questão atual
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
                        pontuacao = pontuacao + Enunciado.Questoes[enunciadoQ].pontuacao;
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
                        pontuacao = pontuacao + Enunciado.Questoes[enunciadoQ].pontuacao;
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
                        pontuacao = pontuacao + Enunciado.Questoes[enunciadoQ].pontuacao;
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
                        pontuacao = pontuacao + Enunciado.Questoes[enunciadoQ].pontuacao;
                    }
                    break;
            }
            DisableButtons(selectedAnswer, correctPosition);
           
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
