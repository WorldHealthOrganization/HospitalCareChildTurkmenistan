using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using who_pocket_book.Models;

namespace who_pocket_book.Db
{
    public class PicturesDatabase
    {
        public static async Task AddPicture(UsersPicture picture)
        {
            Realm realm = await Realm.GetInstanceAsync();
            await realm.WriteAsync((r) =>
            {
                r.Add(picture);
            });
        }

        public static void RemovePicture(UsersPicture picture)
        {
            if (picture != null)
            {
                picture.Realm.Write(() => { picture.Realm.Remove(picture); });
            }
        }

        public static async Task<bool> CategoryPicturesExist(string category)
        {
            Realm realm = await Realm.GetInstanceAsync();
            return realm.All<UsersPicture>().Where(x => x.Category == category).Count() > 0;
        }

        public static async Task<List<UsersPicture>> GetAllPictures()
        {
            Realm realm = await Realm.GetInstanceAsync();
            return realm.All<UsersPicture>().ToList();
        }

        private static IDisposable token;

        public static async Task SubscribeForPictures(Action handler)
        {
            if (token != null)
            {
                token.Dispose();
                token = null;
            }
            Realm realm = await Realm.GetInstanceAsync();
            token = realm.All<UsersPicture>().SubscribeForNotifications((s, c, e) =>
            {
                handler.Invoke();
            });
        }
    }
}