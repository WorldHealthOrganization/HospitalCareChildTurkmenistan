using System.Collections.Generic;

namespace who_pocket_book.Models
{
    public class CategoryDisplayInfo
    {
        public string Category { get; set; }

        public string Text { get; set; }

        public IEnumerable<UsersPicture> Pictures { get; set; }
    }
}