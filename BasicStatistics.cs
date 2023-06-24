using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyStats
{
    public class BasicStatistics
    {
        protected SpotifyClient spotify_;

        protected string artists_, tracks_, genres_ = "";
        protected string Artists
        {
            get
            {
                while (artists_ == "")
                    Task.Delay(10).Wait();
                return artists_;

            }
            set
            {
                artists_ = value;
            }
        }

        protected string Tracks
        {
            get
            {
                while (tracks_ == "")
                    Task.Delay(10).Wait();
                return tracks_;

            }
            set
            {
                tracks_ = value;
            }
        }
        protected string Genres
        {
            get
            {
                while (genres_ == "")
                    Task.Delay(10).Wait();
                return genres_;

            }
            set
            {
                genres_ = value;
            }

        }
    }
}
