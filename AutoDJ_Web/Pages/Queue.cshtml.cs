using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoDJ_Web.Pages
{
    public class QueueModel : PageModel
    {
        public JsonResult OnGetAdd(string id)
        {
            VideoModel vidToAdd = VideoSearch.Videos[int.Parse(id)];
            QueueItemModel queueItem = new QueueItemModel(vidToAdd);
            Models.QueueModel.Queue.Add(queueItem);

            return new JsonResult(queueItem);
        }

        public JsonResult OnGetOrder()
        {
            Models.QueueModel.OrderQueue();
            return new JsonResult(Models.QueueModel.GetOrderList());
        }
    }
}