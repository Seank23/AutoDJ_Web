using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoDJ_Web.Pages
{
    public class PlayerModel : PageModel
    {
        public JsonResult OnGetPlay()
        {
            if(Models.PlayerModel.VideoId == null)
            {
                return OnGetNextSong();
            }

            if(Models.PlayerModel.IsPaused)
            {
                Models.PlayerModel.IsPaused = false;
                return new JsonResult("playing");
            }
            else if(!Models.PlayerModel.IsPaused)
            {
                Models.PlayerModel.IsPaused = true;
                return new JsonResult("paused");
            }
            return null;
        }

        public JsonResult OnGetStop()
        {
            return DisposePlayer();
        }

        public JsonResult OnGetNextSong()
        {
            if (Models.QueueModel.Queue.Count > 0)
            {
                QueueItemModel next = Models.QueueModel.Queue[0];
                Models.QueueModel.PopFromQueue();
                Models.PlayerModel.VideoId = next.Video.VideoId;
                Models.PlayerModel.VideoName = next.Video.Name;
                return new JsonResult(Models.PlayerModel.GetVideoDetails());
            }
            else
            {
                return DisposePlayer();
            }
        }

        public JsonResult DisposePlayer()
        {
            Models.PlayerModel.VideoId = null;
            Models.PlayerModel.VideoName = null;
            return new JsonResult("empty");
        }
    }
}