using Avalonia.Controls;
using Avalonia.Interactivity;
using SpotifyAPI.Web;
using SpotifyStats.Assets;
using System.Threading.Tasks;


namespace SpotifyStats.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object? sender, RoutedEventArgs routedEventArgs)
        {
            // Handle the button click event here
            // You can perform any necessary actions or logic
        }

        private void PersonalButton_Click(object? sender, RoutedEventArgs e)
        {
            UserChoice.Choice = "Personal";
            OpenSecondaryWindow();
            //this.Hide();
        }

        private void GeneralButton_Click(object? sender, RoutedEventArgs e)
        {
            UserChoice.Choice = "General";
            OpenSecondaryWindow();

        }

        private void OpenSecondaryWindow()
        {
            SecondaryWindow secondaryWindow = new SecondaryWindow();
            secondaryWindow.Show();
            this.Close();

        }

    }
}