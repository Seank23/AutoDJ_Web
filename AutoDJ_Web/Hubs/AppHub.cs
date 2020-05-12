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
        private VideoSearch search;

        public AppHub(IDistributedCache cache, ApplicationDbContext context)
        {
            _cache = cache;
            dbHandler = new DBHandler(context);
            search = new VideoSearch();
        }

        public async Task SyncSession()
        {
            if(QueueModel.Queue.Count > 0)
                await Clients.Caller.SendAsync("SyncQueue", QueueModel.Queue);

            if (PlayerModel.VideoId != null)
                await Clients.Caller.SendAsync("SyncPlayer", PlayerModel.GetVideoDetails(), PlayerModel.GetCurrentTime());
        }

        public async Task Search(string searchTerm)
        {
            try
            {
                List<VideoModel> videos = new List<VideoModel>();

                if (_cache.GetString(searchTerm) == null)
                {
                    videos = await search.Search(searchTerm, _cache);
                }
                else
                {
                    List<string[]> result = search.ParseVideoDetailsString(_cache.GetString(searchTerm));
                    videos = search.PopulateVideoModel(result);
                }
                await Clients.Caller.SendAsync("Search", videos);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                await Clients.Caller.SendAsync("Search", -1);
            }
        }

        public async Task AddToQueue(string[] videoData)
        {
            VideoModel video = new VideoModel(videoData[0], videoData[1], videoData[2], videoData[3], videoData[4], videoData[5]);
            QueueItemModel item = new QueueItemModel(video);
            QueueModel.Queue.Add(item);
            await Clients.Caller.SendAsync("Cancel");
            await Clients.All.SendAsync("AddToQueue", item);
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
                PlayerModel.Start();
                await Clients.All.SendAsync("Play", "playing");
            }
            else if (!PlayerModel.IsPaused)
            {
                PlayerModel.IsPaused = true;
                PlayerModel.Stop();
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
                PlayerModel.TotalSeconds = 0;
                PlayerModel.Start();
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

