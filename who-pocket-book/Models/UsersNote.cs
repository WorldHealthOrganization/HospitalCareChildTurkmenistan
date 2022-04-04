using MongoDB.Bson;
using Realms;

namespace who_pocket_book.Models
{
    public class UsersNote : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [Required]
        public string Category { get; set; }

        [Required]
        public string Text { get; set; }

        private UsersNote() { }

        public UsersNote(string category, string text)
        {
            Category = category;
            Text = text;
        }
    }
}