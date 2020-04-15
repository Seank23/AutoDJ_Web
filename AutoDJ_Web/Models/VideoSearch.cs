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
            Videos = new List<VideoModel>();
            Videos.Add(new VideoModel(null, null, null, null, null));
        }

        public async Task Search()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyC0pX3JjTzni8IyMnhLImoU2QaJy_6SPuM",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = SearchTerm;
            searchListRequest.MaxResults = 5;

            var searchListResponse = await searchListRequest.ExecuteAsync();
            Videos = new List<VideoModel>();

            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        Videos.Add(new VideoModel(searchResult.Id.VideoId, 
                                                  searchResult.Snippet.Title, 
                                                  searchResult.Snippet.ChannelTitle, 
                                                  searchResult.Snippet.PublishedAt,
                                                  searchResult.Snippet.Description));
                        break;
                }
            }
        }
    }
}
