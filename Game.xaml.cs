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
    /// 
    public interface IState<T>
    {
        void Avancar();
    }

    public class StartingGame : IState<Game>
    {
        private Game game;
        public StartingGame(Game game)
        {
            this.game = game;
        }

        public void Avancar()
        {
            game.StartGame();
            game.SetState(game.GetInGameState());
        }
    }

    public class InGame : Page, IState<Game>
    {
        private Game game;
        public InGame(Game game)
        {
            this.game = game;
        }

        public void Avancar()
        {
            if (game.EnunciadoQ == game.QuestionsNumber - 1)
            {
                game.SetState(game.GetEndState());
                return;
            }
            game.EnunciadoQ++;
            game.NextQuestion();
        }
    }

    public class EndOfGame : IState<Game>
    {
        private Game game;
        public EndOfGame(Game game)
        {
            this.game = game;
        }

        public void Avancar()
        {
            EndGame();
        }

        private async void EndGame()
        {
            StartLoadingCursor();
            await DatabaseAPI.PostData(BuildDataObject(), "Scores");
            StopLoadingCursor();
            MessageBox.Show("Dados adicionados com sucesso!!");
            game.PontuacaoGame = new PontuacaoGame(game, game.RespostasCertas, game.QuestionsNumber);
            game.NavigateToPage(game.PontuacaoGame);
        }

        private Dictionary<string, object> BuildDataObject()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Username", Login.Username},
                {"categoria", EscolherNumeroPerguntas.globalObj.IdCategoria},
                {"Pontuacao", game.Pontuacao }
            };

            return data;
        }
    }

    public class BackHome : Page, IState<Game>
    {
        private Game game;
        public BackHome(Game game)
        {
            this.game = game;
        }

        public void Avancar()
        {
            MessageBox.Show("Os dados desta sessão foram eliminados.");
            ClearGameStats();
            game.NavigateToPage(PageNavigation.optMenu);
        }

        private void ClearGameStats()
        {
            game.Enunciado.Questoes.Clear();
            game.CorrectAnswerPositionList.Clear();
            game.EnunciadoQ = 0;
            game.RespostasCertas = 0;
            game.Pontuacao = 0;
            game.Enunciado.PontuacaoMax = 0;
        }
    }

    public class NoQuestions : Page, IState<Game>
    {
        private Game game;
        public NoQuestions(Game game)
        {
            this.game = game;
        }

        public void Avancar()
        {
            MessageBox.Show("Lamentamos mas não possuímos perguntas suficientes para satisfazer o seu pedido, por favor tente novamente reduzindo o número de perguntas.");
            game.NavigateToPage(PageNavigation.optMenu);
        }
    }


    public partial class Game : Page
    {
        IState<Game> inGame;
        IState<Game> endOfGame;
        IState<Game> backHome;
        IState<Game> noQuestions;
        IState<Game> startingGame;

        IState<Game> state;

        DispatcherTimer dispatcherTimer;
        private int questionsNumber;

        private Enunciado enunciado;

        public Enunciado Enunciado
        {
            get { return enunciado; }
            set { enunciado = value; }
        }

        public int QuestionsNumber { 
            get { return questionsNumber; } 
            set { questionsNumber = value; }
        }

        private int pontuacao = 0;

        public int Pontuacao
        {
            get { return pontuacao; }
            set { pontuacao = value; }
        }

        private int respostasCertas = 0;

        public int RespostasCertas
        {
            get { return respostasCertas; }
            set { respostasCertas = value; }
        }

        private readonly static Random Rand = new Random();

        //número da pergunta que o utilizador se encontra
        private int enunciadoQ  = 0; 

        public int EnunciadoQ
        {
            get { return enunciadoQ;  }
            set { enunciadoQ = value;  }
        }

        bool hasPressed = false;

        private List<string> correctAnswerPositionList = new List<string>();

        public List<string> CorrectAnswerPositionList
        {
            get { return correctAnswerPositionList; }
            set { correctAnswerPositionList = value; }
        }

        private readonly static int CounterMax = 15;
        private static int CounterAtual = CounterMax;

        private readonly List<string> positions = new List<string>() { "A", "B", "C", "D" };

        private PontuacaoGame pontuacaoGame;

        public PontuacaoGame PontuacaoGame
        {
            get { return pontuacaoGame; }
            set { pontuacaoGame = value; }
        }

        public Game(Enunciado enunciado)
        {
            this.enunciado = enunciado;
            questionsNumber = this.enunciado.Questoes.Count;
            inGame = new InGame(this);
            endOfGame = new EndOfGame(this);
            backHome = new BackHome(this);
            noQuestions = new NoQuestions(this);
            startingGame = new StartingGame(this);

            if (questionsNumber == 0) state = noQuestions;
            else state = startingGame;

            System.Diagnostics.Debug.WriteLine(questionsNumber);

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;

            if (positions.Contains(objname) && !hasPressed) HandleButtonPress(objname);

            // other buttons which are not possible answers
            else
            {
                switch (objname)
                {

                    case "Start":
                    case "Next":
                        state.Avancar();
                        break;

                    case "HomeButton":
                        state = backHome;
                        state.Avancar();
                        break;
                }

            }
        }

        public void NavigateToPage(object obj)
        {
            NavigationService.Navigate(obj);
        }

        public void SetState(IState<Game> state)
        {
            this.state = state;
            if (state == endOfGame) state.Avancar();
        }

        public IState<Game> GetEndState()
        {
            return endOfGame;
        }

        public IState<Game> GetInGameState()
        {
            return inGame;
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

        public void StartGame()
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

        public void NextQuestion()
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
            for(int i = 0; i < questionsNumber; i++)
            {
                // adiciona a posição A,B,C ou D aleatoriamente numa lista 
                correctAnswerPositionList.Add(positions[Rand.Next(0,4)]); 
            }
        }
        

        private void ShowQuestion()
        {
            Next.Visibility = Visibility.Hidden;
            Question.Content = enunciado.Questoes[enunciadoQ].Value;
            if(enunciado.Questoes[enunciadoQ].Type == "boolean") ShowBooleanAnswer();
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

            Resposta primeiraResposta = enunciado.Questoes[enunciadoQ].Respostas[0];

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
            
            foreach ( Resposta resposta in enunciado.Questoes[enunciadoQ].Respostas) //percorre as respoostas da questão atual
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
                Pontuacao += enunciado.Questoes[enunciadoQ].Pontuacao;
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
