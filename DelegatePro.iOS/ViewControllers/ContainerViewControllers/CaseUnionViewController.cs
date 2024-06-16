using System;
using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public partial class CaseUnionViewController : BaseContainerViewController
	{
        public Case CurrentCase { get; set; }

        public CaseUnionViewController (IntPtr handle) : base (handle)
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

            UnionDelegatesLabel.AddBorder();
            ReloadUI();
        }

        public void ReloadUI()
        {
            UnionDelegatesLabel.TextColor = CurrentCase.UnionDelegatesInvolvedTextColor.AsUIColor();
            UnionDelegatesLabel.Text = CurrentCase.UnionDelegatesInvolvedDisplay;
        }
    }
}
