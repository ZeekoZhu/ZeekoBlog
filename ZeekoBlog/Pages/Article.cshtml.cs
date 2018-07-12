using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Core.Services;
using ZeekoBlog.Markdown;
using ZeekoBlog.Markdown.Plugins.CodeLangDetectionPlugin;
using ZeekoBlog.Markdown.Plugins.TOCItemsPlugin;

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

            var mdResult = _mdSvc.Process(Article.Content);
            Languages = mdResult.Storage.TryGet<List<string>>(CodeLangDetectionPlugin.ID).Value;
            TOCList = mdResult.Storage.TryGet<List<TOCItem>>(TOCItemsPlugin.ID).Value;
            return Page();
        }
    }
}
