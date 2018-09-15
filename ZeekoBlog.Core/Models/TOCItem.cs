namespace ZeekoBlog.Core.Models
{
    public class TOCItem
    {
        /// <summary>
        /// 标题名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标题级别
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 数据库中的主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用作 href 的链接地址
        /// </summary>
        public string AnchorName { get; set; }
        /// <summary>
        /// TOC 的顺序
        /// </summary>
        public int Order { get; set; }
        public int ArticleId { get; set; }
    }
}
