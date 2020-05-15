using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDJ_Web.Data;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoDJ_Web.Pages
{
    public class QueueItemTemplateModel : PageModel
    {
        public QueueItemModel MyItem { get; set; }

        protected DBHandler dbHandler;

        public QueueItemTemplateModel(ApplicationDbContext context)
        {
            dbHandler = new DBHandler(context);
        }

        public void OnGet(string sessionId, int itemId)
        {
            MyItem = SessionHandler.GetQueue(sessionId).Queue.Where(item => item.Id == itemId).First();
        }
    }
}