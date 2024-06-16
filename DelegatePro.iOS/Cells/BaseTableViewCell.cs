using System;
using UIKit;

namespace DelegatePro.iOS
{
    public class BaseTableViewCell : UITableViewCell
    {
        private WeakReference _parentVC;
        protected BaseViewController ParentVC
        {
            get { return (BaseViewController)_parentVC.Target; }
            set { _parentVC = new WeakReference(value); }
        }

        public BaseTableViewCell(IntPtr handle) : base(handle)
        {
        }
    }
}
