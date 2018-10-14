using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Application.Services;
using ZeekoBlog.DTOs;
using ZeekoBlog.Extensions;
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

        /// <summary>
        /// GET: /api/Articles
        ///
        /// 获取当前用户的文章列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<ArticleListDto> GetArticles()
        {
            var userId = User.GetId();
            return _context.Articles.Where(x => x.BlogUser.Id == userId).ProjectTo<ArticleListDto>();
        }

        // GET: /api/Articles/5
        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="id">文章 Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDetailDto>> GetArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _context.Articles.ProjectTo<ArticleDetailDto>().SingleOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }

        // PUT: /api/Articles/5
        /// <summary>
        /// 修改文章的内容
        /// </summary>
        /// <param name="id">文章 Id</param>
        /// <param name="dto">数据</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [EasyJwtAuthorize]
        public async Task<IActionResult> PutArticle([FromRoute] int id, [FromBody] ArticlePostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = Mapper.Map<Article>(dto);
            article.Id = id;

            var result = await _articleSvc.UpdateAsync(article, User.GetId());

            if (result.Success)
            {
                return NoContent();
            }

            return BadRequest(result.Msg);
        }

        // POST: /api/Articles
        /// <summary>
        /// 发表一篇文章
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [EasyJwtAuthorize]
        public async Task<IActionResult> PostArticle([FromBody] ArticlePostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = Mapper.Map<Article>(dto);
            article.LastEdited = DateTime.UtcNow;

            await _articleSvc.AddAsync(article, User.GetId());

            return CreatedAtAction("GetArticle", new { id = article.Id }, article);
        }

        public async Task<IActionResult> PostArticle([FromRoute] int id)
        {
            var result = await _articleSvc.RerenderAsync(id, User.GetId());
            if (result.Success)
            {
                return NoContent();
            }

            return BadRequest(result.Msg);

        }

        // DELETE: api/Articles/5
        /// <summary>
        /// 删除一篇文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [EasyJwtAuthorize]
        public async Task<IActionResult> DeleteArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _context.ValidArticles.SingleOrDefaultAsync(m => m.Id == id && m.BlogUser.Id == User.GetId());
            if (article == null)
            {
                return NotFound();
            }
            article.SoftDelete = true;
            await _context.SaveChangesAsync();

            return Ok(article);
        }
    }
}
