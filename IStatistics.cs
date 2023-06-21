using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyStats
{
    public interface IStatistics
    {
        public Task<string> TopArtistsAsync(int ranking);
        public Task<string> TopTracksAsync(int ranking);
        public Task<string> TopGenresAsync(int ranking);
    }
}
