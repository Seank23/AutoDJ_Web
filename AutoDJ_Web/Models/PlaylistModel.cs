using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class PlaylistModel
    {
        public string PlaylistId { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public string Description { get; set; }
        public string PublishedDate { get; set; }
        public string Thumbnail { get; set; }

        public PlaylistModel(string id, string name, string channel, string description, string published, string thumbnail)
        {
            PlaylistId = id;
            Name = name;
            Channel = channel;
            Description = description;
            PublishedDate = published;
            Thumbnail = thumbnail;
        }
    }
}
