using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoDJ_Web.Pages
{
    public class VideoResultsModel : PageModel
    {
        public JsonResult OnGet(bool next)
        {
            if (next && VideoSearch.ResultIndex < VideoSearch.Videos.Count - 1)
            {
                VideoSearch.ResultIndex++;
                return new JsonResult(VideoSearch.Videos[VideoSearch.ResultIndex].ToStringArray());
            }
            if(!next && VideoSearch.ResultIndex > 0)
            {
                VideoSearch.ResultIndex--;
                return new JsonResult(VideoSearch.Videos[VideoSearch.ResultIndex].ToStringArray());
            }
            return new JsonResult(null);
        }
    }
}