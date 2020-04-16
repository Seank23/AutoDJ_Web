using AutoDJ_Web.Models;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web
{
    public class VideoSearch
    {
        public string SearchTerm { get; set; }
        public static List<VideoModel> Videos { get; set; }
        public static int ResultIndex { get; set; } = -1;

        public VideoSearch()
        {
            SearchTerm = "";
            ResultIndex = -1;
            Videos = new List<VideoModel>{ new VideoModel(null, null, null, null, null, null, null) };
        }

        public async Task Search()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyC0pX3JjTzni8IyMnhLImoU2QaJy_6SPuM",
                ApplicationName = GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = SearchTerm;
            searchListRequest.MaxResults = 5;
            searchListRequest.Type = "video";
            searchListRequest.VideoCategoryId = "10";

            var searchListResponse = await searchListRequest.ExecuteAsync();

            var videoListRequest = youtubeService.Videos.List("snippet, contentDetails");
            string idList = "";
            foreach(var result in searchListResponse.Items)
                idList += result.Id.VideoId + ", ";
            videoListRequest.Id = idList;

            var videoListResponse = await videoListRequest.ExecuteAsync();

            Videos = new List<VideoModel>();

            for(int i = 0; i < videoListResponse.Items.Count; i++)
            {
                var videoResult = videoListResponse.Items[i];
                Videos.Add(new VideoModel(videoResult.Id, 
                                            videoResult.Snippet.Title, 
                                            videoResult.Snippet.ChannelTitle, 
                                            videoResult.Snippet.PublishedAt,
                                            videoResult.Snippet.Description,
                                            videoResult.ContentDetails.Duration,
                                            videoResult.Snippet.Thumbnails.Default__.Url));
            }
        }
    }
}
