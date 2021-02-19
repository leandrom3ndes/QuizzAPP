using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace QuizAppWPF
{
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
                    NavigationService.Navigate( Enunciado.pageCategoria);
                    break;
                case "Classificacao":
                    NavigationService.Navigate(Enunciado.scoreMenu);
                    break;
                case "Sair":
                    System.Windows.Application.Current.Shutdown();
                    break;
            }
        }
    }
}
