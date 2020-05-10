using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDJ_Web.Data;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoDJ_Web.Pages
{
    public class QueueModel : PageModel
    {
        protected DBHandler dbHandler;

        public QueueModel(ApplicationDbContext context)
        {
            dbHandler = new DBHandler(context);
        }

        public JsonResult OnGetAdd(string id)
        {
            VideoModel vidToAdd = VideoSearch.Videos[int.Parse(id)];
            QueueItemModel queueItem = new QueueItemModel(vidToAdd);
            Models.QueueModel.Queue.Add(queueItem);
            dbHandler.WriteVideo(queueItem);

            return new JsonResult(queueItem);
        }

        public JsonResult OnGetOrder()
        {
            Models.QueueModel.OrderQueue();
            return new JsonResult(Models.QueueModel.GetOrderList());
        }

        public JsonResult OnGetDuration()
        {
            int duration = Models.QueueModel.GetDurationSeconds();
            int hrs = (int)Math.Floor((double)duration / 3600);
            int mins = (int)Math.Floor(((double)duration / 60) % 60);
            int secs = duration % 60;

            if (hrs != 0)
                return new JsonResult(hrs + " hrs " + mins + " mins " + secs + " secs");
            else if(mins != 0)
                return new JsonResult(mins + " mins " + secs + " secs");
            else
                return new JsonResult(secs + " secs");
        }
    }
}