using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyStats
{
    public class PersonalStatistics : BasicStatistics, IStatistics
    {
        private static EmbedIOAuthServer server_;


        Paging<FullArtist> topArtists = new Paging<FullArtist>();
        private async Task<string> TopArtistsAsync(int ranking)
        {
            string artistsOutput = "";
            var request = new PersonalizationTopRequest();
            request.Limit = 50;
            var topArtists = await spotify_.Personalization.GetTopArtists(request);
            for (int i = 0; i < ranking; i++)
            {
                var artist = topArtists.Items[i];

                artistsOutput += ($"{i + 1}.{artist.Name} \n");
            }
            return artistsOutput;
        }

        private new async Task<string> TopGenresAsync(int ranking)
        {
            string genresOutput = "";
            var request = new PersonalizationTopRequest();
            request.Limit = 50;
            var topArtists = await spotify_.Personalization.GetTopArtists(request);
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
            for (int i = 0; i < ranking; ++i)
            {
                genresOutput += ($"{i + 1}. {topGenres.Keys.ElementAt(i)}\n");
            }

            return genresOutput;
        }

        private new async Task<string> TopTracksAsync(int ranking)
        {
            string tracksOutput = "";
            var topTracks = await spotify_.Personalization.GetTopTracks();
            for (int i = 0; i < ranking; i++)
            {
                var track = topTracks.Items[i];

                tracksOutput += ($"{i + 1}.{track.Name}\n");
            }
            return tracksOutput;
        }


        private new async Task SpotifyConnect()
        {

            server_ = new EmbedIOAuthServer(new Uri("http://localhost:5543/callback"), 5543);
            await server_.Start();

            server_.ImplictGrantReceived += OnImplicitGrantReceived;
            server_.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(server_.BaseUri, "1e4f468b601345c098a3cc41ccb2e138", LoginRequest.ResponseType.Token)
            {
                Scope = new List<string> { Scopes.UserReadEmail, Scopes.UserReadCurrentlyPlaying, Scopes.UserTopRead, Scopes.Streaming }
            };
            BrowserUtil.Open(request.ToUri());
            //await Task.Delay(3000);

        }
        private async Task OnImplicitGrantReceived(object sender, ImplictGrantResponse response)
        {
            await server_.Stop();
            this.spotify_ = new SpotifyClient(response.AccessToken);
            Artists = await TopArtistsAsync(10);
            Tracks = await TopTracksAsync(10);
            Genres = await TopGenresAsync(10);

        }

        private async Task OnErrorReceived(object sender, string error, string state)
        {
            await server_.Stop();
        }
        public static async Task<PersonalStatistics> CreateInstanceAsync()
        {
            PersonalStatistics instance = new();
            await instance.SpotifyConnect();
            return instance;

        }


        public string TopArtists(int ranking)
        {
            return string.Join("\n", Artists.Split("\n").ToList().Take(ranking));
        }

        public string TopTracks(int ranking)
        {
            return string.Join("\n", Tracks.Split("\n").ToList().Take(ranking));

        }

        public string TopGenres(int ranking)
        {
            return string.Join("\n", Genres.Split("\n").ToList().Take(ranking));

        }

        private PersonalStatistics()
        {

        }
    }
}
