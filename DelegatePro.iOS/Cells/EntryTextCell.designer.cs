// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace DelegatePro.iOS
{
    [Register("EntryTextCell")]
    partial class EntryTextCell
    {
        [Outlet]
        UIKit.UITextField EntryTextField { get; set; }

        [Outlet]
        UIKit.UILabel TextLabel { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (EntryTextField != null)
            {
                EntryTextField.Dispose();
                EntryTextField = null;
            }

            if (TextLabel != null)
            {
                TextLabel.Dispose();
                TextLabel = null;
            }
        }
    }
}