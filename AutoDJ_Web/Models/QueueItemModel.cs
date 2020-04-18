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

        public QueueItemModel(VideoModel video)
        {
            Video = video;
            Rating = 0;
            Id = QueueModel.Queue.Count;
        }
    }
}
