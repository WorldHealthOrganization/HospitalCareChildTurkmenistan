using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using who_pocket_book.Models;

namespace who_pocket_book.Db
{
    public class NotesDatabase
    {
        public static async Task UpsertNote(string category, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                UsersNote temp = (await GetAllNotes()).FirstOrDefault(x => x.Category == category);
                if (temp != null)
                {
                    temp.Realm.Write(() => { temp.Realm.Remove(temp); });
                }
            }
            else
            {
                UsersNote temp = (await GetAllNotes()).FirstOrDefault(x => x.Category == category);
                if (temp != null)
                {
                    temp.Realm.Write(() => { temp.Text = text; });
                }
                else
                {
                    Realm realm = await Realm.GetInstanceAsync();
                    await realm.WriteAsync((r) =>
                    {
                        r.Add(new UsersNote(category, text));
                    });
                }
            }
        }

        public static UsersNote GetNote(string category)
        {
            Realm realm = Realm.GetInstance();
            return realm.All<UsersNote>().FirstOrDefault(x => x.Category == category);
        }

        public static async Task<List<UsersNote>> GetAllNotes()
        {
            Realm realm = await Realm.GetInstanceAsync();
            return realm.All<UsersNote>().ToList();
        }

        private static IDisposable token;

        public static async Task SubscribeForNotes(Action handler)
        {
            if (token != null)
            {
                token.Dispose();
                token = null;
            }
            Realm realm = await Realm.GetInstanceAsync();
            token = realm.All<UsersNote>().SubscribeForNotifications((s, c, e) =>
            {
                handler.Invoke();
            });
        }
    }
}