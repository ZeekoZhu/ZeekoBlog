using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ZeekoBlog.TagHelpers
{
    [HtmlTargetElement("side-group")]
    public class SideGroupTagHelper : TagHelper
    {
        private readonly IViewComponentHelper _componentHelper;
        public string Title { get; set; }
        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public SideGroupTagHelper(IViewComponentHelper componentHelper)
        {
            _componentHelper = componentHelper;
        }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ((DefaultViewComponentHelper)_componentHelper).Contextualize(ViewContext);
            var childContent = output.Content.IsModified ? output.Content.GetContent() :
                (await output.GetChildContentAsync()).GetContent();
            var content = await _componentHelper.InvokeAsync("SideGroup", new { title = Title, content = childContent });
            output.Content.SetHtmlContent(content);
            output.TagName = null;
        }
    }
}
