using System;
using DelegatePro.PCL;
using Foundation;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using UIKit;

namespace DelegatePro.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public static AppDelegate Instance
        {
            get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
        }

        public override UIWindow Window
        {
            get;
            set;
        }

        public LoginViewController LoginVC
        {
            get { return (LoginViewController)((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).ViewControllers[0]; }
        }

        public UINavigationController MainNavController
        {
            get { return (UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController; }
        }

        private async void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var viewController = (MainNavController != null)
                                   ? MainNavController.VisibleViewController
                                   : LoginVC;
            
            if (e.IsConnected)
            {
                viewController.DismissToast();

                if (viewController != LoginVC)
                {
                    // user is not on the login, so they are logged into the app. send cases that
                    //   haven't been uploaded yet.
                    await Case.UploadCasesAsync();
                }
            }
            else
            {
                viewController.ShowToast(viewController.View, Constants.OfflineMessage, indefinite: true);    
            }
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            AppSettings.KeychainHelper = new KeyChain.Net.XamarinIOS.KeyChainHelper();

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.

            CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

            var viewController = (MainNavController != null)
                                   ? MainNavController.VisibleViewController
                                   : LoginVC;

            if (APIHelper.HasInternetConnection)
            {
                viewController.DismissToast();
            }
            else
            {
                viewController.ShowToast(LoginVC.View, Constants.OfflineMessage, indefinite: true);
            }
        }

        public override void WillTerminate(UIApplication application)
        {
            DataAccess.Instance.CloseConnection();
            BackgroundSync.Stop();
        }
    }
}