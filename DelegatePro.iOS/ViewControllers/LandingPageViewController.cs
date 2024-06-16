using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreGraphics;
using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public partial class LandingPageViewController : BaseViewController, 
        IUITableViewDelegate, 
        IUITableViewDataSource,
        IUISearchBarDelegate,
        IUISearchControllerDelegate
	{
        private UITapGestureRecognizer _tapGesture;
        private List<Case> _cases;
        private const int DefaultRowHeight = 83;
        private Status _selectedStatus = Status.Open;

        private UIBarButtonItem _logoutBarButton;
        private UIBarButtonItem _addBarButton;

        private UIRefreshControl _refresh;
        private UISearchController _searchController;

        private NSObject _keyboardShowObserver;
        private NSObject _keyboardHideObserver;

		public LandingPageViewController (IntPtr handle) : base (handle)
		{
		}

        protected override void DelegateDispose()
        {
            base.DelegateDispose();

            _refresh.ValueChanged -= PullToRefresh;

            ReleaseDesignerOutlets();
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            BackgroundSync.Start();

            FilterBackgroundView.AddBottomBorder();

            _tapGesture = new UITapGestureRecognizer(() => ShowFilter());
            FilterBackgroundView.AddGestureRecognizer(_tapGesture);

            FilterLabel.TextColor = Constants.SelectorTextColor.AsUIColor();

            CasesTableView.WeakDelegate = this;
            CasesTableView.WeakDataSource = this;
            CasesTableView.RowHeight = UITableView.AutomaticDimension;
            CasesTableView.EstimatedRowHeight = DefaultRowHeight;
            CasesTableView.TableFooterView = new UIView(CGRect.Empty);

            _refresh = new UIRefreshControl();
            _refresh.BackgroundColor = UIColor.FromRGB(242, 242, 242);
            _refresh.ValueChanged += PullToRefresh;
            CasesTableView.AddSubview(_refresh);

            _logoutBarButton = new UIBarButtonItem(Constants.SignOutIcon, UIBarButtonItemStyle.Plain, Logout);
            _logoutBarButton.SetTitleTextAttributes(new UITextAttributes { Font = iOSHelpers.FontAwesomeFont(24f) }, UIControlState.Normal);
            this.TabBarController.NavigationItem.LeftBarButtonItems = new UIBarButtonItem[] { _logoutBarButton };

            // kick these off to download the data while loading the cases
            GrievanceStatus.GetStatusesAsync().WithoutAwait();
            POI.GetUsersAsync().WithoutAwait();

            SetupSearch();

            await LoadWithFilter();
        }

        private async void PullToRefresh(object sender, EventArgs e)
        {
            _refresh.BeginRefreshing();

            await LoadWithFilter();

            _refresh.EndRefreshing();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.TabBarController.NavigationItem.Title = Constants.LandingPage.ViewTitle;

            this.DismissToast();

            if (!APIHelper.HasInternetConnection)
            {
                this.ShowToast(this.View, Constants.OfflineMessage, indefinite: true);
            }

            if (_addBarButton == null)
                _addBarButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddCase);

            this.TabBarController.NavigationItem.RightBarButtonItems = new UIBarButtonItem[] { _addBarButton };
        }

        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            RegisterForKeyboardNotifications();

            await Case.UploadCasesAsync();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            UnregisterForKeyboardNotifications();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            this.TabBarController.NavigationItem.Title = string.Empty;

            if (segue.Identifier == Segues.SegueToStatusFilter)
            {
                var filter = (CaseFilterViewController)segue.DestinationViewController;
                filter.StatusSelected += Filter_StatusSelected;
                filter.SelectedStatus = _selectedStatus;
            }
            else if (segue.Identifier == Segues.SegueToCase)
            {
                var caseVC = (CaseViewController)segue.DestinationViewController;

                if (sender == null)
                {
                    caseVC.CurrentCase = new Case();
                }
                else
                {
                    var currentCase = _cases[((NSIndexPath)sender).Row];
                    currentCase.PopulatePOIs();
                    caseVC.CurrentCase = currentCase;
                }   
                
                caseVC.CaseSaved += CaseVC_CaseSaved;
            }
        }

        private async void CaseVC_CaseSaved(object sender, EventArgs e)
        {
            var caseVC = (CaseViewController)sender;
            caseVC.CaseSaved -= CaseVC_CaseSaved;
            await LoadWithFilter();
            this.ShowToast(Constants.CaseView.SavedMessageFormat);
        }

        private async void Filter_StatusSelected(object sender, EventArgs e)
        {
            var vc = (CaseFilterViewController)sender;
            vc.StatusSelected -= Filter_StatusSelected;
            _selectedStatus = vc.SelectedStatus;
            await LoadWithFilter();
        }

        private async void Logout(object sender, EventArgs e)
        {
            var logout = await ShowYesNoMessage(Constants.LogoutMessageTitle, Constants.LogoutMessage);
            if (!logout)
                return;
                
            AppDelegate.Instance.LoginVC.Logout();
        }

        private void AddCase(object sender, EventArgs e)
        {
            PerformSegue(Segues.SegueToCase, null);
        }

        private async Task LoadWithFilter(bool useLocal = false)
        {
            FilterLabel.Text = _selectedStatus.ToString("G");

            if (!useLocal)
            {
                ShowWaitDialog(Constants.LandingPage.LoadingMessage);    
            }

            var response = await Case.GetCasesAsync(_selectedStatus, useLocal);

            if (!useLocal)
            {
                DismissWaitDialog();
            }

            if (!response.Result)
            {
                ShowError(Constants.LandingPage.ErrorTitle, response.Message);
                return;
            }

            _cases = response.Data;

            NoCasesLabel.Hidden = _cases.Count != 0;

            if (_selectedStatus == Status.All)
                NoCasesLabel.Text = "No cases";
            else
                NoCasesLabel.Text = $"No {FilterLabel.Text} cases";

            CasesTableView.ReloadData();
        }

        private void ShowFilter()
        {
            PerformSegue(Segues.SegueToStatusFilter, this);
        }

        #region TableView Methods

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return (_cases != null) ? _cases.Count : 0;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return CaseTableViewCell.Dequeue(tableView, _cases[indexPath.Row]);
        }

        [Export("tableView:heightForRowAtIndexPath:")]
        public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return DefaultRowHeight;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            PerformSegue(Segues.SegueToCase, indexPath);
        }

        [Export("tableView:heightForFooterInSection:")]
        public nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            return 0;
        }

        #endregion

        #region Search

        private void SetupSearch()
        {
            _searchController = SearchControllerHelper.Create(this, this);
            CasesTableView.TableHeaderView = _searchController.SearchBar;

            // Here is why:
            // http://programmingthomas.com/blog/2014/10/5/uisearchcontroller-and-definespresentationcontext
            this.DefinesPresentationContext = false;
        }

        [Export("searchBar:textDidChange:")]
        public void TextChanged(UISearchBar searchBar, string searchText)
        {
            if (searchText.Length < 3 && searchText.Length != 0)
            {
                return;
            }

            PerformSearch();
        }

        [Export("willPresentSearchController:")]
        public virtual async void WillPresentSearchController(UISearchController searchController)
        {
            _refresh.Enabled = false;
            FilterBackgroundView.Hidden = true;
            _selectedStatus = Status.All;
            await LoadWithFilter(useLocal: true);
        }

        [Export("didDismissSearchController:")]
        public virtual async void DidDismissSearchController(UISearchController searchController)
        {
            CasesTableView.ScrollRectToVisible(CasesTableView.TableHeaderView.Frame, true);
            _refresh.Enabled = true;
            FilterBackgroundView.Hidden = false;
            CasesTableView.ContentInset = UIEdgeInsets.Zero;
            await LoadWithFilter();
        }

        [Export("searchBarSearchButtonClicked:")]
        public virtual void SearchButtonClicked(UISearchBar searchBar)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            var searchText = _searchController.SearchBar.Text;
            Debug.WriteLine($"Search text is {searchText}");

            _cases = Case.FilterCasesBySearchTerm(searchText);
            CasesTableView.ReloadData();
        }

        #endregion

        #region Keyboard Handling

        private void RegisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver == null)
                _keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            if (_keyboardHideObserver == null)
                _keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
        }

        private void UnregisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
                _keyboardShowObserver.Dispose();
                _keyboardShowObserver = null;
            }

            if (_keyboardHideObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
                _keyboardHideObserver.Dispose();
                _keyboardHideObserver = null;
            }
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded) return;

            //Check if the keyboard is becoming visible
            var visible = notification.Name == UIKeyboard.WillShowNotification;

            //Pass the notification, calculating keyboard height, etc.
            var keyboardFrame = visible
                ? UIKeyboard.FrameEndFromNotification(notification)
                : CGRect.Empty;

            CasesTableView.ContentInset = new UIEdgeInsets((visible) ? -44 : 0, 0, keyboardFrame.Height, 0);

            if (!visible)
            {
                CasesTableView.UpdateConstraints();
            }
        }

        #endregion
    }

    public partial class CaseTableViewCell : UITableViewCell
    {
        private const string CellReuseID = "CaseTableViewCell";
        public CaseTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public CaseTableViewCell()
            : base(UITableViewCellStyle.Default, CellReuseID)
        {
        }

        public static CaseTableViewCell Dequeue(UITableView tableView, Case caseData)
        {
            var cell = (CaseTableViewCell)tableView.DequeueReusableCell(CellReuseID);
            cell.DateLabel.Text = caseData.DateDisplay;
            cell.IssueLabel.Text = caseData.Issue;
            cell.NotesLabel.Text = caseData.EmployeesInvolvedListDisplay;
            cell.StatusLabel.Text = caseData.StatusDisplay;
            cell.StatusLabel.TextColor = caseData.StatusTextColor.AsUIColor();

            return cell;
        }
    }
}
