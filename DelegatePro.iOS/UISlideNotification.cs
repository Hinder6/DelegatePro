/*
https://github.com/eclipsed4utoo/UISlideNotification
 
Copyright (c) 2013 Ryan Alford

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using Foundation;

namespace DelegatePro.iOS
{
    public enum UISlideNotificationPosition
    {
        Top,
        Bottom
    }

    public class UISlideNotification
    {
        public UISlideNotification(UIView parentView, string notificationText)
            : this(parentView, notificationText, false, UISlideNotificationPosition.Bottom)
        {
        }

        public UISlideNotification(UIView parentView, string notificationText, bool showActivitySpinner)
            : this(parentView, notificationText, showActivitySpinner, UISlideNotificationPosition.Bottom)
        {
        }

        public UISlideNotification(UIView parentView, string notificationText, UISlideNotificationPosition position)
            : this(parentView, notificationText, false, position)
        {
        }

        public UISlideNotification(UIView parentView, string notificationText, bool showActivitySpinner, UISlideNotificationPosition position)
        {
            _parentView = parentView;
            _notificationText = notificationText;
            _showActivitySpinner = showActivitySpinner;
            _position = position;
        }


        private UIView _parentView;
        private UIViewController _parentController;
        private string _notificationText;
        private readonly bool _showActivitySpinner;
        private UISlideNotificationPosition _position = UISlideNotificationPosition.Bottom;


        private nfloat _labelHeight = 0;
        private const int StatusBarHeight = 20;
        private const int ToolbarHeight = 44;
        private const int MillisecondsInSecond = 1000;


        public int FontSize { get; set; } = 16;
        public int HorizontalPadding { get; set; } = 12;
        public int VerticalPadding { get; set; } = 14;


        /// <summary>
        /// Gets or sets how long the notification stays open after displaying.
        /// </summary>
        public int NotificationDuration { get; set; } = 3000;

        /// <summary>
        /// Gets or sets the duration of the notification animation in milliseconds.
        /// </summary>
        public int NotificationAnimationDuration { get; set; } = 300;

        /// <summary>
        /// Gets or sets the center point of the activity indicator.
        /// </summary>
        public CGPoint ActivityIndicatorViewCenter { get; set; } = new CGPoint(15, 15);

        /// <summary>
        /// Gets or sets the activity indicator view alpha value.
        /// </summary>
        public float ActivityIndicatorViewAlpha { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the activity indicator view style.
        /// </summary>
        public UIActivityIndicatorViewStyle ActivityIndicatorViewStyle { get; set; } = UIActivityIndicatorViewStyle.White;

        /// <summary>
        /// Gets or sets the background color of the notification.
        /// </summary>
        public UIColor BackgroundColor { get; set; } = UIColor.Black;

        /// <summary>
        /// Gets or sets the text color of the notification text
        /// </summary>
        public UIColor TextColor { get; set; } = UIColor.White;

        /// <summary>
        /// Gets or sets the text alignment of the notification text.
        /// </summary>
        public UITextAlignment TextAlignment { get; set; } = UITextAlignment.Left;

        /// <summary>
        /// Gets or sets the alpha value of the notification.
        /// </summary>
        public float Alpha { get; set; } = 0.8f;

        private nfloat NotificationLabelTop
        {
            get
            {
                _parentController = _parentView.GetParentUiViewController();

                if (_parentController != null)
                {
                    if (_position == UISlideNotificationPosition.Bottom)
                    {
                        if ((_parentController.ToolbarItems != null && _parentController.ToolbarItems.Length > 0) ||
                            (_parentController.NavigationController != null && !_parentController.NavigationController.ToolbarHidden))
                        {
                            return _parentView.Frame.Bottom - ToolbarHeight;
                        }
                    }
                    else if (_position == UISlideNotificationPosition.Top)
                    {
                        if (_parentController.NavigationController != null && !_parentController.NavigationController.NavigationBarHidden)
                        {
                            return _parentView.Frame.Top + _labelHeight;
                        }

                        return _parentView.Frame.Top + StatusBarHeight - _labelHeight;
                    }
                }

                return _position == UISlideNotificationPosition.Bottom ? _parentView.Frame.Bottom : _parentView.Frame.Top;
            }
        }

        private UILabel _notificationLabel;
        private UIActivityIndicatorView _activityView;

        private void SetupUi()
        {
            _notificationLabel = new UILabel(new CGRect(0, 0, _parentView.Frame.Width, 0))
            {
                Lines = 0,
                BackgroundColor = this.BackgroundColor,
                TextColor = this.TextColor,
                Alpha = this.Alpha
            };

            var ps = new NSMutableParagraphStyle
            {
                Alignment = this.TextAlignment,
                FirstLineHeadIndent = this.HorizontalPadding,
                TailIndent = -this.HorizontalPadding,
                HeadIndent = this.HorizontalPadding
            };

            _notificationLabel.AttributedText = new NSAttributedString(
                _notificationText,
                UIFont.SystemFontOfSize(this.FontSize),
                paragraphStyle: ps);

            _notificationLabel.SizeToFit();
            _labelHeight = _notificationLabel.Frame.Height + this.VerticalPadding * 2;
            _notificationLabel.Frame = new CGRect(0, this.NotificationLabelTop, _parentView.Frame.Width, _labelHeight);

            if (_showActivitySpinner)
            {
                _activityView = new UIActivityIndicatorView(this.ActivityIndicatorViewStyle)
                {
                    Alpha = this.ActivityIndicatorViewAlpha,
                    HidesWhenStopped = false,
                    Center = this.ActivityIndicatorViewCenter
                };

                _notificationLabel.AddSubview(_activityView);
            }
        }

        public UISlideNotification Show(bool indefinite = false)
        {
            SetupUi();
            _parentView.AddSubview(_notificationLabel);
            _parentView.BringSubviewToFront(_notificationLabel);
            _notificationLabel.Hidden = false;

            if (_showActivitySpinner)
            {
                _activityView.StartAnimating();
            }

            var newFrame = new CGRect(_notificationLabel.Frame.X, _notificationLabel.Frame.Y, _notificationLabel.Frame.Width, _notificationLabel.Frame.Height);

            if (_position == UISlideNotificationPosition.Bottom)
            {
                newFrame.Y -= _labelHeight;
            }
            else
            {
                newFrame.Y += _labelHeight;
            }

            UIView.Transition(
                _notificationLabel,
                this.NotificationAnimationDuration / MillisecondsInSecond,
                UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                    _notificationLabel.Frame = newFrame;
                },
                () =>
                {
                    if (!indefinite)
                    {
                        var ctx = TaskScheduler.FromCurrentSynchronizationContext();
                        Task.Delay(this.NotificationDuration).ContinueWith(task => Hide(), ctx);
                    }
                }
            );

            return this;
        }

        public void Hide()
        {
            var newFrame = new CGRect(_notificationLabel.Frame.X, _notificationLabel.Frame.Y, _notificationLabel.Frame.Width, _notificationLabel.Frame.Height);

            if (_position == UISlideNotificationPosition.Bottom)
            {
                newFrame.Y += _labelHeight;
            }
            else
            {
                newFrame.Y -= _labelHeight;
            }

            UIView.Transition(
                _notificationLabel,
                this.NotificationAnimationDuration / MillisecondsInSecond,
                UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                    _notificationLabel.Frame = newFrame;
                },
                () =>
                {
                    if (_showActivitySpinner)
                    {
                        _activityView.StopAnimating();
                    }
                    _notificationLabel.Hidden = true;
                    _notificationLabel.RemoveFromSuperview();
                }
            );

        }
    }
}