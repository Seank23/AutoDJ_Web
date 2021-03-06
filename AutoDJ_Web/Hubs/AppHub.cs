﻿using AutoDJ_Web.Data;
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
        private YouTubeSearch search;

        public AppHub(IDistributedCache cache, ApplicationDbContext context)
        {
            _cache = cache;
            dbHandler = new DBHandler(context);
            search = new YouTubeSearch();
        }

        public async Task CreateSession(string userId, bool[] permissions)
        {
            if(SessionHandler.GetUsersSession(userId) == "")
            {
                string sessionId = SessionHandler.CreateSession(permissions);
                userId = SessionHandler.CreateAndAddUser(sessionId);
                SessionHandler.AddHostUser(sessionId, userId);
                await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
                await Clients.Caller.SendAsync("SessionCreated", sessionId, userId);
            }
        }

        public async Task JoinSession(string sessionId, string userId, bool sync)
        {
            if (sessionId == null)
                sessionId = "null";
            if (userId == null)
                userId = "null";

            if(SessionHandler.IsValidSession(sessionId) && SessionHandler.GetUsersSession(userId) != sessionId)
            {
                userId = SessionHandler.CreateAndAddUser(sessionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
                await Clients.Caller.SendAsync("SessionJoined", true, sessionId, userId, SessionHandler.GetPermissions(sessionId).PermissionsToDict());
            }
            else if (SessionHandler.GetUsersSession(userId) == sessionId && sessionId != "")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
                await Clients.Caller.SendAsync("SessionJoined", true, sessionId, userId, SessionHandler.GetPermissions(sessionId).PermissionsToDict(), SessionHandler.IsHostUser(sessionId, userId));
            }
            else if(!sync)
            {
                await Clients.Caller.SendAsync("SessionJoined", false);
            }
        }

        public async Task SyncSession(string sessionId)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                PlayerModel myPlayer = SessionHandler.GetPlayer(sessionId);

                if (myQueue.Queue.Count > 0)
                    await Clients.Caller.SendAsync("SyncQueue", myQueue.Queue);

                if (myPlayer.VideoId != null)
                    await Clients.Caller.SendAsync("SyncPlayer", myPlayer.GetVideoDetails(), myPlayer.GetCurrentTime(), myPlayer.IsPaused);

                await Clients.Caller.SendAsync("SessionSynced");
            }
        }

        public async Task LeaveSession(string sessionId, string userId)
        {
            SessionHandler.LeaveSession(sessionId, userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
            await Clients.Caller.SendAsync("SessionLeft");
        }

        public async Task MigrateClientQueue(string sessionId, string queue)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                string[] charDelimiters = new string[] { "\",\"", ",[\"", "\"],", "],[" };
                queue = queue.Replace("[[", "");
                queue = queue.Replace("]]", "");
                string[] values = queue.Split(charDelimiters, StringSplitOptions.RemoveEmptyEntries);

                for(int i = 0; i < values.Length; i += 8)
                {
                    VideoModel video = new VideoModel(values[i + 1], values[i + 2], values[i + 3], values[i + 4], values[i + 5], values[i + 6]);
                    QueueItemModel queueItem = new QueueItemModel(myQueue.NextId(), video, int.Parse(values[i + 7]));
                    myQueue.Queue.Add(queueItem);
                }
                await Clients.Caller.SendAsync("QueueMigrated");
            }
        }

        public void MigrateClientPlayer(string sessionId, List<string> videoDetails)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                PlayerModel myPlayer = SessionHandler.GetPlayer(sessionId);
                myPlayer.VideoId = videoDetails[0];
                myPlayer.VideoName = videoDetails[1];
                myPlayer.TotalSeconds = int.Parse(videoDetails[2]);
                myPlayer.Start();
            }
        }

        public async Task PingServer(string userId)
        {
            if (userId == null)
                userId = "null";
            string[] id = SessionHandler.Ping(userId);
            await Clients.Caller.SendAsync("PingReturned", id[0], id[1]);
        }

        public async Task Search(string searchTerm, bool isPlaylist)
        {
            try
            {
                searchTerm = searchTerm.ToLower();
                string type = "_video";

                if (isPlaylist)
                    type = "_playlist";

                if (_cache.GetString(searchTerm + type) == null)
                {
                    if (isPlaylist)
                    {
                        List<PlaylistModel> results = await search.SearchPlaylist(searchTerm, _cache);
                        await Clients.Caller.SendAsync("Search", results, isPlaylist);
                    }
                    else
                    {
                        List<VideoModel> results = await search.SearchVideo(searchTerm, _cache);
                        await Clients.Caller.SendAsync("Search", results, isPlaylist);
                    }    
                }
                else
                {
                    List<string[]> result = search.ParseVideoDetailsString(_cache.GetString(searchTerm + type));

                    if (isPlaylist)
                    {
                        List<PlaylistModel> results = search.PopulatePlaylistModel(result);
                        await Clients.Caller.SendAsync("Search", results, isPlaylist);
                    }
                    else
                    {
                        List<VideoModel> results = search.PopulateVideoModel(result);
                        await Clients.Caller.SendAsync("Search", results, isPlaylist);
                    }
                }  
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                await Clients.Caller.SendAsync("Search", -1, e.Message);
            }
        }

        public async Task AddToQueue(string sessionId, string userId, string[] data, bool isPlaylist)
        {
            if (SessionHandler.GetUsersSession(userId) == sessionId)
            {
                if (!SessionHandler.GetPermissions(sessionId).CanAddPlaylist && isPlaylist && !SessionHandler.IsHostUser(sessionId, userId))
                    return;

                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                if (isPlaylist)
                {
                    await QueuePlaylist(myQueue, myQueue.Queue.Count, sessionId, data);
                }
                else
                {
                    VideoModel video = new VideoModel(data[0], data[1], data[2], data[3], data[4], data[5]);
                    QueueItemModel item = new QueueItemModel(myQueue.NextId(), video, 1);
                    myQueue.Queue.Add(item);
                    await Clients.Group(sessionId).SendAsync("AddToQueue", item);
                }
                await Clients.Caller.SendAsync("Cancel");
                
            }
        }

        public async Task QueuePlaylist(QueueModel myQueue, int queueCount, string sessionId, string[] data)
        {
            if (myQueue == null)
                myQueue = new QueueModel(queueCount);

            List<VideoModel> playlistVideos = await search.GetPlaylistVideos(data[0]);

            foreach (VideoModel video in playlistVideos)
            {
                QueueItemModel item = new QueueItemModel(myQueue.NextId(), video, 0);
                myQueue.Queue.Add(item);
            }
            if(sessionId != null)
                await Clients.Group(sessionId).SendAsync("QueuePlaylist", myQueue.Queue.Where(item => item.Id >= queueCount).ToArray());
            else
                await Clients.Caller.SendAsync("QueuePlaylist", myQueue.Queue.Where(item => item.Id >= queueCount).ToArray());
        }

        public async Task ClearQueue(string sessionId, string userId)
        {
            if (SessionHandler.GetUsersSession(userId) == sessionId)
            {
                if (!SessionHandler.GetPermissions(sessionId).CanClearQueue && !SessionHandler.IsHostUser(sessionId, userId))
                    return;

                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                myQueue.Queue.Clear();
                await Clients.Group(sessionId).SendAsync("ClearQueue");
            }
        }

        public async Task Order(string sessionId)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                myQueue.OrderQueue();
                await Clients.Group(sessionId).SendAsync("UpdateOrder", myQueue.GetOrderList());
            }
        }

        public async Task Duration(string sessionId, string[] clientDuration)
        {
            int duration = 0;
            if (SessionHandler.IsValidSession(sessionId))
            {
                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                duration = QueueModel.GetDurationSeconds(myQueue.GetDurationList());
            }
            else if(sessionId == "" && clientDuration != null)
                duration = QueueModel.GetDurationSeconds(clientDuration);

            int hrs = (int)Math.Floor((double)duration / 3600);
            int mins = (int)Math.Floor(((double)duration / 60) % 60);
            int secs = duration % 60;

            if(SessionHandler.IsValidSession(sessionId))
            {
                if (hrs != 0)
                    await Clients.Group(sessionId).SendAsync("SetQueueDuration", hrs + " hrs " + mins + " mins " + secs + " secs");
                else if (mins != 0)
                    await Clients.Group(sessionId).SendAsync("SetQueueDuration", mins + " mins " + secs + " secs");
                else
                    await Clients.Group(sessionId).SendAsync("SetQueueDuration", secs + " secs");
            }
            else
            {
                if (hrs != 0)
                    await Clients.Caller.SendAsync("SetQueueDuration", hrs + " hrs " + mins + " mins " + secs + " secs");
                else if (mins != 0)
                    await Clients.Caller.SendAsync("SetQueueDuration", mins + " mins " + secs + " secs");
                else
                    await Clients.Caller.SendAsync("SetQueueDuration", secs + " secs");
            }
            
        }

        public async Task AddVote(string sessionId, int itemId)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                QueueItemModel myItem = myQueue.Queue.Where(item => item.Id == itemId).First();
                myItem.Rating++;
                await Clients.Group(sessionId).SendAsync("SetRating", myItem.Rating, itemId);
            }
        }

        public async Task Remove(string sessionId, string userId, int itemId)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                if (!SessionHandler.GetPermissions(sessionId).CanRemove && !SessionHandler.IsHostUser(sessionId, userId))
                    return;

                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                QueueItemModel myItem = myQueue.Queue.Where(item => item.Id == itemId).First();
                myQueue.Queue.Remove(myItem);
                await Clients.Group(sessionId).SendAsync("RemoveItem", itemId);
            }
        }

        public async Task Play(string sessionId)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                PlayerModel player = SessionHandler.GetPlayer(sessionId);

                if (player.VideoId == null)
                {
                    await NextSong(sessionId, true);
                }
                else if (player.IsPaused)
                {
                    player.IsPaused = false;
                    player.Start();
                    await Clients.Group(sessionId).SendAsync("Play", "playing");
                }
                else if (!player.IsPaused)
                {
                    player.IsPaused = true;
                    player.Stop();
                    await Clients.Group(sessionId).SendAsync("Play", "paused");
                }
                else
                    await Clients.Group(sessionId).SendAsync("Play", null);
            }
        }

        public async Task Stop(string sessionId, string userId)
        {
            if (!SessionHandler.GetPermissions(sessionId).CanStop && !SessionHandler.IsHostUser(sessionId, userId))
                return;

            await DisposePlayer(sessionId);
        }

        public async Task NextSong(string sessionId, bool fromPlay)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                QueueModel myQueue = SessionHandler.GetQueue(sessionId);
                PlayerModel myPlayer = SessionHandler.GetPlayer(sessionId);
                if (myQueue.Queue.Count > 0)
                {
                    QueueItemModel next = myQueue.Queue[0];
                    myQueue.PopFromQueue();
                    myPlayer.VideoId = next.Video.VideoId;
                    myPlayer.VideoName = next.Video.Name;
                    myPlayer.TotalSeconds = 0;
                    myPlayer.Start();

                    if (fromPlay)
                        await Clients.Group(sessionId).SendAsync("Play", myPlayer.GetVideoDetails());
                    else
                        await Clients.Group(sessionId).SendAsync("NextSong", myPlayer.GetVideoDetails());
                }
                else
                {
                    await DisposePlayer(sessionId);
                }
            }
        }

        public async Task DisposePlayer(string sessionId)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                PlayerModel myPlayer = SessionHandler.GetPlayer(sessionId);
                myPlayer.VideoId = null;
                myPlayer.VideoName = null;
                await Clients.Group(sessionId).SendAsync("Stop", "empty");
            }
        }

        public async Task GetPlayer(string sessionId)
        {
            if (SessionHandler.IsValidSession(sessionId))
            {
                PlayerModel myPlayer = SessionHandler.GetPlayer(sessionId);
                await Clients.Caller.SendAsync("PlayerReturned", myPlayer.GetVideoDetails(), myPlayer.GetCurrentTime(), myPlayer.IsPaused);
            }
        }
    }
}

