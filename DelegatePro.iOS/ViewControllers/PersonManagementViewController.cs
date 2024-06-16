using System;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;
using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public partial class PersonManagementViewController : BaseViewController,
        IUITableViewDelegate,
        IUITableViewDataSource
	{
        public Case CurrentCase { get; set; }
        //public List<CasePOI> Persons { get; set; }
        private List<CasePOI> _persons;

        private List<CasePOI> _activePersonList;

        private UIBarButtonItem _addBarButton;

        private bool _personSaved;

        public event EventHandler PersonSaved;
        private void OnPersonSaved()
        {
            if (PersonSaved != null)
                PersonSaved(this, EventArgs.Empty);
        }

        public event EventHandler PersonDeleted;
        private void OnPersonDeleted()
        {
            if (PersonDeleted != null)
                PersonDeleted(this, EventArgs.Empty);
        }

        protected override void DelegateDispose()
        {
            base.DelegateDispose();

            if (_personSaved)
                OnPersonSaved();

            ReleaseDesignerOutlets();
        }

		public PersonManagementViewController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UpdatePersonsList();

            PersonTableView.TableFooterView = new UIView(CGRect.Empty);
            PersonTableView.WeakDataSource = this;
            PersonTableView.WeakDelegate = this;
            PersonTableView.ReloadData();

            _addBarButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddPerson);
            NavigationItem.RightBarButtonItems = new UIBarButtonItem[] { _addBarButton };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.Title = "Case Personnel";

            NoPeopleLabel.Hidden = _persons.Count > 0;

            this.DismissToast();

            if (!APIHelper.HasInternetConnection)
            {
                this.ShowToast(this.View, Constants.OfflineMessage, indefinite: true);
            }
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            this.Title = " ";
            switch(segue.Identifier)
            {
                case Segues.SegueToAddPerson:

                    var isEdit = (sender != null && sender is NSIndexPath);
                    var indexPath = sender as NSIndexPath;

                    CasePOI person = null;
                    if (isEdit)
                        person = _persons[indexPath.Row];
                    else
                        person = CasePOI.Create();

                    var detailVC = (PersonDetailViewController)segue.DestinationViewController;
                    detailVC.CurrentPerson = person;
                    detailVC.CurrentCase = CurrentCase;
                    detailVC.PersonSaved += DetailVC_PersonSaved;
                    break;
            }
        }

        private void DetailVC_PersonSaved(object sender, EventArgs e)
        {
            var vc = (PersonDetailViewController)sender;
            vc.PersonSaved -= DetailVC_PersonSaved;

            UpdatePersonsList();

            PersonTableView.ReloadData();
            _personSaved = true;
        }

        private void AddPerson(object sender, EventArgs e)
        {
            PerformSegue(Segues.SegueToAddPerson, this);
        }

        private void UpdatePersonsList()
        {
            _persons = CurrentCase.AllPeopleInvolved;
            _activePersonList = _persons.Where(t => !t.IsDeleted).ToList();

            NoPeopleLabel.Hidden = _activePersonList.Count > 0;
        }

        [Export("tableView:canEditRowAtIndexPath:")]
        public bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        [Export("tableView:commitEditingStyle:forRowAtIndexPath:")]
        public async void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            if (editingStyle == UITableViewCellEditingStyle.Delete)
            {
                var delete = await ShowYesNoMessage(Constants.POI.RemovePersonTitleText, Constants.POI.RemovePersonFromCaseText);
                if (!delete)
                    return;
                
                var person = _activePersonList[indexPath.Row];

                CurrentCase.RemovePerson(person);

                UpdatePersonsList();
                PersonTableView.ReloadData();

                OnPersonDeleted();
            }
        }

        [Export("numberOfSectionsInTableView:")]
        public nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return _activePersonList.Count;
        }

        private const string CellIdentifier = "Cell";
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier);
            cell = cell ?? new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);

            var person = _activePersonList[indexPath.Row];
            cell.TextLabel.Text = person.POI.ToString();
            cell.DetailTextLabel.Text = person.POI.TypeDisplay;

            return cell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            PerformSegue(Segues.SegueToAddPerson, indexPath);
        }
    }
}