using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class VideoModel
    {
        public string VideoId { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public DateTime? PublishedTime { get; set; }
        public string Description { get; set; } 

        public VideoModel(string id, string name, string channel, DateTime? published, string description) 
        {
            VideoId = id;
            Name = name;
            Channel = channel;
            PublishedTime = published;
            Description = description;
        }
        public string[] ToStringArray()
        {
            return new string[]{ (VideoSearch.ResultIndex + 1).ToString(), VideoSearch.Videos.Count.ToString(), VideoId, Name, Channel, PublishedTime.ToString(), Description };
        }
    }
}
