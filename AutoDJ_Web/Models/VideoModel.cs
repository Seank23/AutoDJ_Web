using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;

namespace AutoDJ_Web.Models
{
    public class VideoModel
    {
        public string VideoId { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public string PublishedDate { get; set; }
        public string Duration { get; set; }
        public string Thumbnail { get; set; }

        public VideoModel(string id, string name, string channel, string published, string duration, string thumbnail) 
        {
            VideoId = id;
            Name = name;
            Channel = channel;
            PublishedDate = published;
            Duration = DurationToTime(duration);
            Thumbnail = thumbnail;
        }

        public string DurationToTime(string duration)
        {
            if (duration != null)
            {
                List<string> elements = new List<string>();
                string num = "";
                for(int i = 0; i < duration.Length - 1; i++) 
                { 
                    if(Char.IsNumber(duration[i]))
                    {
                        num += duration[i];
                        if(!Char.IsNumber(duration[i + 1]))
                        {
                            elements.Add(num);
                            num = "";
                        }
                    }
                }
                if (elements.Count == 1)
                    elements.Add("00");
                else if (elements[1].Length == 1)
                    elements[1] = "0" + elements[1];

                return elements[0] + ":" + elements[1];
            }
            return null;
        }
    }
}
