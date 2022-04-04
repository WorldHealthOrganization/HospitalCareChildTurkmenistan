using System;
using System.Collections.Generic;
using System.Text;
using who_pocket_book.Models;

namespace who_pocket_book.Helpers
{
    public interface IJsonReaderHelper
    {
        List<List<ContentPageItem>> GetContentPageItems(string title);

        List<ContentSearchItem> GetSearchPageItems();

        Dictionary<string, Tuple<int, string>> GetTitles();
    }
}