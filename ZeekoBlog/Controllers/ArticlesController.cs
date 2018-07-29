using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Core.Services;
using ZeekoUtilsPack.AspNetCore.Jwt;

namespace ZeekoBlog.Controllers
{
    [Produces("application/json")]
    [Route("api/Articles")]
    public class ArticlesController : Controller
    {
        private readonly BlogContext _context;
        private readonly ArticleService _articleSvc;

        public ArticlesController(BlogContext context, ArticleService articleSvc)
        {
            _context = context;
            _articleSvc = articleSvc;
        }

        // GET: api/Articles
        [HttpGet]
        public IEnumerable<Article> GetArticles()
        {
            return _context.Articles;
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _context.Articles.SingleOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        [EasyJwtAuthorize]
        public async Task<IActionResult> PutArticle([FromRoute] int id, [FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != article.Id)
            {
                return BadRequest();
            }

            var result = await _articleSvc.UpdateAsync(article);

            if (result.Success)
            {
                return NoContent();
            }

            return BadRequest(result.Msg);
        }

        // POST: api/Articles
        [HttpPost]
        [EasyJwtAuthorize]
        public async Task<IActionResult> PostArticle([FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var userId);
            await _articleSvc.AddAsync(article, userId);

            return CreatedAtAction("GetArticle", new { id = article.Id }, article);
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        [EasyJwtAuthorize]
        public async Task<IActionResult> DeleteArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _context.Articles.SingleOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(article);
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}