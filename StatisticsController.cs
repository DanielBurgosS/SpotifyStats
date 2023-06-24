using SpotifyStats.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyStats
{
    public class StatisticsController
    {
        private IStatistics statistics_;

        public StatisticsController()
        {
            switch (UserChoice.Choice)
            {
                case "Personal":
                    Personal();
                    break;
                case "General":
                    General();
                    break;

            }
        }

        public async Task<(string, string, string)> Update(int ranking)
        {
            return (statistics_.TopArtists(ranking), statistics_.TopTracks(ranking), statistics_.TopGenres(ranking));
        }

        public async Task Personal()
        {
            statistics_ = PersonalStatistics.CreateInstanceAsync().Result;
        }
        public async Task General()
        {
            statistics_ = await GeneralStatistics.CreateInstanceAsync();
        }

    }
}
