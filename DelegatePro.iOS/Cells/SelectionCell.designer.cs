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
    [Register("SelectionCell")]
    partial class SelectionCell
    {
        [Outlet]
        UIKit.UILabel SelectionLabel { get; set; }

        [Outlet]
        UIKit.UILabel TextLabel { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (SelectionLabel != null)
            {
                SelectionLabel.Dispose();
                SelectionLabel = null;
            }

            if (TextLabel != null)
            {
                TextLabel.Dispose();
                TextLabel = null;
            }
        }
    }
}
