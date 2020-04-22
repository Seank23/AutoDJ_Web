using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.Caching.Sqlite;

namespace AutoDJ_Web.Pages
{
    public class SearchModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public SearchModel(IDistributedCache cache)
        {
            _cache = cache;
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
            try
            {
                if(_cache.GetString(searchTerm) == null)
                {
                    VideoSearch.Search(searchTerm, _cache).Wait();
                }
                else
                {
                    List<string[]> result = VideoSearch.ParseVideoDetailsString(_cache.GetString(searchTerm));
                    VideoSearch.PopulateVideoModel(result);
                }

                if (VideoSearch.Videos.Count > 0)
                    return new JsonResult(1);
                return new JsonResult(0);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return new JsonResult(-1);
            }
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