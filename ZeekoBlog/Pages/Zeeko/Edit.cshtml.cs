using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZeekoBlog.Models;
using ZeekoBlog.Services;

namespace ZeekoBlog.Pages.Zeeko
{
    public class EditModel : PageModel
    {
        private readonly ArticleService _articleService;

        public EditModel(ArticleService articleService)
        {
            this._articleService = articleService;
        }

        public Article Article { get; set; }

        public async Task OnGet(int? id)
        {
            if (id != null)
            {
                Article = await _articleService.GetById(id.Value);
            }
        }
    }
}