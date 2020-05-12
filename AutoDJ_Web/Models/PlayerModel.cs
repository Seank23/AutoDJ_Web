using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public static class PlayerModel
    {
        public static string VideoId { get; set; }
        public static string VideoName { get; set; }
        public static bool IsPaused { get; set; }
        private static Stopwatch Timer { get; set; }
        public static int TotalSeconds { get; set; }

        public static string[] GetVideoDetails()
        {
            return new string[] { VideoName, VideoId };
        }

        public static void Start()
        {
            Timer = new Stopwatch();
            Timer.Start();
        }

        public static void Stop()
        {
            Timer.Stop();
            TotalSeconds += (int)Timer.Elapsed.TotalSeconds;
        }

        public static int GetCurrentTime()
        {
            return TotalSeconds + (int)Timer.Elapsed.TotalSeconds;
        }
    }
}
