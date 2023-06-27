using SpotifyAPI.Web;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyStats
{
    public class GeneralStatistics : BasicStatistics, IStatistics
    {
        private List<string> artistIds_ = new List<string>();
        private async Task<string> TopTracksAsync(int ranking)
        {
            string tracksOutput = "";
            var top50Playlist = await spotify_.Playlists.Get("37i9dQZEVXbKCF6dqVpDkS");
            var top50Tracks = top50Playlist.Tracks;

            //TOP TRACKS
            for (int i = 0; i < ranking; i++)
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
            return tracksOutput;
        }

        private async Task<string> TopArtistsAsync(int ranking)
        {
            string artistsOutput = "";
            var top50Playlist = await spotify_.Playlists.Get("37i9dQZEVXbKCF6dqVpDkS");
            var top50Tracks = top50Playlist.Tracks;
            var artistNames = new Dictionary<string, int>();

            foreach (var item in top50Tracks.Items)
            {
                string jsonAnswer = item.ToJson();
                int startIndex = jsonAnswer.LastIndexOf("Artists");
                int length = jsonAnswer.LastIndexOf("AvailableMarkets") - startIndex;
                string artists = jsonAnswer.Substring(startIndex, length);

                //adding 8 to get rid of the 'Name": "'
                while (artists.IndexOf("Name") != -1)
                {
                    artists = artists.Substring(artists.IndexOf("Name") + 8);
                    string artistName = artists.Substring(0, artists.IndexOf("\r") - 2);
                    //adding 15 to avoid the string itself
                    artists = artists.Substring(artists.IndexOf("spotify:artist:") + 15);
                    string artistId = artists.Substring(0, artists.IndexOf("\r") - 1);

                    //Adding the data to the data structures
                    if (!artistNames.ContainsKey(artistName))
                        artistNames.Add(artistName, 1);
                    else
                        artistNames[artistName]++;

                    if (!artistIds_.Contains(artistId))
                        artistIds_.Add(artistId);

                }

            }
            artistNames = artistNames.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            for (int i = 0; i < ranking; ++i)
            {
                artistsOutput += ($"{i + 1}. {artistNames.Keys.ElementAt(i)}\n");
            }
            return artistsOutput;
        }

        private async Task<string> TopGenresAsync(int ranking)
        {
            string genresOutput = "";
            string genresJson = "";


            //The API can only receive a query with 50 artists IDs at a time
            //This is why this loop works in this manner
            //Example: artistIds_.Count = 81
            //First loop we request 50
            //Second loop we request the remaining 31
            while (artistIds_.Count == 0)
                await Task.Delay(10);
            for (int i = 1; i * 50 - artistIds_.Count <= 50; ++i)
            {
                List<string> tempList = new List<string>();
                //in the case that there is remaining IDs
                if (i * 50 - artistIds_.Count > 0)
                    //Example 81 - 1*50 to request the remaining 31 IDs
                    tempList = artistIds_.Skip((i - 1) * 50).Take(artistIds_.Count - ((i - 1) * 50)).ToList();
                else
                    tempList = artistIds_.Skip((i - 1) * 50).Take(50).ToList();
                var tempResponse = await spotify_.Artists.GetSeveral(new ArtistsRequest(tempList));
                genresJson += tempResponse.ToJson();
            }

            var topGenres = new Dictionary<string, int>();
            //Extracting the Gernres information based on the artists
            while (genresJson.IndexOf("Genres") != -1)
            {
                //leaves a string that starts exatly where the first genre is
                genresJson = genresJson.Substring(genresJson.IndexOf("Genres") + 42);
                string allGenres = genresJson.Substring(0, genresJson.IndexOf(']') - 13);
                while (allGenres.IndexOf("\r") != -1)
                {
                    allGenres = allGenres.Substring(allGenres.IndexOf("\"") + 1);
                    string genre = allGenres.Substring(0, allGenres.IndexOf("\r") - 1).TrimEnd('\"');
                    allGenres = allGenres.Substring(allGenres.IndexOf("\r") + 1);
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

        private async Task Results()
        {
            var result = await Task.WhenAll(TopArtistsAsync(10), TopTracksAsync(10), TopGenresAsync(10));
            Artists = result[0];
            Tracks = result[1];
            Genres = result[2];
        }
        public static async Task<GeneralStatistics> CreateInstanceAsync()
        {
            var config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest("1e4f468b601345c098a3cc41ccb2e138", "586ea92568664ffcbf28f416270a5603");
            var response = await new OAuthClient(config).RequestToken(request);


            GeneralStatistics instance = new();
            instance.spotify_ = new SpotifyClient(config.WithToken(response.AccessToken));
            await instance.Results();
            return instance;

        }

        private GeneralStatistics()
        {
            //To create instance: 
            //var myClass = await GeneralStatistics.CreateInstanceAsync();
        }
    }

}
