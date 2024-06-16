using System;
using System.Net;
using KeyChain.Net;
using Newtonsoft.Json;

namespace DelegatePro.PCL
{
    public static class AppSettings
    {
        private static IKeyChainHelper _keyChain;

        public static IKeyChainHelper KeychainHelper
        {
            get
            {
                if (_keyChain == null)
                {
                    throw new NotImplementedException("IKeyChainHelper is not implemented.");
                }

                return _keyChain;
            }
            set { _keyChain = value; }
        }

        private static string _lastUserIDKey = "lastUserID";
        private static string _lastUserNameKey = "lastUserName";
        private static string _lastPasswordKey = "lastPasword";
        private static string _rememberMeKey = "rememberMe";
        private static string _currentUserKey = "currentUser";

        public static int LastUserID
        {
            get
            {
                int userID = 0;
                int.TryParse(KeychainHelper.GetKey(_lastUserIDKey), out userID);
                return userID;
            }
            set { KeychainHelper.SetKey(_lastUserIDKey, value.ToString()); }
        }

        public static string LastUserName
        {
            get
            {
                string userName = string.Empty;
                try
                {
                    userName = KeychainHelper.GetKey(_lastUserNameKey);
                }
                catch { }

                return userName;
            }
            set { KeychainHelper.SetKey(_lastUserNameKey, value); }
        }

        public static string LastPassword
        {
            get
            {
                string password = string.Empty;
                try
                {
                    password = KeychainHelper.GetKey(_lastPasswordKey);
                }
                catch { }

                return password;
            }
            set { KeychainHelper.SetKey(_lastPasswordKey, value); }
        }

        public static bool RememberMe
        {
            get
            {
                var rememberMe = false;
                try
                {
                    rememberMe = bool.Parse(KeychainHelper.GetKey(_rememberMeKey));
                }
                catch { }

                return rememberMe;
            }
            set { KeychainHelper.SetKey(_rememberMeKey, value.ToString()); }
        }

        private static User _currentUser;
        public static User CurrentUser 
        {
            get 
            {
                if (_currentUser == null)
                {
                    var data = KeychainHelper.GetKey(_currentUserKey);
                    if (!string.IsNullOrEmpty(data))
                    {
                        _currentUser = JsonConvert.DeserializeObject<User>(data);
                    }    
                }

                return _currentUser;
            }
            set
            {
                if (value == null)
                {
                    KeychainHelper.DeleteKey(_currentUserKey);
                    return;
                }

                _currentUser = value;

                // don't save if not remembering user
                if (!RememberMe)
                {
                    return;
                }

                var data = JsonConvert.SerializeObject(value);
                KeychainHelper.SaveKey(_currentUserKey, data);
            }
        }

        public static string WebServiceURL
        {
            get { return "http://adensol.selfip.com/DelegatePro/"; }
        }

        public static void Logout()
        {
            LastUserName = string.Empty;
            LastPassword = string.Empty;
            RememberMe = false;
        }
    }
}