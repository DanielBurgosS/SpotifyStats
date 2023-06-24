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
        public string TopArtists(int ranking);
        public string TopTracks(int ranking);
        public string TopGenres(int ranking);
        public static Task<IStatistics> CreateInstanceAsync()
        //Irrelevant code as it will be overridden
        { throw new NotImplementedException(); }
    }
}
