using System.Windows;

namespace DesktopContainer.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RealeaseContainer_OnClick(object sender, RoutedEventArgs e)
        {
            new MainContainer().Show();
            this.Close();
        }
    }
}