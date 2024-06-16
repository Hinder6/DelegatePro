// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace DelegatePro.iOS
{
	[Register ("CasePersonnelViewController")]
	partial class CasePersonnelViewController
	{
		[Outlet]
		UIKit.UILabel AllOrgUsersAssignedLabel { get; set; }

		[Outlet]
		UIKit.UITableView CasePersonnelTableView { get; set; }

		[Outlet]
		UIKit.UITableView ExistingPersonTableView { get; set; }

		[Outlet]
		UIKit.UISegmentedControl NewExistingSegment { get; set; }

		[Outlet]
		UIKit.UIView NewPersonContainerView { get; set; }

		[Outlet]
		UIKit.UILabel NoPeopleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CasePersonnelTableView != null) {
				CasePersonnelTableView.Dispose ();
				CasePersonnelTableView = null;
			}

			if (ExistingPersonTableView != null) {
				ExistingPersonTableView.Dispose ();
				ExistingPersonTableView = null;
			}

			if (NewExistingSegment != null) {
				NewExistingSegment.Dispose ();
				NewExistingSegment = null;
			}

			if (NewPersonContainerView != null) {
				NewPersonContainerView.Dispose ();
				NewPersonContainerView = null;
			}

			if (NoPeopleLabel != null) {
				NoPeopleLabel.Dispose ();
				NoPeopleLabel = null;
			}

			if (AllOrgUsersAssignedLabel != null) {
				AllOrgUsersAssignedLabel.Dispose ();
				AllOrgUsersAssignedLabel = null;
			}
		}
	}
}
