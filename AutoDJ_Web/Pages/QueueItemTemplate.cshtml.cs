using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoDJ_Web.Pages
{
    public class QueueItemTemplateModel : PageModel
    {
        public QueueItemModel MyItem { get; set; }

        public void OnGet()
        {
            MyItem = Models.QueueModel.Queue[Models.QueueModel.Queue.Count - 1];
        }

        public JsonResult OnGetAddVote(int id)
        {
            MyItem = Models.QueueModel.Queue.Where(item => item.Id == id).First();
            MyItem.Rating++;
            return new JsonResult(MyItem.Rating);
        }

        public void OnGetRemove(int id)
        {
            MyItem = Models.QueueModel.Queue.Where(item => item.Id == id).First();
            Models.QueueModel.Queue.Remove(MyItem);
        }
    }
}