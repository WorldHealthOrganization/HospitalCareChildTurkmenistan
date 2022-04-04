using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace who_pocket_book.iOS.Renderers
{
    class CustomListViewCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            UITableViewCell cell = base.GetCell(item, reusableCell, tv);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }
    }
}