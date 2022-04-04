using Foundation;
using System;
using UIKit;
using who_pocket_book.Helpers;
using who_pocket_book.iOS.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageResizer))]
namespace who_pocket_book.iOS.Helpers
{
    public class ImageResizer : IImageResizer
    {
        public byte[] Resize(byte[] original, int to)
        {
            byte[] result = original;
            nfloat compress = 0.9f;
            while (result.Length > to && compress > 0.1)
            {
                UIImage image = UIImage.LoadFromData(NSData.FromArray(original));
                if (image == null)
                {
                    return new byte[] { };
                }
                result = image.AsJPEG(compress).ToArray();
                compress *= 0.9f;
            }
            return result;
        }
    }
}