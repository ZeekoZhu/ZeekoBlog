using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ZeekoBlog.TagHelpers
{
    [HtmlTargetElement("script")]
    [HtmlTargetElement("link", Attributes = "[rel=stylesheet]", TagStructure = TagStructure.WithoutEndTag)]
    public class AutoMinTagHelper : TagHelper
    {
        private readonly IHostingEnvironment _env;
        private readonly IUrlHelper _url;

        public string Cdn { get; set; }

        public AutoMinTagHelper(IHostingEnvironment env, IActionContextAccessor accessor, IUrlHelperFactory urlHelperFactory)
        {
            this._env = env;
            _url = urlHelperFactory.GetUrlHelper(accessor.ActionContext);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            do
            {

                if (_env.IsDevelopment()) break;
                var srcVal = (Attr: "", Val: "");
                if (context.TagName.Equals("script", StringComparison.CurrentCultureIgnoreCase))
                {
                    var src = context.AllAttributes["src"];
                    srcVal.Attr = "src";
                    srcVal.Val = (src?.Value as HtmlString)?.Value;
                }
                else if (context.TagName.Equals("link", StringComparison.CurrentCultureIgnoreCase))
                {
                    var href = context.AllAttributes["href"];
                    srcVal.Attr = "href";
                    srcVal.Val = (href?.Value as HtmlString)?.Value;
                }
                else break;

                if (string.IsNullOrEmpty(srcVal.Val))
                {
                    break;
                }
                if (srcVal.Val.TrimStart(' ').StartsWith("http"))
                {
                    break;
                }
                if (string.IsNullOrEmpty(Cdn)) // using local min file
                {
                    var extReg = new Regex(@"(\.js|\.css)$", RegexOptions.IgnoreCase);
                    srcVal.Val = extReg.Replace(srcVal.Val, ".min." + (srcVal.Attr == "src" ? "js" : "css"));
                }
                else // using cdn resource
                {
                    srcVal.Val = Cdn;
                }
                output.Attributes.RemoveAll(srcVal.Attr);
                output.Attributes.Add(new TagHelperAttribute(srcVal.Attr, _url.Content(srcVal.Val)));
            } while (false);

        }
    }
}
