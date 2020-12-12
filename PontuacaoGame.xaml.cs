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
        private int pontTotal { get; set; }
        private int pontObtida { get; set; }
        public PontuacaoGame(int pontTotal, int pontObtida)
        {
            InitializeComponent();
            PontObtida.Content = pontObtida;
            PontTotal.Content = pontTotal;
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
        private void ClearStats()
        {
            Enunciado.Questoes.Clear();
            //Enunciado.ListaRespostas.Clear();
            Game.correctAnswerPositionList.Clear();
            Enunciado.pontuacaoMax = 0;
        }
    }
}
