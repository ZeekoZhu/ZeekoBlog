using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZeekoBlog.Models;
using ZeekoBlog.Services;
using ZeekoBlog.ViewModels;

namespace ZeekoBlog.Pages
{
    public class ArticleModel : PageModel
    {
        private readonly ArticleService _articleService;
        private readonly MarkdownService _mdSvc;

        public ArticleModel(ArticleService articleService, MarkdownService mdSvc)
        {
            this._articleService = articleService;
            _mdSvc = mdSvc;
        }

        public Article Article { get; set; }
        public List<string> Languages { get; set; }
        public List<TOCItem> TOCList { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            Article = await _articleService.GetById(id);
            if (Article == null)
            {
                return NotFound();
            }

            Languages = _mdSvc.GetLanguages(Article.Content);
            TOCList = _mdSvc.GetTableOfContent(Article.Content);
            return Page();
        }
    }
}