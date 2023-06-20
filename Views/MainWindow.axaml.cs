using Avalonia.Controls;
using Avalonia.Interactivity;
using SpotifyAPI.Web;
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

        private async void PersonalButton_Click(object? sender, RoutedEventArgs e)
        {
            SecondaryWindow secondaryWindow = new SecondaryWindow();
            secondaryWindow.Show();
            this.Close();
        }

        private async void GeneralButton_Click(object? sender, RoutedEventArgs e)
        {
            var config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest("1e4f468b601345c098a3cc41ccb2e138", "586ea92568664ffcbf28f416270a5603");
            var response = await new OAuthClient(config).RequestToken(request);

            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

            var topTrackResponse = await spotify.Artists.GetTopTracks("3z97WMRi731dCvKklIf2X6", new ArtistsTopTracksRequest("NL"));
            string output = "";
            for (int i = 0; i < 5; i++)
            {
                var track = topTrackResponse.Tracks[i];
                output += ($"{i + 1}.{track.Name}");
            }

            Title.Content = output;
        }

    }
}