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
	[Register ("NewPersonViewController")]
	partial class NewPersonViewController
	{
		[Outlet]
		UIKit.UITextField CellTextField { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CellWidthConstraint { get; set; }

		[Outlet]
		UIKit.UITextField EmailTextField { get; set; }

		[Outlet]
		UIKit.UITextField FirstNameTextField { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint FirstNameWidthConstraint { get; set; }

		[Outlet]
		UIKit.UITextField LastNameTextField { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint LastNameWidthConstraint { get; set; }

		[Outlet]
		UIKit.UITextField PhoneTextField { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint PhoneWidthConstraint { get; set; }

		[Outlet]
		UIKit.UITextField SeniorityDateTextField { get; set; }

		[Outlet]
		UIKit.UITextField TypeInputField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CellTextField != null) {
				CellTextField.Dispose ();
				CellTextField = null;
			}

			if (EmailTextField != null) {
				EmailTextField.Dispose ();
				EmailTextField = null;
			}

			if (FirstNameTextField != null) {
				FirstNameTextField.Dispose ();
				FirstNameTextField = null;
			}

			if (LastNameTextField != null) {
				LastNameTextField.Dispose ();
				LastNameTextField = null;
			}

			if (PhoneTextField != null) {
				PhoneTextField.Dispose ();
				PhoneTextField = null;
			}

			if (SeniorityDateTextField != null) {
				SeniorityDateTextField.Dispose ();
				SeniorityDateTextField = null;
			}

			if (TypeInputField != null) {
				TypeInputField.Dispose ();
				TypeInputField = null;
			}

			if (LastNameWidthConstraint != null) {
				LastNameWidthConstraint.Dispose ();
				LastNameWidthConstraint = null;
			}

			if (FirstNameWidthConstraint != null) {
				FirstNameWidthConstraint.Dispose ();
				FirstNameWidthConstraint = null;
			}

			if (PhoneWidthConstraint != null) {
				PhoneWidthConstraint.Dispose ();
				PhoneWidthConstraint = null;
			}

			if (CellWidthConstraint != null) {
				CellWidthConstraint.Dispose ();
				CellWidthConstraint = null;
			}
		}
	}
}
