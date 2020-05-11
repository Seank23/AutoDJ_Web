using AutoDJ_Web.Data;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Hubs
{
    public class QueueHub : Hub
    {
        protected DBHandler dbHandler;

        public QueueHub(ApplicationDbContext context)
        {
            dbHandler = new DBHandler(context);
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
    }
}

