// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using CoreGraphics;
using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public partial class PersonListViewController : BaseViewController,
        IUITableViewDelegate,
        IUITableViewDataSource
	{
        private List<POI> _users;

        public POI SelectedPerson { get; set; }

        public event EventHandler PersonSelected;
        private void OnPersonSelected()
        {
            if (PersonSelected != null)
                PersonSelected(this, EventArgs.Empty);
        }

		public PersonListViewController (IntPtr handle) : base (handle)
		{
		}

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            ShowWaitDialog("Loading users...");

            var response = await POI.GetUsersAsync();

            DismissWaitDialog();

            if (!response.Result)
            {
                ShowError("Users", response.Message);
                return;
            }

            _users = response.Data;

            PersonListTableView.WeakDelegate = this;
            PersonListTableView.WeakDataSource = this;
            PersonListTableView.EstimatedRowHeight = UITableView.AutomaticDimension;
            PersonListTableView.TableFooterView = new UIView(CGRect.Empty);
            PersonListTableView.ReloadData();
        }

        [Export("numberOfSectionsInTableView:")]
        public nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        private static string CellIdentifier = "cell";
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier) ?? new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);

            var user = _users[indexPath.Row];
            cell.TextLabel.Text = user.ToString();
            cell.DetailTextLabel.Text = user.TypeDisplay;

            return cell;
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return _users.Count;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            SelectedPerson = _users[indexPath.Row];
            OnPersonSelected();
            NavigationController.PopViewController(true);
        }
	}
}