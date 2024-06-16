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
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		UIKit.UIButton LoginButton { get; set; }

		[Outlet]
		UIKit.UITextField PasswordTextField { get; set; }

		[Outlet]
		UIKit.UISwitch RememberMeSwitch { get; set; }

		[Outlet]
		UIKit.UITextField UserNameTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (UserNameTextField != null) {
				UserNameTextField.Dispose ();
				UserNameTextField = null;
			}

			if (PasswordTextField != null) {
				PasswordTextField.Dispose ();
				PasswordTextField = null;
			}

			if (RememberMeSwitch != null) {
				RememberMeSwitch.Dispose ();
				RememberMeSwitch = null;
			}

			if (LoginButton != null) {
				LoginButton.Dispose ();
				LoginButton = null;
			}
		}
	}
}
