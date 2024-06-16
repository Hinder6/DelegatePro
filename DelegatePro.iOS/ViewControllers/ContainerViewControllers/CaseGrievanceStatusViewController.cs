using System;
using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public partial class CaseGrievanceStatusViewController : BaseContainerViewController
	{
        public Case CurrentCase { get; set; }

		public CaseGrievanceStatusViewController (IntPtr handle) : base (handle)
		{
		}

        protected override void DelegateDispose()
        {
            base.DelegateDispose();

            ReleaseDesignerOutlets();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ReloadUI();
        }

        public void ReloadUI()
        {
            StatusLabel.Text = (CurrentCase.Grievance != null) ? CurrentCase.Grievance.Name : "Select Status";
        }
	}
}
