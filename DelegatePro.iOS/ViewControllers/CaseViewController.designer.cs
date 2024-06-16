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
	[Register ("CaseViewController")]
	partial class CaseViewController
	{
		[Outlet]
		UIKit.UIView DateContainerView { get; set; }

		[Outlet]
		UIKit.UIView EmployeesContainerView { get; set; }

		[Outlet]
		UIKit.UIView GrievanceStatusContainerView { get; set; }

		[Outlet]
		UIKit.UIView IssueContainerView { get; set; }

		[Outlet]
		UIKit.UIScrollView MainScrollView { get; set; }

		[Outlet]
		UIKit.UIView ManagersContainerView { get; set; }

		[Outlet]
		UIKit.UIView NotesContainerView { get; set; }

		[Outlet]
		UIKit.UIView StatusContainerView { get; set; }

		[Outlet]
		UIKit.UIView UnionDelegatesContainerView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DateContainerView != null) {
				DateContainerView.Dispose ();
				DateContainerView = null;
			}

			if (EmployeesContainerView != null) {
				EmployeesContainerView.Dispose ();
				EmployeesContainerView = null;
			}

			if (GrievanceStatusContainerView != null) {
				GrievanceStatusContainerView.Dispose ();
				GrievanceStatusContainerView = null;
			}

			if (IssueContainerView != null) {
				IssueContainerView.Dispose ();
				IssueContainerView = null;
			}

			if (MainScrollView != null) {
				MainScrollView.Dispose ();
				MainScrollView = null;
			}

			if (ManagersContainerView != null) {
				ManagersContainerView.Dispose ();
				ManagersContainerView = null;
			}

			if (NotesContainerView != null) {
				NotesContainerView.Dispose ();
				NotesContainerView = null;
			}

			if (StatusContainerView != null) {
				StatusContainerView.Dispose ();
				StatusContainerView = null;
			}

			if (UnionDelegatesContainerView != null) {
				UnionDelegatesContainerView.Dispose ();
				UnionDelegatesContainerView = null;
			}
		}
	}
}
