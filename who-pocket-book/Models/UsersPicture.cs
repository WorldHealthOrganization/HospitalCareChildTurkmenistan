using MongoDB.Bson;
using Realms;
using who_pocket_book.Helpers;
using Xamarin.Forms;

namespace who_pocket_book.Models
{
    public class UsersPicture: RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [Required]
        public string Category { get; set; }

        [Required]
        public byte[] Picture { get; set; }

        private UsersPicture() { }

        public UsersPicture(string category, byte[] picture)
        {
            Category = category;
            Picture = DependencyService.Get<IImageResizer>().Resize(picture, (int)(1024 * 1024 * (Device.RuntimePlatform == Device.iOS ? 10 : 0.5)));
        }
    }
}