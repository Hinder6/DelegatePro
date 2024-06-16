using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace DelegatePro.PCL
{
    public enum POIType
    {
        None = 0,
        Employee,
        Manager,
        Union
    }

    public class POI : DataItem, IEquatable<POI>
    {
        [JsonProperty("GUID")]
        public override Guid ID { get; set; }
        [JsonProperty("GUID_Organization")]
        public Guid OriganizationID { get; set; }
        [JsonProperty("Name_First")]
        public string FirstName { get; set; }
        [JsonProperty("Name_Last")]
        public string LastName { get; set; }
        public POIType Type { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public string Home { get; set; }
        [JsonProperty("GUID_CreatedBy")]
        public Guid CreatedByID { get; set; }
        [JsonProperty("DT_Created")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("Name_CreatedBy")]
        public string CreatedByName { get; set; }
        [JsonProperty("GUID_DeletedBy")]
        public Guid? DeletedByID { get; set; }
        [JsonProperty("DT_Deleted")]
        public DateTime? DeletedDate { get; set; }
        [JsonProperty("Name_DeletedBy")]
        public string DeletedByName { get; set; }
        [JsonProperty("DT_Hired")]
        public DateTime? SeniorityDate { get; set; }

        [Ignore, JsonIgnore]
        public bool IsNew { get; set; }

        [Ignore, JsonIgnore]
        public bool IsDeleted 
        {
            get { return DeletedDate.HasValue; }
        }

        [JsonIgnore]
        public bool IsUploaded { get; set; } = true;

        [Ignore, JsonIgnore]
        public bool IsPopulated
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(FirstName) ||
                       !string.IsNullOrWhiteSpace(LastName) ||
                       !string.IsNullOrWhiteSpace(Home) ||
                       !string.IsNullOrWhiteSpace(Cell) ||
                       !string.IsNullOrWhiteSpace(Email) ||
                       Type != POIType.None;
                        
            }
        }

        [Ignore, JsonIgnore]
        public static List<POIType> POITypes
        {
            get
            {
                var types = new List<POIType>();
                foreach (POIType enumValue in Enum.GetValues(typeof(POIType)))
                {
                    if (enumValue == POIType.None)
                        continue;
                    
                    types.Add(enumValue);
                }

                return types;
            }
        }

        public POI()
        {
            IsNew = true;
            Type = POIType.None;
        }

        [Ignore, JsonIgnore]
        public string CellPhoneDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Cell))
                    return string.Empty;

                return string.Format(Constants.PhoneNumberFormat, double.Parse(Cell));
            }
        }

        [Ignore, JsonIgnore]
        public string HomePhoneDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Home))
                    return string.Empty;

                return string.Format(Constants.PhoneNumberFormat, double.Parse(Home));
            }
        }

        [Ignore, JsonIgnore]
        public string TypeDisplay
        {
            get 
            {
                if (Type == POIType.None)
                    return "Select Type";
                
                return Type.ToString("G"); 
            }
        }

        [Ignore, JsonIgnore]
        public RGBWrapper TypeTextColor
        {
            get { return (Type == POIType.None) ? Constants.SelectorTextColor : Constants.DefaultTextColor; }
        }

        [Ignore, JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{LastName}, {FirstName}";
        }

        public static async Task<APIResponse<List<POI>>> GetUsersAsync(POIType type = POIType.None)
        {
            if (!APIHelper.HasInternetConnection || type != POIType.None)
            {
                var users = DataAccess.Instance.GetItems<POI>();
                users.RemoveAll(t => t.IsDeleted);
                users.RemoveAll(t => t.Type != type);
                return APIResponse.CreateWithData(users);
            }

            var response = await APIHelper.MakeApiCall(APICalls.GetUsers);

            if (!response.Result)
            {
                return APIResponse.CreateAsFailure<List<POI>>(response.Message);
            }

            var pois = APIHelper.DataAsList<POI>(response.Data);
            DataAccess.Instance.DeleteTable<POI>();
            DataAccess.Instance.SaveItems(pois);
            return APIResponse.CreateWithData(pois);
        }

        public Result Save()
        {
            var validationResult = Validate();
            if (!validationResult.IsValid)
            {
                return Result.CreateAsFailure(validationResult.Message);
            }

            try
            {
                if (this.ID == Guid.Empty)
                    this.ID = Guid.NewGuid();
                
                this.OriganizationID = AppSettings.CurrentUser.OrganizationID;
                if (this.CreatedByID == Guid.Empty)
                {
                    this.CreatedDate = DateTime.UtcNow;
                    this.CreatedByID = AppSettings.CurrentUser.UserID;
                    this.CreatedByName = AppSettings.CurrentUser.ToString();
                }

                DataAccess.Instance.SaveItem(this);
            }
            catch(Exception ex)
            {
                return Result.CreateAsFailure(ex.Message);
            }

            this.IsNew = false;
            return Result.CreateAsSuccess();
        }

        public async Task<APIResponse> SaveAsync()
        {
            var validationResult = Validate();
            if (!validationResult.IsValid)
            {
                return APIResponse.CreateAsFailure(validationResult.Message);
            }

            try
            {
                var isNew = this.ID == Guid.Empty;

                if (isNew)
                {
                    this.ID = Guid.NewGuid();
                    this.OriganizationID = AppSettings.CurrentUser.OrganizationID;
                    this.CreatedDate = DateTime.UtcNow;
                    this.CreatedByID = AppSettings.CurrentUser.UserID;
                    this.CreatedByName = AppSettings.CurrentUser.ToString();
                }

                if (!APIHelper.HasInternetConnection)
                {
                    this.IsUploaded = false;
                    DataAccess.Instance.SaveItem(this);
                    return APIResponse.CreateAsSuccess();
                }

                var jsonContent = new JsonContent(this);
                var response = await APIHelper.MakeApiCall(APICalls.AddUser, jsonContent);

                if (!response.Result)
                {
                    return response;
                }

                this.IsNew = false;
                return response;
            }
            catch (Exception ex)
            {
                return APIResponse.CreateAsFailure(ex.Message);
            }
        }

        public ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(this.FirstName))
            {
                return ValidationResult.AsFailed(Constants.POI.FirstNameRequiredMessage);
            }

            if (string.IsNullOrWhiteSpace(this.LastName))
            {
                return ValidationResult.AsFailed(Constants.POI.LastNameRequiredMessage);
            }

            if (Type == POIType.None)
            {
                return ValidationResult.AsFailed(Constants.POI.PersonTypeRequiredMessage);
            }

            return ValidationResult.AsValid();
        }

        public async Task<APIResponse> Delete()
        {
            try
            {
                this.DeletedByID = AppSettings.CurrentUser.UserID;
                this.DeletedDate = DateTime.UtcNow;
                this.DeletedByName = AppSettings.CurrentUser.ToString();

                if (!APIHelper.HasInternetConnection)
                {
                    this.IsUploaded = false;
                    DataAccess.Instance.SaveItem(this);
                    return APIResponse.CreateAsSuccess();
                }

                var jsonContent = new JsonContent(this);
                return await APIHelper.MakeApiCall(APICalls.UpdateUser, jsonContent);
            }
            catch(Exception ex)
            {
                return APIResponse.CreateAsFailure(ex.Message);
            }
        }

        private static bool _isSendingInBackground = false;

        public static async Task UploadAsync()
        {
            if (!APIHelper.HasInternetConnection || _isSendingInBackground)
            {
                return;
            }

            var responses = new List<APIResponse>();
            var pois = DataAccess.Instance.GetItems<POI>(t => t.IsUploaded == false);

            Debug.WriteLine($"Uploading {pois.Count} cases");
            if (pois.Count == 0)
                return;

            _isSendingInBackground = true;

            foreach(var poi in pois)
            {
                var response = await poi.SaveAsync();
                responses.Add(response);

                if (!response.Result)
                {
                    continue;
                }

                if (poi.IsDeleted)
                {
                    DataAccess.Instance.DeleteItem(poi);
                }
                else
                {
                    poi.IsUploaded = true;
                    DataAccess.Instance.SaveItem(poi);
                }

            }

            _isSendingInBackground = false;
        }

        #region IEquatable implementation

        public bool Equals(POI other)
        {
            if (other == null)
                return false;

            return other.ID == this.ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is POI))
                return false;

            return Equals(obj as POI);
        }

        #endregion

        //public static async Task UploadPOIsAsync()
        //{
        //    var responses = new List<APIResponse>();
        //    var cases = DataAccess.Instance.GetItems<POI>(t => t.IsUploaded == false);

        //    Debug.WriteLine($"Uploading {cases.Count} cases");
        //    if (cases.Count == 0)
        //        return;

        //    foreach (var newCase in cases)
        //    {
        //        var response = await newCase.Save();
        //        responses.Add(response);

        //        if (!response.Result)
        //        {
        //            continue;
        //        }

        //        DataAccess.Instance.DeleteItem(newCase);
        //    }
        //}
    }
}