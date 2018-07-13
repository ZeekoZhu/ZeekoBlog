using System;
using System.Collections.Generic;
using System.Text;

namespace ZeekoBlog.Markdown.Plugins.TOCItemsPlugin
{
    public class TOCItem
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public string Id { get; set; }
    }
}
