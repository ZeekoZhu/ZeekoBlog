using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZeekoBlog.Models;
using ZeekoBlog.Services;

namespace ZeekoBlog.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ArticleService _articleService;

        public IndexModel(ArticleService articleService)
        {
            this._articleService = articleService;
        }

        public List<Article> Articles { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGet()
        {
            (Articles, TotalPages) = await _articleService.GetPaged(0, 20);
        }
    }
}
