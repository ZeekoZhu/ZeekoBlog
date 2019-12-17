using System.Threading.Tasks;
using ZeekoBlog.Core.Models;

namespace ZeekoBlog.Application.Services
{
    public interface IArticleRenderer
    {
        Task RenderAsync(Article article);
    }
}
