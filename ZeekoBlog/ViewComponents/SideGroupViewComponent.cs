using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace ZeekoBlog.ViewComponents
{
    public class SideGroupViewComponent : ViewComponent
    {
        public Task<ViewViewComponentResult> InvokeAsync(string title, string content)
        {
            ViewBag.Title = title;
            ViewBag.Content = content;
            return Task.FromResult(View());
        }
    }
}
