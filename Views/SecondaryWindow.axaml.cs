using Avalonia.Controls;
using Avalonia.Interactivity;
using SpotifyAPI.Web;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using SpotifyStats.Assets;
using Avalonia.Media;
using Avalonia.LogicalTree;
using System.Reflection.Emit;
using SpotifyAPI.Web.Auth;
using System.Collections.Generic;
using System.Reactive;
using System;
using System.Linq;
using Swan;
using System.Xml.Linq;

namespace SpotifyStats.Views
{
    public partial class SecondaryWindow : Window
    {
        Avalonia.Controls.Label tracks;
        Avalonia.Controls.Label artists;
        Avalonia.Controls.Label genres;
        private static EmbedIOAuthServer _server;
        private string tracksOutput = "";
        private string artistsOutput = "";
        private string genresOutput = "";
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

            artistsOutput = "";
            tracksOutput = "";
            genresOutput = "";
            //top 50 songs in the netherlands playlist id 37i9dQZEVXbKCF6dqVpDkS
            var config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest("1e4f468b601345c098a3cc41ccb2e138", "586ea92568664ffcbf28f416270a5603");
            var response = await new OAuthClient(config).RequestToken(request);

            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

            var top50Playlist = await spotify.Playlists.Get("37i9dQZEVXbKCF6dqVpDkS");
            var top50Tracks = top50Playlist.Tracks;
            for (int i = 0; i < 5; i++)
            {
                var track = top50Tracks.Items[i];
                string jsonAnswer = track.Track.ToJson();
                int startIndex = jsonAnswer.LastIndexOf("Name");
                int length = jsonAnswer.IndexOf("Popularity") - startIndex;


                // result ends up looking something like this
                // "Name\": \"Song Name\",\r\n    \""
                jsonAnswer = jsonAnswer.Substring(startIndex, length);
                startIndex = jsonAnswer.IndexOf(":") + 3;
                //-2 is because of the ", before it
                length = jsonAnswer.IndexOf("\r") - 2 - startIndex;
                string songName = jsonAnswer.Substring(startIndex, length);
                tracksOutput += $"{i + 1}. {songName}\n";
            }
            genresOutput += "asdf";
            //var topTrackResponse = await spotify;
            //string artistsOutput = "";
            //for (int i = 0; i < 5; i++)
            //{
            //    var track = topTrackResponse.[i];
            //    artistsOutput += ($"{i + 1}.{track.Name}\n");
            //}

            //artists.Content = artistsOutput;
            //artists.Content = "2";
            await UpdateContent();



        }
        private async void Top10Button_Click(object? sender, RoutedEventArgs e)
        {
            await SpotifyConnect();
            await UpdateContent();

        }

        private async Task UpdateContent()
        {
            //condition checks only genresOutput as it is the last one to get updated
            //thus meaning all other output strings will have been updated at this point
            while (genresOutput == "") { await Task.Delay(500); }
            artists.Content = artistsOutput;
            tracks.Content = tracksOutput;
            genres.Content = genresOutput;
        }

        private async Task SpotifyConnect()
        {

            _server = new EmbedIOAuthServer(new Uri("http://localhost:5543/callback"), 5543);
            await _server.Start();

            _server.ImplictGrantReceived += OnImplicitGrantReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, "1e4f468b601345c098a3cc41ccb2e138", LoginRequest.ResponseType.Token)
            {
                Scope = new List<string> { Scopes.UserReadEmail, Scopes.UserReadCurrentlyPlaying, Scopes.UserTopRead, Scopes.Streaming }
            };
            BrowserUtil.Open(request.ToUri());
            //await Task.Delay(3000);

        }
        private async Task OnImplicitGrantReceived(object sender, ImplictGrantResponse response)
        {
            artistsOutput = "";
            tracksOutput = "";
            genresOutput = "";
            await _server.Stop();
            var spotify = new SpotifyClient(response.AccessToken);
            //var topTrackResponse = spotify.UserProfile.Stringify();
            //await Console.Out.WriteLineAsync(topTrackResponse);
            //var topTrackResponse = await spotify.Artists.GetTopTracks("3z97WMRi731dCvKklIf2X6", new ArtistsTopTracksRequest("NL"));
            //for (int i = 0; i < 5; i++)
            //{
            //    var track = topTrackResponse.Tracks[i];
            //    await Console.Out.WriteLineAsync($"{i + 1}.{track.Name}");
            //}
            //TOP ARTISTS
            var request = new PersonalizationTopRequest();
            request.Limit = 50;
            var topArtists = await spotify.Personalization.GetTopArtists(request);
            for (int i = 0; i < 5; i++)
            {
                var artist = topArtists.Items[i];

                artistsOutput += ($"{i + 1}.{artist.Name} \n");
            }

            //TOP TRACKS

            var topTracks = await spotify.Personalization.GetTopTracks();
            for (int i = 0; i < 5; i++)
            {
                var track = topTracks.Items[i];

                tracksOutput += ($"{i + 1}.{track.Name}\n");
            }

            //TOP GENRES

            var topGenres = new Dictionary<string, int>();
            foreach (var artist in topArtists.Items)
            {
                foreach (var genre in artist.Genres)
                {
                    if (!topGenres.ContainsKey(genre))
                        topGenres.Add(genre, 1);
                    else
                        topGenres[genre]++;
                }
            }

            topGenres = topGenres.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            for (int i = 0; i < 5; ++i)
            {
                genresOutput += ($"{i + 1}. {topGenres.Keys.ElementAt(i)}\n");
            }
            //artists.Content = artistsOutput;
            //tracks.Content = tracksOutput;
            //genres.Content = genresOutput;


        }

        private async Task OnErrorReceived(object sender, string error, string state)
        {
            await _server.Stop();
        }
    }
}
