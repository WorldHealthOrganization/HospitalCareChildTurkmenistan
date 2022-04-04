using System.IO;

namespace who_pocket_book.Helpers
{
    public interface IAssetPath
    {
        Stream GetFile(string name);
    }
}