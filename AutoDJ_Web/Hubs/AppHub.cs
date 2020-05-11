using AutoDJ_Web.Data;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Hubs
{
    public class AppHub : Hub
    {
        protected DBHandler dbHandler;
        private readonly IDistributedCache _cache;

        public AppHub(IDistributedCache cache, ApplicationDbContext context)
        {
            _cache = cache;
            dbHandler = new DBHandler(context);
        }

        public async Task UpdateResult(bool next)
        {
            if (next && VideoSearch.ResultIndex < VideoSearch.Videos.Count - 1)
            {
                VideoSearch.ResultIndex++;
                await Clients.Caller.SendAsync("UpdateResult", VideoSearch.Videos[VideoSearch.ResultIndex].ToStringArray());
            }
            else if (!next && VideoSearch.ResultIndex > 0)
            {
                VideoSearch.ResultIndex--;
                await Clients.Caller.SendAsync("UpdateResult", VideoSearch.Videos[VideoSearch.ResultIndex].ToStringArray());
            }
            else
                await Clients.Caller.SendAsync("UpdateResult", null);
        }

        public async Task Search(string searchTerm)
        {
            try
            {
                if (_cache.GetString(searchTerm) == null)
                {
                    VideoSearch.Search(searchTerm, _cache).Wait();
                }
                else
                {
                    List<string[]> result = VideoSearch.ParseVideoDetailsString(_cache.GetString(searchTerm));
                    VideoSearch.PopulateVideoModel(result);
                }

                if (VideoSearch.Videos.Count > 0)
                    await Clients.Caller.SendAsync("Search", 1);
                else
                    await Clients.Caller.SendAsync("Search", 0);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                await Clients.Caller.SendAsync("Search", -1);
            }
        }

        public async Task Cancel()
        {
            VideoSearch.ResetVideoSearch();
            await Clients.Caller.SendAsync("Cancel");
        }

        public async Task ResultId()
        {
            await Clients.Caller.SendAsync("ResultToAdd", new string[] { VideoSearch.ResultIndex.ToString(), VideoSearch.Videos.Count.ToString() });
        }

        public async Task Add(string id)
        {
            VideoModel vidToAdd = VideoSearch.Videos[int.Parse(id)];
            QueueItemModel queueItem = new QueueItemModel(vidToAdd);
            QueueModel.Queue.Add(queueItem);
            //dbHandler.WriteVideo(queueItem);

            await Clients.All.SendAsync("AddToQueue", queueItem);
        }

        public async Task Order()
        {
            QueueModel.OrderQueue();
            await Clients.All.SendAsync("UpdateOrder", QueueModel.GetOrderList());
        }

        public async Task Duration()
        {
            int duration = QueueModel.GetDurationSeconds();
            int hrs = (int)Math.Floor((double)duration / 3600);
            int mins = (int)Math.Floor(((double)duration / 60) % 60);
            int secs = duration % 60;

            if (hrs != 0)
                await Clients.All.SendAsync("SetQueueDuration", hrs + " hrs " + mins + " mins " + secs + " secs");
            else if (mins != 0)
                await Clients.All.SendAsync("SetQueueDuration", mins + " mins " + secs + " secs");
            else
                await Clients.All.SendAsync("SetQueueDuration", secs + " secs");
        }

        public async Task AddVote(int id)
        {
            QueueItemModel myItem = QueueModel.Queue.Where(item => item.Id == id).First();
            myItem.Rating++;
            //dbHandler.UpdateRating(myItem);
            await Clients.All.SendAsync("SetRating", myItem.Rating, id);
        }

        public async Task Remove(int id)
        {
            QueueItemModel myItem = QueueModel.Queue.Where(item => item.Id == id).First();
            QueueModel.Queue.Remove(myItem);
            //dbHandler.DeleteVideo(myItem);
            await Clients.All.SendAsync("RemoveItem", id);
        }

        public async Task Play()
        {
            if (PlayerModel.VideoId == null)
            {
                await NextSong(true);
            }
            else if (PlayerModel.IsPaused)
            {
                PlayerModel.IsPaused = false;
                await Clients.All.SendAsync("Play", "playing");
            }
            else if (!PlayerModel.IsPaused)
            {
                PlayerModel.IsPaused = true;
                await Clients.All.SendAsync("Play", "paused");
            }
            else
                await Clients.All.SendAsync("Play", null);
        }

        public async Task Stop()
        {
            await DisposePlayer();
        }

        public async Task NextSong(bool fromPlay)
        {
            if (QueueModel.Queue.Count > 0)
            {
                QueueItemModel next = QueueModel.Queue[0];
                QueueModel.PopFromQueue();
                PlayerModel.VideoId = next.Video.VideoId;
                PlayerModel.VideoName = next.Video.Name;
                //dbHandler.DeleteVideo(next);
                if (fromPlay)
                    await Clients.All.SendAsync("Play", PlayerModel.GetVideoDetails());
                else
                    await Clients.All.SendAsync("NextSong", PlayerModel.GetVideoDetails());
            }
            else
            {
                await DisposePlayer();
            }
        }

        public async Task DisposePlayer()
        {
            PlayerModel.VideoId = null;
            PlayerModel.VideoName = null;
            await Clients.All.SendAsync("Stop", "empty");
        }
    }
}

