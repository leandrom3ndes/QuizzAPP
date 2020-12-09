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
    /// Interação lógica para OptionMenu.xam
    /// </summary>
    public partial class OptionMenu : Page
    {
        public OptionMenu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string objname = ((Button)sender).Name;
            switch (objname)
            {
                case "Jogo":
                    /*Jogo pageGame = new Jogo();
                    this.NavigationService.Navigate(pageGame);*/
                    EscolherCategoria pageCategoria = new EscolherCategoria();
                    this.NavigationService.Navigate( pageCategoria );
                    break;
                case "Classificacao":
                    MessageBox.Show("O chaves é muito bolha!!");
                    break;
                case "Sair":
                    MessageBox.Show("Obrigado e volte sempre!");
                    System.Windows.Application.Current.Shutdown();
                    break;
            }
        }
    }
}
