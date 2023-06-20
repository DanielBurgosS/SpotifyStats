using Avalonia.Controls;
using Avalonia.Interactivity;
using SpotifyAPI.Web;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using SpotifyStats.Assets;
using Avalonia.Media;
using Avalonia.LogicalTree;
using System.Reflection.Emit;

namespace SpotifyStats.Views
{
    public partial class SecondaryWindow : Window
    {
        Avalonia.Controls.Label tracks;
        Avalonia.Controls.Label artists;
        Avalonia.Controls.Label genres;
        public SecondaryWindow()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            artists = this.FindControl<Avalonia.Controls.Label>("Artists");
            tracks = this.FindControl<Avalonia.Controls.Label>("Tracks");
            genres = this.FindControl<Avalonia.Controls.Label>("Genres");

        }
        private async void Top5Button_Click(object? sender, RoutedEventArgs e)
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
                output += ($"{i + 1}.{track.Name}\n");
            }

            artists.Content = output;
            tracks.Content = "5";

        }
        private async void Top10Button_Click(object? sender, RoutedEventArgs e)
        {
            tracks.Content = "10";
        }
    }
}
