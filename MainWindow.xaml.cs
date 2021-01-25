using System.Windows;


namespace QuizAppWPF
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Login loginPage = new Login();
            mainFrame.NavigationService.Navigate(loginPage);
        }
    }

}
