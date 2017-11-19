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
    public class ArticleModel : PageModel
    {
        private readonly ArticleService _articleService;

        public ArticleModel(ArticleService articleService)
        {
            this._articleService = articleService;
        }

        public Article Article { get; set; }
        
        public async Task<IActionResult> OnGet(int id)
        {
            Article = await _articleService.GetById(id);
            if (Article == null)
            {
                return RedirectToPage("/Error");
            }
            return Page();
        }
    }
}