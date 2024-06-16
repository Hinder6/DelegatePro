using System;
using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public partial class EntryTextCell : BaseTableViewCell, IUITextFieldDelegate
    {
        private const string CellIdentifier = "EntryTextCell";

        private PersonDetailViewController PersonDetailVC => ParentVC as PersonDetailViewController;
        private NoteDetailsViewController NoteDetailVC => ParentVC as NoteDetailsViewController;

        public UIKeyboardType KeyboardType { get; set; }

        public EntryTextCell(IntPtr handle) : base(handle)
        {
        }

        public override bool ResignFirstResponder()
        {
            var returnValue = base.ResignFirstResponder();
            EntryTextField.ResignFirstResponder();
            return returnValue;
        }

        [Export("textField:shouldChangeCharactersInRange:replacementString:")]
        public bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
        {
            var text = EntryTextField.Text + ((replacementString != "\n") ? replacementString : string.Empty);
            if (string.IsNullOrEmpty(replacementString))
            {
                // deleting character
                text = text.Remove((int)range.Location, (int)range.Length);
            }

            if (replacementString.Length > 1)
            {
                // user pasted value or selected an autocorrect.
                text = replacementString;
            }

            if (PersonDetailVC != null)
            {
                if (TextLabel.Text == Constants.PersonDetail.FirstNameLabelText)
                    PersonDetailVC.FirstName = text;
                else if (TextLabel.Text == Constants.PersonDetail.LastNameLabelText)
                    PersonDetailVC.LastName = text;
                else if (TextLabel.Text == Constants.PersonDetail.CellPhoneTextLabel)
                {
                    var t = iOSHelpers.CleanPhoneFormatting(text);
                    if (t.Length > 10)
                        t = t.Remove(9, 1);
                    PersonDetailVC.Cell = t;
                }
                else if (TextLabel.Text == Constants.PersonDetail.EmailLabelText)
                {
                    PersonDetailVC.Email = text;
                }
                else if (TextLabel.Text == Constants.PersonDetail.HomePhoneTextLabel)
                {
                    var t = iOSHelpers.CleanPhoneFormatting(text);
                    if (t.Length > 10)
                        t = t.Remove(9, 1);
                    PersonDetailVC.Home = t;
                }
            }

            if (NoteDetailVC != null)
            {
                if (TextLabel.Text == Constants.Note.TitleText)
                    NoteDetailVC.NoteTitle = text;
            }

            if (this.KeyboardType == UIKeyboardType.PhonePad)
            {
                var numberText = iOSHelpers.CleanPhoneFormatting(text);
                if (numberText.Length < 10)
                    numberText = numberText.PadRight(10, ' ');

                if (!string.IsNullOrWhiteSpace(numberText) && numberText.Length <= 10)
                    EntryTextField.Text = string.Format(Constants.PhoneNumberFormat, double.Parse(numberText));
                else if (string.IsNullOrWhiteSpace(numberText))
                    EntryTextField.Text = "(   )   -    ";

                return false;
            }

            return true;
        }

        public static UITableViewCell Dequeue(BaseViewController cont, UITableView tableView, string labelText, string value, UIKeyboardType keyboardType = UIKeyboardType.Default)
        {
            var cell = (EntryTextCell)tableView.DequeueReusableCell(CellIdentifier);
            cell.TextLabel.Text = labelText;
            cell.EntryTextField.Placeholder = labelText;

            if (string.IsNullOrWhiteSpace(cell.EntryTextField.Text))
                cell.EntryTextField.Text = value;

            cell.ParentVC = cont;
            cell.EntryTextField.Delegate = cell;
            cell.EntryTextField.KeyboardType = keyboardType;
            cell.EntryTextField.AutocapitalizationType = UITextAutocapitalizationType.Words;

            cell.KeyboardType = keyboardType;

            return cell;
        }
    }
}