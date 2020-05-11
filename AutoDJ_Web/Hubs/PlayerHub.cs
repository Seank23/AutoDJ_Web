using AutoDJ_Web.Data;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Hubs
{
    public class PlayerHub : Hub
    {
        protected DBHandler dbHandler;

        public PlayerHub(ApplicationDbContext context)
        {
            dbHandler = new DBHandler(context);
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
                if(fromPlay)
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

