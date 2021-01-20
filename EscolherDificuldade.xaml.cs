using System.Windows;
using System.Windows.Controls;

namespace QuizAppWPF
{
    /// <summary>
    /// Interaction logic for EscolherCategoria.xaml
    /// </summary>
    public partial class EscolherDificuldade : Page
    {
        public string IdCategoria { get; set; }
        public EscolherDificuldade(string idCategoria)
        {
            IdCategoria = idCategoria;
            InitializeComponent();
        }


        private void DificuldadeEscolhida(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            string dificuldade = senderButton.Name;

            EscolherNumeroPerguntas pageNumeroPerguntas = new EscolherNumeroPerguntas(IdCategoria, dificuldade);
            NavigationService.Navigate(pageNumeroPerguntas);

        }
    }
}
