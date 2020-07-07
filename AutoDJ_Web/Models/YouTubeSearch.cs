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
    public class YouTubeSearch
    {
        private YouTubeService youtubeService;
        
        public YouTubeSearch()
        {
            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                //ApiKey = "AIzaSyBLnKqOiA4NGKpDKNcZt6EbsYVN_u56C2I", // Production
                ApiKey = "AIzaSyAVSZRciCUhLhssGCRqVGRpQchkQkACjpk", // Development
                ApplicationName = "AutoDJ_Web.YouTubeSearch"
            });
        }

        public async Task<List<VideoModel>> SearchVideo(string searchTerm, IDistributedCache cache)
        {
            var searchListRequest = youtubeService.Search.List("id");
            searchListRequest.Q = searchTerm;
            searchListRequest.MaxResults = 5;
            searchListRequest.Type = "video";
            //searchListRequest.VideoCategoryId = "10";

            var searchListResponse = await searchListRequest.ExecuteAsync();

            var videoListRequest = youtubeService.Videos.List("snippet,contentDetails");
            string idList = "";
            foreach(var result in searchListResponse.Items)
                idList += result.Id.VideoId + ",";

            idList = idList.Substring(0, idList.Length - 1);
            videoListRequest.Id = idList;

            var videoListResponse = await videoListRequest.ExecuteAsync();

            List<string[]> videoDetails = new List<string[]>();

            for (int i = 0; i < videoListResponse.Items.Count; i++)
            {
                var videoResult = videoListResponse.Items[i];
                string[] resultArray = { videoResult.Id, videoResult.Snippet.Title, videoResult.Snippet.ChannelTitle, videoResult.Snippet.PublishedAt.Substring(0, 10),
                                         videoResult.ContentDetails.Duration, videoResult.Snippet.Thumbnails.Default__.Url };
                videoDetails.Add(resultArray);
            }

            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(7));
            cache.SetString(searchTerm + "_video", VideoDetailsToString(videoDetails), options);
            return PopulateVideoModel(videoDetails);
        }

        public async Task<List<PlaylistModel>> SearchPlaylist(string searchTerm, IDistributedCache cache)
        {
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = searchTerm;
            searchListRequest.MaxResults = 5;
            searchListRequest.Type = "playlist";

            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string[]> playlistDetails = new List<string[]>();

            for (int i = 0; i < searchListResponse.Items.Count; i++)
            {
                var playlistResult = searchListResponse.Items[i];
                string[] resultArray = { playlistResult.Id.PlaylistId, playlistResult.Snippet.Title, playlistResult.Snippet.ChannelTitle, playlistResult.Snippet.PublishedAt.Substring(0, 10), 
                                         playlistResult.Snippet.Description, playlistResult.Snippet.Thumbnails.Default__.Url };
                playlistDetails.Add(resultArray);
            }

            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(7));
            cache.SetString(searchTerm + "_playlist", VideoDetailsToString(playlistDetails), options);
            return PopulatePlaylistModel(playlistDetails);
        }

        public async Task<List<VideoModel>> GetPlaylistVideos(string playlistId)
        {
            var listRequest = youtubeService.PlaylistItems.List("snippet,contentDetails");
            listRequest.PlaylistId = playlistId;
            listRequest.MaxResults = 50;

            var listResponse = await listRequest.ExecuteAsync();
            var videoListRequest = youtubeService.Videos.List("snippet,contentDetails");

            string pageToken = "";
            List<string[]> videoDetails = new List<string[]>();

            while (pageToken != null)
            {
                string idList = "";
                foreach (var result in listResponse.Items)
                    idList += result.ContentDetails.VideoId + ",";

                pageToken = listResponse.NextPageToken;
                listRequest.PageToken = pageToken;
                listResponse = await listRequest.ExecuteAsync();

                idList = idList.Substring(0, idList.Length - 1);
                videoListRequest.Id = idList;

                var videoListResponse = await videoListRequest.ExecuteAsync();

                for (int i = 0; i < videoListResponse.Items.Count; i++)
                {
                    var videoResult = videoListResponse.Items[i];
                    string[] resultArray = { videoResult.Id, videoResult.Snippet.Title, videoResult.Snippet.ChannelTitle, videoResult.Snippet.PublishedAt.Substring(0, 10),
                                             videoResult.ContentDetails.Duration, videoResult.Snippet.Thumbnails.Default__.Url };
                    videoDetails.Add(resultArray);
                }
            }
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

        public List<PlaylistModel> PopulatePlaylistModel(List<string[]> playlistDetails)
        {
            List<PlaylistModel> myPlaylists = new List<PlaylistModel>();

            foreach (string[] details in playlistDetails)
            {
                myPlaylists.Add(new PlaylistModel(details[0], details[1], details[2], details[3], details[4], details[5]));
            }
            return myPlaylists;
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
