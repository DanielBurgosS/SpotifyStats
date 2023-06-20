using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyStats.Assets
{
    public static class UserChoice
    {
        public static string Choice { get; set; }
        public static bool IsPersonal => Choice == "Personal";
        public static bool IsGeneral => Choice == "General";

    }
}
