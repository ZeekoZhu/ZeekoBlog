using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace ZeekoBlog.ViewComponents
{
    public class SideItemViewComponent : ViewComponent
    {
        public Task<ViewViewComponentResult> InvokeAsync(string head, string itemContent)
        {
            ViewBag.Head = head;
            ViewBag.Content = itemContent;
            return Task.FromResult(View());
        }
    }
}
