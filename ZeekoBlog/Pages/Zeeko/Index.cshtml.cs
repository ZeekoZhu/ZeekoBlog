using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Application.Services;

namespace ZeekoBlog.Pages.Zeeko
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
            (Articles, TotalPages) = await _articleService.GetPaged(0, 50, 1);
        }
    }
}
