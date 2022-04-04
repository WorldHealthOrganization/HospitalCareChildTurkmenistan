using System.Linq;

namespace who_pocket_book.Models
{
    public enum ChildElementType
    {
        Content,
        Chapter,
        Html,
        Link,
        Image,
        Url,
        Bold
    }

    public class ContentPageItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ChildElement { get; set; }
        public ChildElementType ChildElementType { get; set; }
    }

    public class ContentTitleItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
    }

    public class ContentSearchItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChildElement { get; set; }
        public ChildElementType ChildElementType { get; set; }
        public string SearchKey { get; set; }
    }

    public class InternalContentSearchItem
    {
        public InternalContentSearchItem(ContentSearchItem item)
        {
            Id = item.Id;
            Title = item.Title;
            Description = item.Description;
            ChildElement = item.ChildElement;
            ChildElementType = item.ChildElementType;
            SearchKeys = string.Join(" ", Title, Description, item.SearchKey).Trim().ToLowerInvariant();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChildElement { get; set; }
        public ChildElementType ChildElementType { get; set; }
        public string SearchKeys { get; set; }
    }
}