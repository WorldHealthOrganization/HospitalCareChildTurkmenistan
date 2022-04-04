using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using who_pocket_book.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl_iOS))]
namespace who_pocket_book.iOS
{
    class BaseUrl_iOS : IBaseUrl
    {
        public string Get()
        {
            return Path.Combine(NSBundle.MainBundle.BundlePath, "assets");
        }
    }
}