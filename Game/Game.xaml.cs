using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

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
        private Game _game;
        public StartingGame(Game game)
        {
            this._game = game;
        }

        public void Avancar()
        {
            _game.StartGame();
            _game.SetState(_game.GetInGameState());
        }
    }

    public class InGame : Page, IState<Game>
    {
        private Game _game;
        public InGame(Game game)
        {
            this._game = game;
        }

        public void Avancar()
        {
            if (_game.EnunciadoQ == _game.QuestionsNumber - 1)
            {
                _game.SetState(_game.GetEndState());
                return;
            }
            _game.EnunciadoQ++;
            _game.NextQuestion();
        }
    }

    public class EndOfGame : IState<Game>
    {
        private Game _game;
        public EndOfGame(Game game)
        {
            this._game = game;
        }

        public void Avancar()
        {
            EndGame();
        }

        private async void EndGame()
        {
            LoadingCursor.StartLoadingCursor();
            await DatabaseAPI.PostData(BuildDataObject(), "Scores");
            LoadingCursor.StopLoadingCursor();
            MessageBox.Show("Dados adicionados com sucesso!!");
            _game.PontuacaoGame = new PontuacaoGame(_game, _game.RespostasCertas, _game.QuestionsNumber);
            _game.NavigateToPage(_game.PontuacaoGame);
        }

        private Dictionary<string, object> BuildDataObject()
        {
            Dictionary<string, object> Data = new Dictionary<string, object>()
            {
                {"Username", Login.Username},
                {"categoria", EscolherNumeroPerguntas.globalObj.IdCategoria},
                {"Pontuacao", _game.Pontuacao }
            };
            
            return Data;
        }
    }

    public class BackHome : Page, IState<Game>
    {
        private Game _game;
        public BackHome(Game game)
        {
            this._game = game;
        }

        public void Avancar()
        {
            MessageBox.Show("Os dados desta sessão foram eliminados.");
            ClearGameStats();
            _game.NavigateToPage(PageNavigation.optMenu);
        }

        private void ClearGameStats()
        {
            _game.Enunciado.Questoes.Clear();
            _game.CorrectAnswerPositionList.Clear();
            _game.EnunciadoQ = 0;
            _game.RespostasCertas = 0;
            _game.Pontuacao = 0;
            _game.Enunciado.PontuacaoMax = 0;
        }
    }

    public class NoQuestions : Page, IState<Game>
    {
        private Game _game;
        public NoQuestions(Game game)
        {
            this._game = game;
        }

        public void Avancar()
        {
            MessageBox.Show("Lamentamos mas não possuímos perguntas suficientes para satisfazer o seu pedido, por favor tente novamente reduzindo o número de perguntas.");
            _game.NavigateToPage(PageNavigation.optMenu);
        }
    }


    public partial class Game : Page
    {
        IState<Game> IInGame;
        IState<Game> IEndOfGame;
        IState<Game> IBackHome;
        IState<Game> INoQuestions;
        IState<Game> IStartingGame;

        IState<Game> IState;
        
        private int _questionsNumber;

        private Enunciado enunciado;

        public Enunciado Enunciado
        {
            get { return enunciado; }
            set { enunciado = value; }
        }

        public int QuestionsNumber { 
            get { return _questionsNumber; } 
            set { _questionsNumber = value; }
        }

        private int _pontuacao = 0;

        public int Pontuacao
        {
            get { return _pontuacao; }
            set { _pontuacao = value; }
        }

        private int _respostasCertas = 0;

        public int RespostasCertas
        {
            get { return _respostasCertas; }
            set { _respostasCertas = value; }
        }

        private readonly static Random Rand = new Random();

        //número da pergunta que o utilizador se encontra
        private int _enunciadoQ  = 0; 

        public int EnunciadoQ
        {
            get { return _enunciadoQ;  }
            set { _enunciadoQ = value;  }
        }

        bool hasPressed = false;

        private List<string> _correctAnswerPositionList = new List<string>();

        public List<string> CorrectAnswerPositionList
        {
            get { return _correctAnswerPositionList; }
            set { _correctAnswerPositionList = value; }
        }

        private readonly static int _counterMax = 15;
        private static int _counterAtual = _counterMax;

        private readonly List<string> _positions = new List<string>() { "A", "B", "C", "D" };

        private PontuacaoGame pontuacaoGame;

        public PontuacaoGame PontuacaoGame
        {
            get { return pontuacaoGame; }
            set { pontuacaoGame = value; }
        }

        Timer timer;
        public Game(Enunciado enunciado)
        {
            this.enunciado = enunciado;
            _questionsNumber = this.enunciado.Questoes.Count;
            IInGame = new InGame(this);
            IEndOfGame = new EndOfGame(this);
            IBackHome = new BackHome(this);
            INoQuestions = new NoQuestions(this);
            IStartingGame = new StartingGame(this);

            if (_questionsNumber == 0) IState = INoQuestions;
            else IState = IStartingGame;

            System.Diagnostics.Debug.WriteLine(_questionsNumber);

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;

            if (_positions.Contains(objname) && !hasPressed) HandleButtonPress(objname);

            // other buttons which are not possible answers
            else
            {
                switch (objname)
                {

                    case "Start":
                    case "Next":
                        IState.Avancar();
                        break;

                    case "HomeButton":
                        IState = IBackHome;
                        IState.Avancar();
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
            this.IState = state;
            if (state == IEndOfGame) state.Avancar();
        }

        public IState<Game> GetEndState()
        {
            return IEndOfGame;
        }

        public IState<Game> GetInGameState()
        {
            return IInGame;
        }

        private void StartTimer()
        {
            timer = new Timer();
            timer.StartTimer(this);
            TimerLabel.Width = 800; //volta à largura inicial
            var converter = new BrushConverter();
            TimerLabel.Background = (Brush)converter.ConvertFromString("#FF8BC5B0"); //volta à cor inicial
            ModifyAllButtons("enable-disable", "true");
        }

        public void TimerCountdown(object sender, EventArgs e)
        {
            _counterAtual--;
            TimerLabel.Width = _counterAtual * 800 / _counterMax;
            if (_counterAtual <= 10 && _counterAtual > 5) TimerLabel.Background = Brushes.Yellow;
            if (_counterAtual <=5) TimerLabel.Background = Brushes.Red;
            if (_counterAtual == 0) CounterTimeout(); 
        }

        private void CounterTimeout()
        {
            timer.CounterTimeout();
            CounterAtual = CounterMax;
            ModifyAllButtons("enable-disable", "false");
            Next.Visibility = Visibility.Visible;
        }

        public void StartGame()
        {
            GameAux.CreateListWithRandomValues(correctAnswerPositionList, questionsNumber, positions);
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
            timer.CounterTimeout();
            CounterAtual = CounterMax;
            VerifyAnswer( buttonPressed );
            Next.Visibility = Visibility.Visible;
            hasPressed = true;
            
        }

        private void ShowQuestion()
        {
            Next.Visibility = Visibility.Hidden;
            Question.Content = enunciado.Questoes[_enunciadoQ].Value;
            if(enunciado.Questoes[_enunciadoQ].Type == "boolean") ShowBooleanAnswer();
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

            Resposta primeiraResposta = enunciado.Questoes[_enunciadoQ].Respostas[0];

            // se a resposta correta for true, afirmo que o botão A possui a resposta correta
            if ( primeiraResposta.CorrectAnswer)  _correctAnswerPositionList[_enunciadoQ] = "A"; 
            
            else  _correctAnswerPositionList[_enunciadoQ] = "B"; 

        }

        private void ShowAnswer()
        {
            int j = 0;
            string correctPosition = _correctAnswerPositionList[_enunciadoQ]; //para teste, este valor deverá ser atribuído randomicamente
            List<string> Incorretpositions = _positions.ToList();
            Incorretpositions.Remove(correctPosition); //lista das resposta incorretas
            
            foreach ( Resposta resposta in enunciado.Questoes[_enunciadoQ].Respostas) //percorre as respoostas da questão atual
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
            string correctPosition = _correctAnswerPositionList[_enunciadoQ];

            // green background on correct button
            ModifyDynamicButton(correctPosition, "green-background");

            if (selectedAnswer != correctPosition) ModifyDynamicButton(selectedAnswer, "red-background");
            //atribuir pontuação
            else
            {
                Pontuacao += enunciado.Questoes[_enunciadoQ].Pontuacao;
                RespostasCertas += 1;
            }

            DisableButtons(selectedAnswer, correctPosition);

        }

        private void ModifyAllButtons( string action, string value = null )
        {
            foreach (string buttonName in _positions)
            {
                ModifyDynamicButton(buttonName, action, value);
            }
        }

        private void DisableButtons(string selectedanswser, string correctanswer)
        {
            foreach (string i in _positions)
            {
                if (i != selectedanswser && i != correctanswer) ModifyDynamicButton(i, "enable-disable", "false");
                else ModifyDynamicButton(i, "enable-disable-click", "false" );
            }
        }

        private void ModifyDynamicButton(string name, string action, string value = null)
        {
            object DynamicObject = FindName(name);
            Button DynamicButton = GetButton(DynamicObject);

            System.Diagnostics.Debug.WriteLine("Name: " + name + " Action: " + action);

            switch ( action )
            {
                case "red-background":
                    DynamicButton.Background = Brushes.Red;
                    break;

                case "green-background":
                    DynamicButton.Background = Brushes.Green;
                    break;

                case "new-value":
                    DynamicButton.Content = value;
                    // make button clickable when it gets a new value
                    DynamicButton.IsHitTestVisible = true;
                    break;

                case "enable-disable":
                    DynamicButton.IsEnabled = bool.Parse(value);
                    break;

                case "clear-background":
                    DynamicButton.ClearValue(BackgroundProperty);
                    break;
                case "enable-disable-click":
                    DynamicButton.IsHitTestVisible = bool.Parse(value);
                    break;
            }

        }

    }
}
