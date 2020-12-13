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
using System.Timers;
using System.Windows.Threading;

namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para Game.xam
    /// </summary>
    public partial class Game : Page
    {
        DispatcherTimer dispatcherTimer;
        private int questionsNumber { get; set; }
        int pontuacao = 0;
        static Random rand = new Random();//DateTime.Now.ToString().GetHashCode()
        int enunciadoQ = 0; //número da pergunta que o utilizador se encontra
        bool hasPressed = false;
        public static List<string> correctAnswerPositionList = new List<string>();

        static int counterMax = 15;
        static int counterAtual = counterMax;

        List<string> positions = new List<string>() { "A", "B", "C", "D" };

        public Game(int questionsNumber)
        {
            this.questionsNumber = questionsNumber;
            InitializeComponent();
        }

        private void startTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(timerCountdown);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            TimerLabel.Width = 800;
            EnableDisableButtons(true);
        }

        private void timerCountdown(object sender, EventArgs e)
        {
            counterAtual--;
            TimerLabel.Width = counterAtual * 800 / counterMax;
            if (counterAtual == 0) { counterTimeout(); }
        }

        private void counterTimeout()
        {
            stopCounter();
            EnableDisableButtons(false);
            Next.Visibility = Visibility.Visible;
        }

        private void stopCounter()
        {
            dispatcherTimer.Stop();
            counterAtual = counterMax;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ( (Button) sender ).Name; 

            if ( positions.Contains( objname ) )
            {
                if ( !hasPressed ) { handleButtonPress( objname ); }
                else { MessageBox.Show("Para de tentar aldrabar pallhaço"); }
            }

            // other buttons which are not possible answers
            else
            {
                switch ( objname )
                {
                    case "Next":
                        enunciadoQ++;
                        EnableDisableButtons(true);
                        Next.Visibility = Visibility.Hidden;

                        if (enunciadoQ < questionsNumber) { nextQuestion();}

                        else { gameEnded(); }

                        break;

                    case "Start":
                        startGame();
                        break;

                    case "HomeButton":
                        HomeButtonPressed();
                        break;
                }

            }
        }
        
        private void HomeButtonPressed()
        {
            stopCounter();
            MessageBox.Show("Os dados desta sessão foram eliminados.");
            PontuacaoGame.ClearStats();
            OptionMenu optionMenu1 = new OptionMenu();
            NavigationService.Navigate(optionMenu1);
        }

        private void gameEnded()
        {
            MessageBox.Show("Acabou palhaço");
            PontuacaoGame pontGame = new PontuacaoGame(Enunciado.pontuacaoMax, pontuacao);
            NavigationService.Navigate(pontGame);
        }

        private void startGame()
        {
            CreateRandomSequence();
            Start.Visibility = Visibility.Hidden;
            ShowQuestion();
            A.Visibility = Visibility.Visible;
            B.Visibility = Visibility.Visible;
            Question.Visibility = Visibility.Visible;
            TimerLabel.Visibility = Visibility.Visible;
            startTimer();
        }

        private void nextQuestion()
        {
            hasPressed = false;
            modifyAllButtons("clear-background");
            ShowQuestion();
            startTimer();
        }

        private void handleButtonPress(string buttonPressed)
        {
            stopCounter();
            VerifyAnswer( buttonPressed );
            Next.Visibility = Visibility.Visible;
            hasPressed = true;
            
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
                ShowBooleanAnswer();
            }
            else if (Enunciado.Questoes[enunciadoQ].type == "multiple")
            {
                if (!C.IsVisible)
                {
                    C.Visibility = Visibility.Visible;
                    D.Visibility = Visibility.Visible;
                }

                ShowAnswer();
            }
        }

        private void ShowBooleanAnswer()
        {
            C.Visibility = Visibility.Hidden;
            D.Visibility = Visibility.Hidden;
            A.Content = "True";
            B.Content = "False";

            Resposta primeiraResposta = Enunciado.Questoes[enunciadoQ].Respostas[0];

            // se a resposta correta for true, afirmo que o botão A possui a resposta correta
            if ( primeiraResposta.correctAnswer) { correctAnswerPositionList[enunciadoQ] = "A"; }
            
            else { correctAnswerPositionList[enunciadoQ] = "B"; }

        }

        private void ShowAnswer()
        {
            int j = 0;
            string correctPosition = correctAnswerPositionList[enunciadoQ]; //para teste, este valor deverá ser atribuído randomicamente
            List<string> Incorretpositions = positions.ToList();
            Incorretpositions.Remove(correctPosition); //lista das resposta incorretas
            
            foreach ( Resposta resposta in Enunciado.Questoes[enunciadoQ].Respostas) //percorre as respoostas da questão atual
            {
                if (resposta.correctAnswer) //mostrar resposta correta
                {
                    modifyDynamicButton(correctPosition, "new-value", resposta.value);
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

        // displays the content of all incorrect answers
        private void ShowIncorretAnswer(Resposta resposta, List<string> Incorretpositions, int j)
        {
            modifyDynamicButton(Incorretpositions[j], "new-value", resposta.value);

        }

        private void VerifyAnswer(string selectedAnswer)
        {
            string correctPosition = correctAnswerPositionList[enunciadoQ];

            // green background on correct button
            modifyDynamicButton(correctPosition, "green-background");

            if (selectedAnswer != correctPosition)
            {
                modifyDynamicButton(selectedAnswer, "red-background");
            }
            else //atribuir pontuação
            {
                pontuacao += Enunciado.Questoes[enunciadoQ].pontuacao;
            }
            
            DisableButtons(selectedAnswer, correctPosition);

        }

        private void EnableDisableButtons(bool value)
        {
            modifyAllButtons("enable-disable", value.ToString());
        }

        private void modifyAllButtons( string action, string value = null )
        {
            foreach (string buttonName in positions)
            {
                modifyDynamicButton(buttonName, action, value);
            }
        }

        private void DisableButtons(string selectedanswser, string correctanswer)
        {
            foreach (string i in positions)
            {
                if (i != selectedanswser && i != correctanswer)
                {
                    modifyDynamicButton(i, "enable-disable", "false");
                }
            }
        }

        private void modifyDynamicButton(string name, string action, string value = null)
        {
            object dynamicObject = FindName(name);
            Button dynamicButton = getButton(dynamicObject);

            switch ( action )
            {
                case "red-background":
                    dynamicButton.Background = Brushes.Red;
                    break;

                case "green-background":
                    dynamicButton.Background = Brushes.Green;
                    break;

                case "new-value":
                    dynamicButton.Content = value;
                    break;

                case "enable-disable":
                    dynamicButton.IsEnabled = bool.Parse(value);
                    break;

                case "clear-background":
                    dynamicButton.ClearValue(BackgroundProperty);
                    break;

            }

        }

        private Button getButton(object buttonObject)
        {
            return ((Button)buttonObject);
        }

    }
}
