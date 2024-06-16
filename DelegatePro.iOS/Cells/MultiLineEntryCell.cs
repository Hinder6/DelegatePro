using System;
using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public partial class MultiLineEntryCell : BaseTableViewCell, IUITextViewDelegate
	{
        private const string CellIdentifier = "MultiLineEntryCell";

        private NoteDetailsViewController NoteDetailVC => ParentVC as NoteDetailsViewController;

		public MultiLineEntryCell (IntPtr handle) : base (handle)
		{
		}

        [Export("textView:shouldChangeTextInRange:replacementText:")]
        public bool ShouldChangeText(UITextView textView, NSRange range, string text)
        {
            var data = textView.Text + ((text != "\n") ? text : string.Empty);
            if (string.IsNullOrEmpty(text))
            {
                // deleting character
                data = data.Remove((int)range.Location, (int)range.Length);
            }

            if (text.Length > 1)
            {
                // user pasted value or selected an autocorrect.
                data = text;
            }

            if (NoteDetailVC != null)
            {
                if (TextLabel.Text == Constants.Note.TextViewText)
                    NoteDetailVC.NoteText = data;
            }

            return true;
        }

        public void AddBorder()
        {
            var layer = TextView.Layer;
            layer.BorderColor = UIColor.LightGray.CGColor;
            layer.BorderWidth = 1f;
        }

        public static UITableViewCell Dequeue(BaseViewController cont, UITableView tableView, string labelText, string value)
        {
            var cell = (MultiLineEntryCell)tableView.DequeueReusableCell(CellIdentifier);
            cell.TextLabel.Text = labelText;

            if (string.IsNullOrWhiteSpace(cell.TextView.Text))
                cell.TextView.Text = value;

            cell.ParentVC = cont;
            cell.TextView.Delegate = cell;
            cell.TextView.AutocapitalizationType = UITextAutocapitalizationType.Sentences;

            cell.AddBorder();

            return cell;
        }
	}
}
