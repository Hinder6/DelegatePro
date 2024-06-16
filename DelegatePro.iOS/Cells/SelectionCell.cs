using System;

using UIKit;

namespace DelegatePro.iOS
{
    public partial class SelectionCell : UITableViewCell
    {
        private const string CellIdentifier = "SelectionCell";
        public SelectionCell(IntPtr handle) : base(handle)
        {
        }

        public static UITableViewCell Dequeue(UITableView tableView, string labelText, string typeValue, UIColor textColor)
        {
            var cell = (SelectionCell)tableView.DequeueReusableCell(CellIdentifier);

            cell.TextLabel.Text = labelText;
            cell.SelectionLabel.Text = typeValue;
            cell.SelectionLabel.TextColor = textColor;

            return cell;
        }
    }
}