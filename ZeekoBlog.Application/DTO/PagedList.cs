using System.Collections.Generic;

namespace ZeekoBlog.Application.DTO
{
    public class PagedList<T>
    {
        public List<T> List { get; set; }
        public int TotalPage { get; set; }
    }
}