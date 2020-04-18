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
    }
}