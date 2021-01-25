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

namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para PontuacaoGame.xam
    /// </summary>
    public partial class PontuacaoGame : Page
    {
        public PontuacaoGame(int respostasCertas, int totalRespostas)
        {
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
        public static void ClearStats()
        {
            Enunciado.Questoes.Clear();
            Game.correctAnswerPositionList.Clear();
            Enunciado.pontuacaoMax = 0;
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
