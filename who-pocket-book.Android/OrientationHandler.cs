using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using who_pocket_book.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(OrientationHandler))]
namespace who_pocket_book.Droid
{
    class OrientationHandler : IOrientationHandler
    {
        public void ForceDefault()
        {
            MainActivity.Instance.RequestedOrientation = ScreenOrientation.Unspecified;
        }

        public void ForcePortrait()
        {
            MainActivity.Instance.RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}