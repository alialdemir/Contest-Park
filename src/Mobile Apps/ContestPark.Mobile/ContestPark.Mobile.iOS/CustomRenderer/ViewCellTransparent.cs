using ContestPark.Mobile.iOS.CustomRenderer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCellTransparent))]

namespace ContestPark.Mobile.iOS.CustomRenderer
{
    public class ViewCellTransparent : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            try
            {
                var cell = base.GetCell(item, reusableCell, tv);
                if (cell != null)
                {
                    // Disable native cell selection color style - set as *Transparent*
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                }
                return cell;
            }
            catch (System.Exception ex)
            {
            }

            return base.GetCell(item, reusableCell, tv);
        }
    }
}
