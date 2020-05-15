using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class PlayerModel
    {
        public string VideoId { get; set; }
        public string VideoName { get; set; }
        public bool IsPaused { get; set; }
        private Stopwatch Timer { get; set; }
        public int TotalSeconds { get; set; }

        public string[] GetVideoDetails()
        {
            return new string[] { VideoName, VideoId };
        }

        public void Start()
        {
            Timer = new Stopwatch();
            Timer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
            TotalSeconds += (int)Timer.Elapsed.TotalSeconds;
        }

        public int GetCurrentTime()
        {
            return TotalSeconds + (int)Timer.Elapsed.TotalSeconds;
        }
    }
}
