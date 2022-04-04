
using System.Threading.Tasks;

namespace who_pocket_book
{
    public interface IFileManager
    {
        Task<string> Read(string fileName);
    }
}
