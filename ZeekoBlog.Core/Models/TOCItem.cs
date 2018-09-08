namespace ZeekoBlog.Core.Models
{
    public class TOCItem
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Id { get; set; }
        public string AnchorName { get; set; }
        public int ArticleId { get; set; }
    }
}
