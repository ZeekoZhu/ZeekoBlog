using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ZeekoBlog.Pages
{
    public class OppsModel : PageModel
    {
        public string Code { get; set; }
        public void OnGet(string code)
        {
            Code = code;
        }
    }
}