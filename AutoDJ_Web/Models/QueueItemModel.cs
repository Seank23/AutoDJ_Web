using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class QueueItemModel
    {
        public int Id { get; set; }
        public VideoModel Video { get; set; }
        public int Rating { get; set; }

        public QueueItemModel(int id, VideoModel video, int rating)
        {
            Video = video;
            Rating = rating;
            Id = id;
        }
    }
}
