using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Core.Services;

namespace ZeekoBlog.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ArticleService _articleService;

        public IndexModel(ArticleService articleService)
        {
            _articleService = articleService;
        }

        public List<Article> Articles { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery(Name = "p")] int page = 1)
        {
            if (page < 1) page = 1;
            CurrentIndex = page;
            (Articles, TotalPages) = await _articleService.GetPaged(page - 1, 20);
            if (Articles.Any() == false)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
