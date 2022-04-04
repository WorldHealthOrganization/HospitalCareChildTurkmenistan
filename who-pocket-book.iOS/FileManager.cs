
using Foundation;
using System.IO;
using System.Threading.Tasks;
using who_pocket_book.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileManager))]
namespace who_pocket_book.iOS
{
    class FileManager : IFileManager
    {
        public async Task<string> Read(string fileName)
        {
            var uri = Path.Combine(NSBundle.MainBundle.BundlePath, "assets", fileName);
            var content = File.ReadAllText(uri);
            return (content);
        }
    }
}