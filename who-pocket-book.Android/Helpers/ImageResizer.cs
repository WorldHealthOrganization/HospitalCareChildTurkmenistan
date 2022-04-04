using Android.Graphics;
using System;
using System.IO;
using who_pocket_book.Droid.Helpers;
using who_pocket_book.Helpers;
using Xamarin.Forms;
using static Android.Graphics.Bitmap;

[assembly: Dependency(typeof(ImageResizer))]
namespace who_pocket_book.Droid.Helpers
{
    public class ImageResizer : IImageResizer
    {
        public byte[] Resize(byte[] original, int to)
        {
            byte[] result = original;
            int compress = 100;
            while (result.Length > to && compress > 0)
            {
                Bitmap bitmap = BitmapFactory.DecodeByteArray(original, 0, original.Length);
                MemoryStream blob = new MemoryStream();
                bitmap.Compress(CompressFormat.Jpeg, compress, blob);
                result = blob.ToArray();
                compress = Math.Max(0, compress - 20);
            }
            return result;
        }
    }
}