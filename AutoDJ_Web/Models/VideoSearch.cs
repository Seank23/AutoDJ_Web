using AutoDJ_Web.Models;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web
{
    public class VideoSearch
    {
        public async Task<List<VideoModel>> Search(string searchTerm, IDistributedCache cache)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyC0pX3JjTzni8IyMnhLImoU2QaJy_6SPuM",
                ApplicationName = "AutoDJ_Web.VideoSearch"
            });

            var searchListRequest = youtubeService.Search.List("id");
            searchListRequest.Q = searchTerm;
            searchListRequest.MaxResults = 5;
            searchListRequest.Type = "video";
            searchListRequest.VideoCategoryId = "10";

            var searchListResponse = await searchListRequest.ExecuteAsync();

            var videoListRequest = youtubeService.Videos.List("snippet, contentDetails");
            string idList = "";
            foreach(var result in searchListResponse.Items)
                idList += result.Id.VideoId + ",";
            videoListRequest.Id = idList;

            var videoListResponse = await videoListRequest.ExecuteAsync();

            List<string[]> videoDetails = new List<string[]>();

            for (int i = 0; i < videoListResponse.Items.Count; i++)
            {
                var videoResult = videoListResponse.Items[i];
                string[] resultArray = { videoResult.Id, videoResult.Snippet.Title, videoResult.Snippet.ChannelTitle, videoResult.Snippet.PublishedAt.Value.ToShortDateString(),
                                         videoResult.ContentDetails.Duration, videoResult.Snippet.Thumbnails.Default__.Url };
                videoDetails.Add(resultArray);
            }

            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(7));
            cache.SetString(searchTerm, VideoDetailsToString(videoDetails), options);
            return PopulateVideoModel(videoDetails);
        }

        public List<VideoModel> PopulateVideoModel(List<string[]> videoDetails)
        {
            List<VideoModel> myVideos = new List<VideoModel>();

            foreach(string[] details in videoDetails)
            {
                string duration = VideoModel.DurationToTime(details[4]);
                myVideos.Add(new VideoModel(details[0], details[1], details[2], details[3], duration, details[5]));
            }
            return myVideos;
        }

        public string VideoDetailsToString(List<string[]> details)
        {
            string output = "";

            foreach(string[] array in details)
            {
                for(int i = 0; i < array.Length; i++)
                {
                    output += array[i];
                    if (i < array.Length - 1)
                        output += " ````` ";
                }
                output += " ~~~~~ ";
            }
            return output;
        }

        public List<string[]> ParseVideoDetailsString(string str)
        {
            List<string[]> videoDetails = new List<string[]>();
            string[] lists = str.Split(" ~~~~~ ");

            foreach(string list in lists)
            {
                if (list != "")
                {
                    string[] details = list.Split(" ````` ");
                    videoDetails.Add(details);
                }
            }
            return videoDetails;
        }
    }
}
