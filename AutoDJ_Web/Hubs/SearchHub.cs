using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace AutoDJ_Web.Hubs
{
    public class SearchHub : Hub
    {
        private readonly IDistributedCache _cache;

        public SearchHub(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task UpdateResult(bool next)
        {
            if (next && VideoSearch.ResultIndex < VideoSearch.Videos.Count - 1)
            {
                VideoSearch.ResultIndex++;
                await Clients.Caller.SendAsync("UpdateResult", VideoSearch.Videos[VideoSearch.ResultIndex].ToStringArray());
            }
            else if (!next && VideoSearch.ResultIndex > 0)
            {
                VideoSearch.ResultIndex--;
                await Clients.Caller.SendAsync("UpdateResult", VideoSearch.Videos[VideoSearch.ResultIndex].ToStringArray());
            }
            else
                await Clients.Caller.SendAsync("UpdateResult", null);
        }

        public async Task Search(string searchTerm)
        {
            try
            {
                if (_cache.GetString(searchTerm) == null)
                {
                    VideoSearch.Search(searchTerm, _cache).Wait();
                }
                else
                {
                    List<string[]> result = VideoSearch.ParseVideoDetailsString(_cache.GetString(searchTerm));
                    VideoSearch.PopulateVideoModel(result);
                }

                if (VideoSearch.Videos.Count > 0)
                    await Clients.Caller.SendAsync("Search", 1);
                else
                    await Clients.Caller.SendAsync("Search", 0);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                await Clients.Caller.SendAsync("Search", -1);
            }
        }

        public async Task Cancel()
        {
            VideoSearch.ResetVideoSearch();
            await Clients.Caller.SendAsync("Cancel");
        }

        public async Task ResultId()
        {
            await Clients.Caller.SendAsync("ResultToAdd", new string[] { VideoSearch.ResultIndex.ToString(), VideoSearch.Videos.Count.ToString() });
        }
    }
}
