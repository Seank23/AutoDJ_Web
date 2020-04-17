using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoDJ_Web.Pages
{
    public class SearchModel : PageModel
    {
        public bool hasSearched = false;

        public SearchModel()
        {

        }

        public JsonResult OnGetUpdateResult(bool next)
        {
            if (next && VideoSearch.ResultIndex < VideoSearch.Videos.Count - 1)
            {
                VideoSearch.ResultIndex++;
                return new JsonResult(VideoSearch.Videos[VideoSearch.ResultIndex].ToStringArray());
            }
            if (!next && VideoSearch.ResultIndex > 0)
            {
                VideoSearch.ResultIndex--;
                return new JsonResult(VideoSearch.Videos[VideoSearch.ResultIndex].ToStringArray());
            }
            return new JsonResult(null);
        }

        public JsonResult OnGetSearch(string searchTerm)
        {
            hasSearched = true;
            VideoSearch.Search(searchTerm).Wait();

            if (VideoSearch.Videos.Count > 0)
                return new JsonResult(1);
            return new JsonResult(0);
        }

        public void OnGetCancel()
        {
            VideoSearch.ResetVideoSearch();
        }

        public JsonResult OnGetResultId()
        {
            return new JsonResult(new string[] { VideoSearch.ResultIndex.ToString(), VideoSearch.Videos.Count.ToString() });
        }
    }
}