using System;
using CoreAnimation;
using CoreGraphics;
using DelegatePro.PCL;
using Foundation;
using UIKit;

namespace DelegatePro.iOS
{
    public static class iOSHelpers
    {
        public static void AddTopBorder(this UIView view)
        {
            var border = new CALayer();
            border.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 1f);
            border.BackgroundColor = UIColor.FromRGB(240, 240, 240).CGColor;
            view.Layer.AddSublayer(border);
        }

        public static void AddBottomBorder(this UIView view)
        {
            var border = new CALayer();
            border.Frame = new CGRect(0, view.Frame.Height - 1f, UIScreen.MainScreen.Bounds.Width + 5f, 1f);
            border.BackgroundColor = UIColor.FromRGB(240, 240, 240).CGColor;
            view.Layer.AddSublayer(border);
        }

        public static void AddBorder(this UIView view)
        {
            var layer = view.Layer;
            layer.BorderWidth = 1f;
            layer.BorderColor = UIColor.FromRGB(240, 240, 240).CGColor;
        }

        public static UIColor AsUIColor(this RGBWrapper rgb)
        {
            return rgb.A == byte.MaxValue
                ? UIColor.FromRGB(rgb.R, rgb.G, rgb.B)
                : UIColor.FromRGBA(rgb.R, rgb.G, rgb.B, rgb.A);
        }

        public static CGColor AsCGColor(this RGBWrapper w)
        {
            return new CGColor(w.R / 255f, w.G / 255f, w.B / 255f, w.A / 255f);
        }

        public static DateTime ToDateTime(this NSDate date)
        {
            DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
                new DateTime(2001, 1, 1, 0, 0, 0));
            return reference.AddSeconds(date.SecondsSinceReferenceDate);
        }

        public static NSDate ToNSDate(this DateTime date)
        {
            DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
                new DateTime(2001, 1, 1, 0, 0, 0));
            return NSDate.FromTimeIntervalSinceReferenceDate(
                (date - reference).TotalSeconds);
        }

        public static UIFont FontAwesomeFont(nfloat size)
        {
            return UIFont.FromName("FontAwesome", size);
        }

        public static string DatabaseDirectoryPath
        {
            get 
            { 
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return documentsPath + "/";
            }   
        }

        public static UIViewController GetParentUiViewController(this UIView view)
        {
            var nextResponder = view.NextResponder;
            if (nextResponder is UIViewController)
            {
                return (UIViewController)nextResponder;
            }
            if (nextResponder is UIView)
            {
                return ((UIView)nextResponder).GetParentUiViewController();
            }
            return null;
        }

        private static UISlideNotification _slideNoti;
        public static UISlideNotification ShowToast(this UIViewController viewController, string text, bool indefinite = false)
        {
            if (viewController.NavigationController == null)
            {
                throw new Exception("Navigation Controller not set");
            }

            return ShowToast(viewController, viewController.NavigationController.View, text, indefinite);
        }

        public static UISlideNotification ShowToast(this UIViewController viewController, UIView parentView, string text, bool indefinite = false)
        {
            if (indefinite)
            {
                _slideNoti = new UISlideNotification(parentView, text).Show(indefinite);
                return _slideNoti;
            }

            return new UISlideNotification(parentView, text).Show();
        }

        public static void DismissToast(this UIViewController viewController)
        {
            if (_slideNoti != null)
                _slideNoti.Hide();
        }

        public static string CleanPhoneFormatting(string value)
        {
            return value.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("x", "").Replace("-", "");
        }
    }
}

