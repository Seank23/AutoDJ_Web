using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDJ_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AutoDJ_Web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public VideoSearch VideoSearch { get; set; }

        public bool hasSearched = false;
        private readonly ILogger<IndexModel> _logger;


        public IndexModel(ILogger<IndexModel> logger)
        {
            VideoSearch = new VideoSearch();
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public JsonResult OnGetSearch(string searchTerm)
        {
            hasSearched = true;
            VideoSearch.SearchTerm = searchTerm;
            VideoSearch.Search().Wait();

            if (VideoSearch.Videos.Count > 0)
                return new JsonResult(1);
            return new JsonResult(0);
        }

        public void OnGetCancel()
        {
            VideoSearch = new VideoSearch();
        }
    }
}
