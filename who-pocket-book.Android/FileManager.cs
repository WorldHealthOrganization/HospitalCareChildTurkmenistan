
using System.IO;
using System.Threading.Tasks;
using who_pocket_book.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileManager))]
namespace who_pocket_book.Droid
{
    class FileManager : IFileManager
    {
        public async Task<string> Read(string fileName)
        {
            var assetManager = MainActivity.Instance.Assets;
            using (var streamReader = new StreamReader(assetManager.Open(fileName)))
            {
                var content = streamReader.ReadToEnd();
                return (content);
            }
        }
    }
}