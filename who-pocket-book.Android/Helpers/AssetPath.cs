using Android.Content.Res;
using System.IO;
using who_pocket_book.Droid.Helpers;
using who_pocket_book.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(AssetPath))]
namespace who_pocket_book.Droid.Helpers
{
    public class AssetPath : IAssetPath
    {
        public Stream GetFile(string name)
        {
            AssetManager assets = MainActivity.Instance.Assets;
            return assets.Open(name);
        }
    }
}