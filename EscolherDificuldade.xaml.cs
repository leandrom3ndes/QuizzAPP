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
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text.RegularExpressions;

namespace QuizAppWPF
{
    /// <summary>
    /// Interaction logic for EscolherCategoria.xaml
    /// </summary>
    public partial class EscolherDificuldade : Page
    {
        public string idCategoria { get; set; }
        public EscolherDificuldade(string idCategoria)
        {
            this.idCategoria = idCategoria;
            InitializeComponent();
        }

        
        private void DificuldadeEscolhida(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;

            string dificuldade = senderButton.Name;

            EscolherNumeroPerguntas pageNumeroPerguntas = new EscolherNumeroPerguntas( idCategoria, dificuldade);
            this.NavigationService.Navigate( pageNumeroPerguntas );

        }
    }
}
