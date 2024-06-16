using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using SQLite.Net.Attributes;

namespace DelegatePro.PCL
{
    public class Case : DataItem, IEquatable<Case>
    {
        public Case()
        {
            IsUploaded = false;
        }

        [JsonProperty("GUID")]
        public override Guid ID { get; set; }

        public int CaseID { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [Collation("nocase")]
        public string Issue { get; set; }
        public Status Status { get; set; } = Status.Open;

        [JsonProperty("GUID_Organization")]
        public Guid OrganizationID { get; set; }
        public int GrievanceStatus { get; set; }

        private string _notes;
        [Collation("nocase")]
        [Ignore]
        public string Notes
        {
            get 
            {
                return _notes ?? (_notes = Note?.Text);
            }
            set
            {
                _notes = value;
            }
        }

        [JsonProperty("GUID_Note")]
        public Guid? NoteID { get; set; }

        [Ignore]
        public Note Note { get; set; }

        [JsonProperty("GUID_CreatedBy")]
        public Guid CreatedByID { get; set; }
        [JsonProperty("DT_Created")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("Name_CreatedBy")]
        public string CreatedByName { get; set; }

        [JsonProperty("DT_Closed")]
        public DateTime? ClosedDate { get; set; }
        [JsonProperty("GUID_ClosedBy")]
        public Guid? ClosedByID { get; set; }
        [JsonProperty("Name_ClosedBy")]
        public string ClosedByName { get; set; }

        [JsonProperty("DT_LastUpdated")]
        public DateTime LastUpdatedDate { get; set; }
        [JsonProperty("GUID_LastUpdatedBy")]
        public Guid LastUpdatedByID { get; set; }
        [JsonProperty("Name_LastUpdatedBy")]
        public string LastUpdatedByName { get; set; }

        [JsonIgnore]
        public bool IsUploaded { get; set; } = true;

        [Ignore, JsonIgnore]
        public bool IsNew 
        {
            get { return ID == Guid.Empty; }
        }

        [Ignore]
        public List<CasePOI> ExistingPeople { get; set; } = new List<CasePOI>();

        [Ignore, JsonIgnore]
        public List<CasePOI> NewExistingPeople { get; set; } = new List<CasePOI>();

        [Ignore]
        [JsonIgnore]
        public List<CasePOI> NewPeople { get; set; } = new List<CasePOI>();

        [Ignore]
        [JsonProperty("NewPeople")]
        public List<POI> NewPeopleForUpload 
        {
            get { return NewPeople.Select(t => t.POI).ToList(); }
        }

        [Ignore, JsonIgnore]
        public List<CasePOI> DeletedPeople { get; set; } = new List<CasePOI>();

        private GrievanceStatus _grievanceStatus;
        [Ignore, JsonIgnore]
        public GrievanceStatus Grievance
        {
            get
            {
                if (_grievanceStatus != null)
                    return _grievanceStatus;

                _grievanceStatus = DataAccess.Instance.GetItemInt<GrievanceStatus>(this.GrievanceStatus);
                return _grievanceStatus;
            }
            set
            {
                _grievanceStatus = value;
                if (_grievanceStatus == null)
                    GrievanceStatus = 0;
                else
                    GrievanceStatus = _grievanceStatus.ID;
            }
        }

        #region Color Properties

        [Ignore, JsonIgnore]
        public RGBWrapper EmployeesInvolvedTextColor
        {
            get { return (EmployeesInvolved.Count != 0) ? Constants.DefaultTextColor : Constants.PlaceholderTextColor; }
        }

        [Ignore, JsonIgnore]
        public RGBWrapper ManagersInvolvedTextColor
        {
            get { return (ManagersInvolved.Count != 0) ? Constants.DefaultTextColor : Constants.PlaceholderTextColor; }
        }

        [Ignore, JsonIgnore]
        public RGBWrapper UnionDelegatesInvolvedTextColor
        {
            get { return (UnionDelegatesInvolved.Count != 0) ? Constants.DefaultTextColor : Constants.PlaceholderTextColor; }
        }

        [Ignore, JsonIgnore]
        public RGBWrapper StatusTextColor
        {
            get { return (Status == Status.Open) ? Constants.OpenStatusColor : Constants.ClosedStatusColor; }
        }

        #endregion

        [Ignore, JsonIgnore]
        public List<CasePOI> AllPeopleInvolved
        {
            get 
            {
                return EmployeesInvolved.Union(ManagersInvolved).Union(UnionDelegatesInvolved).ToList();
            }
        }

        [Ignore, JsonIgnore]
        public List<CasePOI> EmployeesInvolved
        {
            get 
            {
                var existing = ExistingPeople.Where(t => t.POI.Type == POIType.Employee && t.IsDeleted == false).ToList();
                var newExisting = NewExistingPeople.Where(t => t.POI.Type == POIType.Employee && t.IsDeleted == false).ToList();
                var newPeople = NewPeople.Where(t => t.POI.Type == POIType.Employee && t.IsDeleted == false).ToList();

                return existing.Union(newExisting).Union(newPeople).ToList();
            }
        }

        [Ignore, JsonIgnore]
        public List<CasePOI> ManagersInvolved
        {
            get
            {
                var existing = ExistingPeople.Where(t => t.POI.Type == POIType.Manager && t.IsDeleted == false).ToList();
                var newExisting = NewExistingPeople.Where(t => t.POI.Type == POIType.Manager && t.IsDeleted == false).ToList();
                var newPeople = NewPeople.Where(t => t.POI.Type == POIType.Manager && t.IsDeleted == false).ToList();

                return existing.Union(newExisting).Union(newPeople).ToList();
            }
        }

        [Ignore, JsonIgnore]
        public List<CasePOI> UnionDelegatesInvolved
        {
            get
            {
                var existing = ExistingPeople.Where(t => t.POI.Type == POIType.Union && t.IsDeleted == false).ToList();
                var newExisting = NewExistingPeople.Where(t => t.POI.Type == POIType.Union && t.IsDeleted == false).ToList();
                var newPeople = NewPeople.Where(t => t.POI.Type == POIType.Union && t.IsDeleted == false).ToList();

                return existing.Union(newExisting).Union(newPeople).ToList();
            }
        }

        #region Display Properties

        [Ignore, JsonIgnore]
        public string SelectionDisplay 
        {
            get 
            {
                if (Issue.Length <= 30)
                    return $"{CaseID} - {Issue}";
                
                return $"{CaseID} - {Issue.Substring(0, 30)}...";
            }
        }

        [Ignore, JsonIgnore]
        public string EmployeesInvolvedListDisplay
        {
            get
            {
                var list = EmployeesInvolved;

                if (list.Count == 0)
                {
                    return string.Empty;
                }

                return string.Join(", ", list.Select(t => t.ToString()));
            }
        }

        [Ignore, JsonIgnore]
        public string EmployeesInvolvedDisplay
        {
            get 
            {
                var list = EmployeesInvolved;

                if (list.Count == 0)
                {
                    return Constants.CaseView.NoEmployeesAdded;
                }

                return string.Join(", ", list.Select(t => t.ToString()));
            }
        }

        [Ignore, JsonIgnore]
        public string ManagersInvolvedDisplay
        {
            get
            {
                var list = ManagersInvolved;

                if (list.Count() == 0)
                {
                    return Constants.CaseView.NoManagersAdded;
                }

                return string.Join(", ", list.Select(t => t.ToString()));
            }
        }

        [Ignore, JsonIgnore]
        public string UnionDelegatesInvolvedDisplay
        {
            get
            {
                var list = UnionDelegatesInvolved;

                if (list.Count() == 0)
                {
                    return Constants.CaseView.NoUnionAdded;
                }

                return string.Join(", ", list.Select(t => t.ToString()));
            }
        }

        [Ignore, JsonIgnore]
        public string IssueDisplayForTitle
        {
            get
            {
                if (string.IsNullOrEmpty(Issue))
                    return Constants.CaseView.NewCaseTitle;
                
                return (Issue.Length <= 18) ? Issue : Issue.Substring(0, 18) + "...";
            }
        }

        [Ignore, JsonIgnore]
        public string DateDisplay
        {
            get { return (CreatedDate != DateTime.MinValue) ? CreatedDate.ToString("MM/dd/yyyy") : DateTime.Now.ToString("MM/dd/yyyy"); }
        }

        [Ignore, JsonIgnore]
        public string StatusDisplay
        {
            get { return Status.ToString("G"); }
        }

        #endregion

        /// <summary>
        /// Gets the statuses available to be filtered by.
        /// </summary>
        /// <value>The filterable statuses.</value>
        [Ignore, JsonIgnore]
        public static List<Status> FilterableStatuses
        {
            get
            {
                var statuses = new List<Status>();
                foreach (Status enumValue in Enum.GetValues(typeof(Status)))
                {
                    statuses.Add(enumValue);
                }

                return statuses;
            }
        }


        /// <summary>
        /// Gets the statuses available to be chosen as a case status.
        /// </summary>
        /// <value>The available statuses.</value>
        [Ignore, JsonIgnore]
        public static List<Status> AvailableStatuses
        {
            get 
            {
                var statuses = new List<Status>();
                foreach (Status enumValue in Enum.GetValues(typeof(Status)))
                {
                    if ((int)enumValue <= 0)
                        continue;
                    
                    statuses.Add(enumValue);
                }

                return statuses;
            }
        }

        /// <summary>
        /// Adds Person of Interest to the in-memory collection
        /// </summary>
        /// <param name="poi">POI instance</param>
        public void AddExistingPerson(CasePOI poi)
        {
            if (NewExistingPeople.Contains(poi))
                return;

            poi.CaseID = this.ID;
            NewExistingPeople.Add(poi);
        }

        public void UpdateExistingPerson(CasePOI poi)
        {
            var existing = ExistingPeople.First(t => t.ID == poi.ID);
            ExistingPeople.Remove(existing);
            poi.IsEdited = true;
            ExistingPeople.Add(poi);
        }

        public void AddNewPerson(CasePOI poi)
        {
            poi.CaseID = this.ID;
            poi.CreatedByID = AppSettings.CurrentUser.UserID;
            poi.CreatedByName = AppSettings.CurrentUser.ToString();
            poi.CreatedDate = DateTime.UtcNow;
            NewPeople.Add(poi);
        }

        public void UpdateNewPerson(CasePOI poi)
        {
            var existing = NewPeople.First(t => t.ID == poi.ID);
            NewPeople.Remove(existing);
            poi.IsEdited = true;
            NewPeople.Add(poi);
        }

        public void RemovePerson(CasePOI poi)
        {
            if (this.IsNew)
            {
                NewExistingPeople.Remove(poi);
                NewPeople.Remove(poi);
            }
            else
            {
                poi.SoftDelete();
                DeletedPeople.Add(poi);
                if (ExistingPeople.Any(t => t.ID == poi.ID))
                    ExistingPeople.Remove(poi);
                if (NewExistingPeople.Any(t => t.ID == poi.ID))
                    NewExistingPeople.Remove(poi);
                if (NewPeople.Any(t => t.ID == poi.ID))
                    NewPeople.Remove(poi);
            }
        }

        /// <summary>
        /// Populates the Persons of Interest from the local database
        /// </summary>
        /// <returns>The POI.</returns>
        public void PopulatePOIs()
        {
            ExistingPeople = DataAccess.Instance.GetItems<CasePOI>(t => t.CaseID == this.ID && t.IsNew == false && t.HasChanged == false).OrderBy(t => t.CreatedDate).ToList();
            NewExistingPeople = DataAccess.Instance.GetItems<CasePOI>(t => t.CaseID == this.ID && t.IsNew == false && t.HasChanged == true).OrderBy(t => t.CreatedDate).ToList();
            NewPeople = DataAccess.Instance.GetItems<CasePOI>(t => t.CaseID == this.ID && t.IsNew == true).OrderBy(t => t.CreatedDate).ToList();

            var nullDate = new DateTime?();
            DeletedPeople = DataAccess.Instance.GetItems<CasePOI>(t => t.CaseID == this.ID && t.DeletedDate != nullDate).ToList();
        }

        /// <summary>
        /// Filters the cases by search term.
        /// </summary>
        /// <returns>Filtered cases</returns>
        /// <param name="searchTerm">Search term</param>
        public static List<Case> FilterCasesBySearchTerm(string searchTerm)
        {
            var cases = GetCasesAsync(Status.All, useLocal: true).Result.Data;
            searchTerm = searchTerm.ToLower();
            var issueCases = cases.Where(t => t.Issue.ToLower().Contains(searchTerm)).ToList();

            var personCases = new List<Case>();
            foreach(var c in cases)
            {
                foreach(var person in c.AllPeopleInvolved)
                {
                    if (person.POI.FirstName.Contains(searchTerm) ||
                        person.POI.LastName.Contains(searchTerm) ||
                        (!string.IsNullOrEmpty(person.POI.Email) && person.POI.Email.Contains(searchTerm)))
                    {
                        personCases.Add(c);
                    }
                }
            }

            var notes = cases.Where(t => t.Notes != null && t.Notes.ToLower().Contains(searchTerm)).ToList();

            var foundCases = new List<Case>();
            foundCases.AddRange(issueCases);
            foundCases.AddRange(personCases);
            foundCases.AddRange(notes);

            foundCases = foundCases.Distinct().ToList();

            return foundCases;
        }

        /// <summary>
        /// Gets cases by status
        /// </summary>
        /// <returns>Cases filtered by status</returns>
        /// <param name="status">Status to filter by</param>
        /// <param name="useLocal">Determines whether to use the local database or the API to get the cases. Defaults to using API.</param>
        public static async Task<APIResponse<List<Case>>> GetCasesAsync(Status status, bool useLocal = false)
        {
            // get from local data store if no internet connection or we are searching
            //   on only want to search locally
            if (!APIHelper.HasInternetConnection || useLocal)
            {
                var cases = (status != Status.All)
                    ? DataAccess.Instance.GetItems<Case>(t => t.Status == status)
                    : DataAccess.Instance.GetItems<Case>();

                foreach(var c in cases)
                {
                    c.PopulatePOIs();
                }

                return APIResponse.CreateWithData(cases);
            }

            var showClosed = status == Status.Closed || status == Status.All;

            var response = await APIHelper.MakeApiCall(APICalls.GetCases, null, showClosed);

            if (!response.Result)
            {
                return APIResponse.CreateAsFailure<List<Case>>(response.Message);
            }

            var data = APIHelper.DataAsList<Case>(response.Data);
            foreach(var d in data)
            {
                d.IsUploaded = true;
            }

            DataAccess.Instance.DeleteTable<Case>();
            DataAccess.Instance.DeleteTable<CasePOI>();

            var caseNotes = DataAccess.Instance.GetItems<Note>(t => t.CaseID != null);
            DataAccess.Instance.DeleteItems(caseNotes);

            if (data.Count > 0)
                DataAccess.Instance.SaveItems(data);

            foreach(var c in data)
            {
                DataAccess.Instance.SaveItems(c.ExistingPeople);

                if (c.Note != null)
                    DataAccess.Instance.SaveItem(c.Note);
            }

            return status == Status.All
                                   ? APIResponse.CreateWithData(data)
                                   : APIResponse.CreateWithData(data.Where(t => t.Status == status).ToList());
        }

        /// <summary>
        /// Adds case in API if network is available. If no internet, stores locally
        /// </summary>
        private async Task<APIResponse> Add()
        {
            var validate = Validate();
            if (!validate.IsValid)
            {
                return APIResponse.CreateAsFailure(validate.Message);
            }

            var now = DateTime.UtcNow;

            this.ID = Guid.NewGuid();
            this.OrganizationID = AppSettings.CurrentUser.OrganizationID;
            this.CreatedDate = now;
            this.CreatedByID = AppSettings.CurrentUser.UserID;
            this.CreatedByName = AppSettings.CurrentUser.ToString();
            this.LastUpdatedByID = this.CreatedByID;
            this.LastUpdatedDate = this.CreatedDate;
            this.LastUpdatedByName = this.CreatedByName;

            if (this.Status == Status.Closed)
            {
                this.ClosedDate = this.CreatedDate;
                this.ClosedByID = this.CreatedByID;
                this.ClosedByName = this.CreatedByName;
            }

            foreach(var poi in this.NewPeople)
            {
                poi.CaseID = this.ID;
            }

            foreach (var poi in this.NewExistingPeople)
            {
                poi.CaseID = this.ID;
            }

            if (!APIHelper.HasInternetConnection)
            {
                DataAccess.Instance.InsertItems(this.NewPeople);
                DataAccess.Instance.SaveItem(this);
                return APIResponse.CreateAsSuccess();
            }

            var originalExisting = this.ExistingPeople.ToList();
            this.ExistingPeople = this.NewExistingPeople;
            this.Note = null;

            var jsonData = new JsonContent(this);
            var response = await APIHelper.MakeApiCall(APICalls.AddCase, jsonData);

            if (!response.Result)
            {
                this.ExistingPeople = originalExisting;
            }

            return response;
        }

        /// <summary>
        /// Updates case in API if network is available. If no internet, stores locally
        /// </summary>
        private async Task<APIResponse> Update()
        {
            if (!_isSendingInBackground)
            {
                var validate = Validate();
                if (!validate.IsValid)
                {
                    return APIResponse.CreateAsFailure(validate.Message);
                }

                var now = DateTime.UtcNow;

                this.LastUpdatedDate = now;
                this.LastUpdatedByID = AppSettings.CurrentUser.UserID;
                this.LastUpdatedByName = AppSettings.CurrentUser.ToString();

                if (this.Status == Status.Closed)
                {
                    this.ClosedDate = this.LastUpdatedDate;
                    this.ClosedByID = this.LastUpdatedByID;
                    this.ClosedByName = this.LastUpdatedByName;
                }

                if (!APIHelper.HasInternetConnection)
                {
                    DataAccess.Instance.UpdateItems(this.NewPeople);
                    DataAccess.Instance.UpdateItems(this.NewExistingPeople);
                    DataAccess.Instance.SaveItem(this);
                    return APIResponse.CreateAsSuccess();
                }
            }

            var updatedPeople = this.ExistingPeople.Where(t => t.IsEdited).ToList();

            this.ExistingPeople = NewExistingPeople.Union(DeletedPeople).ToList();
            this.Note = null;

            foreach (var p in updatedPeople)
            {
                await APIHelper.MakeApiCall(APICalls.AddUser, new JsonContent(p.POI));
            }

            var jsonData = new JsonContent(this);
            var response = await APIHelper.MakeApiCall(APICalls.UpdateCase, jsonData);

            return response;
        }

        /// <summary>
        /// Validates input values
        /// </summary>
        private ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Issue))
                return ValidationResult.AsFailed(Constants.CaseView.IssueRequiredMessage);

            if (this.GrievanceStatus <= 0)
                return ValidationResult.AsFailed(Constants.CaseView.GrievanceStatusRequiredMessage);

            return ValidationResult.AsValid();
        }

        /// <summary>
        /// Saves case. Determines if the case is new or editing.
        /// </summary>
        public async Task<APIResponse> Save()
        {
            if (this.IsNew)
                return await Add();
            else
                return await Update();
        }

        private void Delete()
        {
            try
            {
                var pois = DataAccess.Instance.GetItems<CasePOI>(t => t.CaseID == this.ID);
                DataAccess.Instance.DeleteItems(pois);
                DataAccess.Instance.DeleteItem(this);
            }
            catch{}
        }

        private static bool _isSendingInBackground = false;

        /// <summary>
        /// Uploads cases that were stored locally because of no internet
        /// </summary>
        /// <returns>The cases.</returns>
        public static async Task UploadCasesAsync()
        {
            if (!APIHelper.HasInternetConnection || _isSendingInBackground)
                return;

            var responses = new List<APIResponse>();
            var cases = DataAccess.Instance.GetItems<Case>(t => t.IsUploaded == false);

            Debug.WriteLine($"Uploading {cases.Count} cases");
            if (cases.Count == 0)
                return;

            _isSendingInBackground = true;

            foreach(var newCase in cases)
            {
                newCase.PopulatePOIs();
                var response = await newCase.Save();
                responses.Add(response);

                if (!response.Result)
                {
                    continue;
                }

                newCase.Delete();
            }

            _isSendingInBackground = false;
        }

        #region IEquatable implementation

        public bool Equals(Case other)
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
            if (obj == null || !(obj is Case))
                return false;

            return Equals(obj as Case);
        }

        #endregion
    }
}