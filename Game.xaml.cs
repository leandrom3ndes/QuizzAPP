using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using Google.Cloud.Firestore;
using static GlobalMethods.GlobalMethods;

namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para Game.xam
    /// </summary>
    public partial class Game : Page
    {

        DispatcherTimer dispatcherTimer;
        private int QuestionsNumber { get; set; }
        private static int Pontuacao = 0;
        private static int RespostasCertas = 0;
        private readonly static Random Rand = new Random();
        private static int enunciadoQ = 0; //número da pergunta que o utilizador se encontra
        bool hasPressed = false;
        public static List<string> correctAnswerPositionList = new List<string>();

        private readonly static int CounterMax = 15;
        private static int CounterAtual = CounterMax;

        private readonly List<string> positions = new List<string>() { "A", "B", "C", "D" };

        public Game(int QuestionsNumber)
        {
            this.QuestionsNumber = QuestionsNumber;
            InitializeComponent();
        }

        private static async Task<Task> Pontuacao_usernameID()
        {
            CollectionReference coll = firedatabase.Collection("Scores");
            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                {"Username", Login.username},
                {"categoria", EscolherNumeroPerguntas.globalObj.IdCategoria},
                {"Pontuacao", Pontuacao }
            };
            await coll.AddAsync(data1);
            MessageBox.Show("Dados adicionados com sucesso!!");

            return Task.CompletedTask;
        }


        private void StartTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(TimerCountdown);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            TimerLabel.Width = 800; //volta à largura inicial
            var converter = new BrushConverter();
            TimerLabel.Background = (Brush)converter.ConvertFromString("#FF8BC5B0"); //volta à cor inicial
            ModifyAllButtons("enable-disable", "true");
        }

        private void TimerCountdown(object sender, EventArgs e)
        {
            CounterAtual--;
            TimerLabel.Width = CounterAtual * 800 / CounterMax;
            if (CounterAtual <= 10 && CounterAtual > 5) TimerLabel.Background = Brushes.Yellow;
            if (CounterAtual <=5) TimerLabel.Background = Brushes.Red;
            if (CounterAtual == 0) CounterTimeout(); 
        }

        private void CounterTimeout()
        {
            StopCounter();
            ModifyAllButtons("enable-disable", "false");
            Next.Visibility = Visibility.Visible;
        }

        private void StopCounter()
        {
            dispatcherTimer.Stop();
            CounterAtual = CounterMax;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ( (Button) sender ).Name; 

            if ( positions.Contains(objname) && !hasPressed ) HandleButtonPress( objname ); 

            // other buttons which are not possible answers
            else
            {
                switch ( objname )
                {
                    case "Next":
                        enunciadoQ++;
                        ModifyAllButtons("enable-disable", "true");
                        Next.Visibility = Visibility.Hidden;

                        if (enunciadoQ < QuestionsNumber) NextQuestion();

                        else GameEnded();

                        break;

                    case "Start":
                        StartGame();
                        break;

                    case "HomeButton":
                        HomeButtonPressed();
                        break;
                }

            }
        }
        
        private void HomeButtonPressed()
        {
            if ( dispatcherTimer != null ) StopCounter();
            MessageBox.Show("Os dados desta sessão foram eliminados.");
            PontuacaoGame.ClearStats();
            OptionMenu optionMenu1 = new OptionMenu();
            NavigationService.Navigate(optionMenu1);
        }

        private async void GameEnded()
        {
            // MessageBox.Show("A sua sessão chegou ao fim. Pressione Ok para descobrir a pontuação que obteve");
            StartLoadingCursor();
            await Pontuacao_usernameID();
            StopLoadingCursor();
            PontuacaoGame pontGame = new PontuacaoGame(RespostasCertas, QuestionsNumber);
            NavigationService.Navigate(pontGame);
        }

        private void StartGame()
        {
            CreateRandomSequence();
            Start.Visibility = Visibility.Hidden;
            ShowQuestion();
            A.Visibility = Visibility.Visible;
            B.Visibility = Visibility.Visible;
            Question.Visibility = Visibility.Visible;
            TimerLabel.Visibility = Visibility.Visible;
            StartTimer();
        }

        private void NextQuestion()
        {
            hasPressed = false;
            ModifyAllButtons("clear-background");
            ShowQuestion();
            StartTimer();
        }

        private void HandleButtonPress(string buttonPressed)
        {
            StopCounter();
            VerifyAnswer( buttonPressed );
            Next.Visibility = Visibility.Visible;
            hasPressed = true;
            
        }

        private void CreateRandomSequence()
        {
            for(int i = 0; i < QuestionsNumber; i++)
            {
                correctAnswerPositionList.Add(positions[Rand.Next(0,4)]);  //adiciona a posição A,B,C ou D aleatoriamente numa lista 
            }
        }
        

        private void ShowQuestion()
        {
            Question.Content = Enunciado.Questoes[enunciadoQ].Value;
            if(Enunciado.Questoes[enunciadoQ].Type == "boolean") ShowBooleanAnswer();
            else
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

            ModifyDynamicButton( "A" , "new-value", "True");
            ModifyDynamicButton( "B", "new-value", "False" );

            Resposta primeiraResposta = Enunciado.Questoes[enunciadoQ].Respostas[0];

            // se a resposta correta for true, afirmo que o botão A possui a resposta correta
            if ( primeiraResposta.CorrectAnswer)  correctAnswerPositionList[enunciadoQ] = "A"; 
            
            else  correctAnswerPositionList[enunciadoQ] = "B"; 

        }

        private void ShowAnswer()
        {
            int j = 0;
            string correctPosition = correctAnswerPositionList[enunciadoQ]; //para teste, este valor deverá ser atribuído randomicamente
            List<string> Incorretpositions = positions.ToList();
            Incorretpositions.Remove(correctPosition); //lista das resposta incorretas
            
            foreach ( Resposta resposta in Enunciado.Questoes[enunciadoQ].Respostas) //percorre as respoostas da questão atual
            {
                //mostrar resposta correta
                if (resposta.CorrectAnswer) ModifyDynamicButton(correctPosition, "new-value", resposta.Value);

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
            ModifyDynamicButton(Incorretpositions[j], "new-value", resposta.Value);

        }

        private void VerifyAnswer(string selectedAnswer)
        {
            string correctPosition = correctAnswerPositionList[enunciadoQ];

            // green background on correct button
            ModifyDynamicButton(correctPosition, "green-background");

            if (selectedAnswer != correctPosition) ModifyDynamicButton(selectedAnswer, "red-background");
            //atribuir pontuação
            else
            {
                Pontuacao += Enunciado.Questoes[enunciadoQ].Pontuacao;
                RespostasCertas += 1;
            }

            DisableButtons(selectedAnswer, correctPosition);

        }

        private void ModifyAllButtons( string action, string value = null )
        {
            foreach (string buttonName in positions)
            {
                ModifyDynamicButton(buttonName, action, value);
            }
        }

        private void DisableButtons(string selectedanswser, string correctanswer)
        {
            foreach (string i in positions)
            {
                if (i != selectedanswser && i != correctanswer) ModifyDynamicButton(i, "enable-disable", "false");
                else ModifyDynamicButton(i, "enable-disable-click", "false" );
            }
        }

        private void ModifyDynamicButton(string name, string action, string value = null)
        {
            object dynamicObject = FindName(name);
            Button dynamicButton = GetButton(dynamicObject);

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
                    // make button clickable when it gets a new value
                    dynamicButton.IsHitTestVisible = true;
                    break;

                case "enable-disable":
                    dynamicButton.IsEnabled = bool.Parse(value);
                    break;

                case "clear-background":
                    dynamicButton.ClearValue(BackgroundProperty);
                    break;
                case "enable-disable-click":
                    dynamicButton.IsHitTestVisible = bool.Parse(value);
                    break;
            }

        }

        private Button GetButton(object buttonObject)
        {
            return ((Button)buttonObject);
        }

    }
}
