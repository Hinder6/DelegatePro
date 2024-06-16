using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DelegatePro.PCL
{
    public class User
    {
        [JsonProperty("GUID")]
        public Guid UserID { get; set; }
        [JsonProperty("GUID_Organization")]
        public Guid OrganizationID { get; set; }
        [JsonProperty("MobileToken")]
        public Guid Token { get; set; }
        [JsonProperty("Name_Organization")]
        public string OrganizationName { get; set; }
        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("Email")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [JsonProperty("Name_First")]
        public string FirstName { get; set; }
        [JsonProperty("Name_Last")]
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }

        public async Task<APIResponse> Login(bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(this.UserName) || string.IsNullOrWhiteSpace(this.Password))
            {
                return APIResponse.CreateAsFailure(Constants.Login.UserNameAndPasswordRequiredMessage);
            }

            if (!APIHelper.HasInternetConnection && rememberMe && !string.IsNullOrEmpty(AppSettings.LastUserName))
            {
                return APIResponse.CreateAsSuccess();
            }

            var data = new
            {
                Email = this.UserName,
                Password = this.Password
            };

            var response = await APIHelper.MakeApiCall(APICalls.Login, new JsonContent(data));

            if (!response.Result)
            {
                return response;
            }

            var user = APIHelper.DataAsObject<User>(response.Data);

            AppSettings.RememberMe = rememberMe;
            AppSettings.CurrentUser = user;
            return APIResponse.CreateAsSuccess();
        }

        public static async Task<APIResponse> ResetPassword(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return APIResponse.CreateAsFailure(Constants.ResetPassword.UserNameRequiredMessage);
            }

            await Task.Delay(2000);

            return APIResponse.CreateAsSuccess();
        }
    }
}

