using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public static class PlayerModel
    {
        public static string VideoId { get; set; }
        public static string VideoName { get; set; }
        public static bool IsPaused { get; set; }

        public static string[] GetVideoDetails()
        {
            return new string[] { VideoName, VideoId };
        }
    }
}
