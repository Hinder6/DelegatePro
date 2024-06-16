using System;
using System.Threading.Tasks;
using DelegatePro.PCL;
using Foundation;
using SQLite.Net.Platform.XamarinIOS;
using UIKit;

namespace DelegatePro.iOS
{
    public partial class LoginViewController : BaseViewController
	{
        private UITapGestureRecognizer _tapGesture;

        private string UserName
        {
            get { return UserNameTextField.Text; }
            set { UserNameTextField.Text = value; }
        }

        private string Password
        {
            get { return PasswordTextField.Text; }
            set { PasswordTextField.Text = value; }
        }

        private bool RememberMe
        {
            get { return RememberMeSwitch.On; }
            set { RememberMeSwitch.On = value; }
        }

		public LoginViewController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            LoginButton.TouchUpInside += LoginButton_TouchUpInside;
        }

        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.NavigationController.NavigationBarHidden = true;

            RememberMe = AppSettings.RememberMe;
            UserName = AppSettings.LastUserName;
            Password = AppSettings.LastPassword;

            _tapGesture = new UITapGestureRecognizer(() =>
            {
                this.View.EndEditing(true);
            });
            this.View.AddGestureRecognizer(_tapGesture);

            if (AppSettings.RememberMe)
            {
                await Task.Delay(200);
                await Login();
            }

#if DEBUG
            else
            {
                RememberMe = true;
                //UserName = "dxp@adenhold.com";
                //Password = "demo";
                UserName = "whinderliter@me.com";
                Password = "password";
                await Task.Delay(200);
                await Login();
            }
#endif
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            this.DismissToast();

            if (!APIHelper.HasInternetConnection)
            {
                this.ShowToast(this.View, Constants.OfflineMessage, indefinite: true);
            }
        }

        private async void LoginButton_TouchUpInside(object sender, EventArgs e)
        {
            await Login();
        }

        private async Task Login()
        {
            ToggleUI(isEnabled: false);
            ShowWaitDialog(Constants.Login.LoginMessage);

            var user = new User
            {
                UserName = this.UserName,
                Password = this.Password
            };

            APIResponse response = null;

#if DEBUG
            if (APIHelper.HasInternetConnection)
            {
                response = await user.Login(RememberMe);
            }
            else
            {
                response = new APIResponse { Result = true };
            }
#else
            response = await user.Login(RememberMe);
#endif

            DismissWaitDialog();

            if (!response.Result)
            {
                ToggleUI(isEnabled: true);
                ShowError(Constants.Login.LoginErrorTitle, response.Message);
                return;
            }

            DataAccess.GetInstance(new SQLitePlatformIOS(), iOSHelpers.DatabaseDirectoryPath, UserName);

            AppSettings.RememberMe = RememberMe;
            AppSettings.LastUserName = (AppSettings.RememberMe) ? UserName : string.Empty;
            AppSettings.LastPassword = (AppSettings.RememberMe) ? Password : string.Empty;

            PerformSegue(Segues.SegueToLandingPage, this);
        }

        public void Logout()
        {
            ToggleUI(isEnabled: true);
            AppSettings.Logout();
            DismissViewController(true, null);
            BackgroundSync.Stop();
        }

        private void ToggleUI(bool isEnabled)
        {
            LoginButton.Enabled = isEnabled;
            UserNameTextField.Enabled = isEnabled;
            PasswordTextField.Enabled = isEnabled;
            RememberMeSwitch.Enabled = isEnabled;
        }
    }
}
