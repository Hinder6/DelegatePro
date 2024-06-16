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
	[Register ("NotesTableViewCell")]
	partial class NotesTableViewCell
	{
		[Outlet]
		UIKit.UILabel NotesAddressedLabel { get; set; }

		[Outlet]
		UIKit.UILabel NotesLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NotesLabel != null) {
				NotesLabel.Dispose ();
				NotesLabel = null;
			}

			if (NotesAddressedLabel != null) {
				NotesAddressedLabel.Dispose ();
				NotesAddressedLabel = null;
			}
		}
	}
}
