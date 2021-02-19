using System.Windows;
using System.Windows.Controls;

namespace QuizAppWPF
{
    /// <summary>
    /// Interaction logic for EscolherCategoria.xaml
    /// </summary>
    public partial class EscolherCategoria : Page
    {
        public EscolherCategoria()
        {
            InitializeComponent();
        }

        private void CategoriaEscolhida(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            string idCategoria = senderButton.Name.Remove(0, 2);

            EscolherDificuldade pageDificuldade = new EscolherDificuldade( idCategoria );
            NavigationService.Navigate( pageDificuldade );

        }
    }
}
