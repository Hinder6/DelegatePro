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
	[Register ("SwitchCell")]
	partial class SwitchCell
	{
		[Outlet]
		UIKit.UISwitch Switch { get; set; }

		[Outlet]
		UIKit.UILabel TextLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TextLabel != null) {
				TextLabel.Dispose ();
				TextLabel = null;
			}

			if (Switch != null) {
				Switch.Dispose ();
				Switch = null;
			}
		}
	}
}
