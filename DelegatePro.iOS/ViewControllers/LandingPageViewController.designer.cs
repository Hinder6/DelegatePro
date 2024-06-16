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
	[Register ("LandingPageViewController")]
	partial class LandingPageViewController
	{
		[Outlet]
		UIKit.UITableView CasesTableView { get; set; }

		[Outlet]
		UIKit.UIView FilterBackgroundView { get; set; }

		[Outlet]
		UIKit.UILabel FilterLabel { get; set; }

		[Outlet]
		UIKit.UILabel NoCasesLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CasesTableView != null) {
				CasesTableView.Dispose ();
				CasesTableView = null;
			}

			if (FilterBackgroundView != null) {
				FilterBackgroundView.Dispose ();
				FilterBackgroundView = null;
			}

			if (FilterLabel != null) {
				FilterLabel.Dispose ();
				FilterLabel = null;
			}

			if (NoCasesLabel != null) {
				NoCasesLabel.Dispose ();
				NoCasesLabel = null;
			}
		}
	}

	[Register ("CaseTableViewCell")]
	partial class CaseTableViewCell
	{
		[Outlet]
		UIKit.UILabel DateLabel { get; set; }

		[Outlet]
		UIKit.UILabel IssueLabel { get; set; }

		[Outlet]
		UIKit.UILabel NotesLabel { get; set; }

		[Outlet]
		UIKit.UILabel StatusLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DateLabel != null) {
				DateLabel.Dispose ();
				DateLabel = null;
			}

			if (IssueLabel != null) {
				IssueLabel.Dispose ();
				IssueLabel = null;
			}

			if (NotesLabel != null) {
				NotesLabel.Dispose ();
				NotesLabel = null;
			}

			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}
		}
	}
}
