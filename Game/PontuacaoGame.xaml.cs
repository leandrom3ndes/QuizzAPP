using System.Windows;
using System.Windows.Controls;

namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para PontuacaoGame.xam
    /// </summary>
    public partial class PontuacaoGame : Page
    {
        Game game;
        public PontuacaoGame(Game game, int respostasCertas, int totalRespostas)
        {
            this.game = game;
            InitializeComponent();
            ShowScores(respostasCertas, totalRespostas);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;
            switch (objname)
            {
                case "JogarNovamente":
                    ClearStats();
                    EscolherCategoria pageCategoria = new EscolherCategoria();
                    this.NavigationService.Navigate(pageCategoria);
                    break;
                case "VoltarMenu":
                    ClearStats();
                    OptionMenu optionMenu = new OptionMenu();
                    this.NavigationService.Navigate(optionMenu);
                    break;

            }
        }
        public void ClearStats()
        {
            game.Enunciado.Questoes.Clear();
            game.CorrectAnswerPositionList.Clear();
            game.EnunciadoQ = 0;
            game.RespostasCertas = 0;
            game.Pontuacao = 0;
            game.Enunciado.PontuacaoMax = 0;
        }

        private void ShowScores(int respostasCertas, int totalRespostas)
        {
            Resultado.Content = "Obteve " + ResultInPercentage(respostasCertas, totalRespostas) + " % das respostas corretas!";
            Score.Content = "Score: " + respostasCertas  + "/" + totalRespostas;
        }

        private decimal ResultInPercentage( int respostasCertas, int totalRespostas )
        {
            var result = (respostasCertas / (decimal)totalRespostas) * 100;
            return result ;
        }

    }
}
