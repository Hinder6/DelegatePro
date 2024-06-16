using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace DelegatePro.PCL
{
    public class Note : DataItem, IEquatable<Note>
    {
        public Note()
        {
            IsUploaded = false;
        }

        [JsonProperty("GUID")]
        public override Guid ID { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsAddressed { get; set; }
        public DateTime? AddressedDate { get; set; }
        public Guid? CaseID { get; set; }
        public NoteVisibility Visibility { get; set; } = NoteVisibility.Me;
        public Guid? ParentNoteID { get; set; }

        [JsonProperty("GUID_CreatedBy")]
        public Guid CreatedByID { get; set; }
        [JsonProperty("DT_Created")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("GUID_DeletedBy")]
        public Guid? DeletedByID { get; set; }
        [JsonProperty("DT_Deleted")]
        public DateTime? DeletedDate { get; set; }
        [JsonProperty("DT_LastUpdated")]
        public DateTime LastUpdatedDate { get; set; }
        [JsonProperty("GUID_LastUpdatedBy")]
        public Guid LastUpdatedByID { get; set; }

        [Ignore]
        public Case Case { get; set; }

        [JsonIgnore]
        public bool IsUploaded { get; set; }
        [JsonIgnore, Ignore]
        public bool IsNew 
        {
            get { return this.ID == Guid.Empty; }
        }

        [Ignore, JsonIgnore]
        public static List<NoteVisibility> NoteVisibilities
        {
            get
            {
                var types = new List<NoteVisibility>();
                foreach (NoteVisibility enumValue in Enum.GetValues(typeof(NoteVisibility)))
                {
                    types.Add(enumValue);
                }

                return types;
            }
        }

        #region Display Properties

        private Case _attachedCase;
        [JsonIgnore, Ignore]
        public string CaseDisplay
        {
            get 
            {
                if (Case != null)
                    _attachedCase = Case;
                
                if (!CaseID.HasValue)
                    return Constants.Note.NotAttachedToCaseMessage;
                
                _attachedCase = _attachedCase ?? (_attachedCase = DataAccess.Instance.GetItem<Case>(this.CaseID.Value));

                if (_attachedCase == null)
                    return Constants.Note.CaseNotFound;

                if (_attachedCase.Issue.Length <= 20)
                    return $"{_attachedCase.CaseID} - {_attachedCase.Issue}";

                return $"{_attachedCase.CaseID} - {_attachedCase.Issue.Substring(0, 20)}...";
            }
        }

        private Note _parentNote;
        [JsonIgnore, Ignore]
        public string ParentNoteDisplay
        {
            get 
            {
                if (!ParentNoteID.HasValue)
                    return Constants.Note.NoParentNote;

                _parentNote = _parentNote ?? (_parentNote = DataAccess.Instance.GetItem<Note>(ParentNoteID.Value));

                if (_parentNote.Title.Length <= 20)
                    return _parentNote.Title;
                
                return _parentNote.Title.Substring(0, 20) + "...";
            }
        }

        [JsonIgnore, Ignore]
        public string VisibilityDisplay => Visibility.ToString("G");

        [JsonIgnore, Ignore]
        public RGBWrapper VisibilityDisplayColor => Constants.DefaultTextColor;

        [JsonIgnore, Ignore]
        public RGBWrapper CaseDisplayColor => CaseID.HasValue ? Constants.DefaultTextColor : Constants.BlueSelectionTextColor;

        [JsonIgnore, Ignore]
        public RGBWrapper ParentNoteDisplayColor => ParentNoteID.HasValue ? Constants.DefaultTextColor : Constants.BlueSelectionTextColor;

        #endregion

        public void SetAddressed(bool addressed)
        {
            this.IsAddressed = addressed;
            this.AddressedDate = (addressed) ? DateTime.UtcNow : (DateTime?)null;
        }

        public static List<Note> GetNotes()
        {
            return DataAccess.Instance.GetItems<Note>();
        }

        public static async Task<APIResponse<List<Note>>> GetNotesAsync()
        {
            if (!APIHelper.HasInternetConnection)
            {
                var dbData = DataAccess.Instance.GetItems<Note>(t => t.DeletedDate == null);
                if (dbData.Count == 0)
                    return APIResponse.CreateAsFailure<List<Note>>(Constants.NoInternetMessage);

                return APIResponse.CreateWithData(dbData);
            }

            var response = await APIHelper.MakeApiCall(APICalls.GetNotes);

            if (!response.Result)
            {
                return APIResponse.CreateAsFailure<List<Note>>(response.Message);
            }

            var data = APIHelper.DataAsList<Note>(response.Data);
            foreach (var d in data)
            {
                d.IsUploaded = true;
            }

            DataAccess.Instance.DeleteTable<Note>();

            if (data.Count > 0)
                DataAccess.Instance.SaveItems(data);

            return APIResponse.CreateWithData(data);
        }

        public static async Task<APIResponse<Note>> GetNote(Guid noteID)
        {
            if (!APIHelper.HasInternetConnection)
            {
                var dbData = DataAccess.Instance.GetItem<Note>(noteID);
                if (dbData == null)
                    return APIResponse.CreateAsFailure<Note>(Constants.NoInternetMessage);

                return APIResponse.CreateWithData(dbData);
            }

            var response = await APIHelper.MakeApiCall(APICalls.GetNote, null, noteID);
            if (!response.Result)
            {
                return APIResponse.CreateAsFailure<Note>(response.Message);
            }

            var note = APIHelper.DataAsObject<Note>(response.Data);
            DataAccess.Instance.SaveItem(note);

            return APIResponse.CreateWithData(note);
        }

        public async Task<APIResponse> AddAsync()
        {
            var validationResult = Validate();
            if (!validationResult.IsValid)
                return APIResponse.CreateAsFailure(validationResult.Message);

            var now = DateTime.UtcNow;

            this.ID = Guid.NewGuid();
            this.CreatedDate = now;
            this.CreatedByID = AppSettings.CurrentUser.UserID;
            this.LastUpdatedByID = this.CreatedByID;
            this.LastUpdatedDate = this.CreatedDate;

            if (!APIHelper.HasInternetConnection)
            {
                IsUploaded = false;
                DataAccess.Instance.SaveItem(this);
                return APIResponse.CreateAsSuccess();
            }

            var jsonData = new JsonContent(this);
            var response = await APIHelper.MakeApiCall(APICalls.AddNote, jsonData);

            return response;
        }

        public async Task<APIResponse> DeleteAsync()
        {
            var now = DateTime.UtcNow;

            this.DeletedByID = AppSettings.CurrentUser.UserID;
            this.DeletedDate = now;

            if (!APIHelper.HasInternetConnection)
            {
                IsUploaded = false;
                DataAccess.Instance.SaveItem(this);
                return APIResponse.CreateAsSuccess();
            }

            var response = await APIHelper.MakeApiCall(APICalls.DeleteNote, null, this.ID);
            return response;
        }

        public async Task<APIResponse> UpdateAsync()
        {
            var validationResult = Validate();
            if (!validationResult.IsValid)
                return APIResponse.CreateAsFailure(validationResult.Message);

            var now = DateTime.UtcNow;

            this.LastUpdatedByID = AppSettings.CurrentUser.UserID;
            this.LastUpdatedDate = this.CreatedDate;

            if (!APIHelper.HasInternetConnection)
            {
                IsUploaded = false;
                DataAccess.Instance.SaveItem(this);
                return APIResponse.CreateAsSuccess();
            }

            var jsonData = new JsonContent(this);
            var response = await APIHelper.MakeApiCall(APICalls.UpdateNote, jsonData);

            return response;
        }

        public async Task<APIResponse> SaveAsync()
        {
            if (this.IsNew)
                return await AddAsync();
            else
                return await UpdateAsync();
        }

        private void Delete()
        {
            try
            {
                DataAccess.Instance.DeleteItem(this);
            }
            catch { }
        }

        private ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
                return ValidationResult.AsFailed(Constants.Note.TitleRequiredMessage);

            if (string.IsNullOrWhiteSpace(Text))
                return ValidationResult.AsFailed(Constants.Note.NoteTextRequiredMessage);

            return ValidationResult.AsValid();
        }

        private static bool _isSendingInBackground = false;

        public static async Task UploadNotesAsync()
        {
            if (!APIHelper.HasInternetConnection || _isSendingInBackground)
                return;

            var responses = new List<APIResponse>();
            var notes = DataAccess.Instance.GetItems<Note>(t => t.IsUploaded == false);

            Debug.WriteLine($"Uploading {notes.Count} notes");
            if (notes.Count == 0)
                return;

            _isSendingInBackground = true;

            foreach(var note in notes)
            {
                APIResponse response = null;

                if (note.DeletedDate == null)
                    response = await note.SaveAsync();
                else
                    response = await note.DeleteAsync();

                responses.Add(response);

                if (!response.Result)
                    continue;

                note.Delete();
            }

            _isSendingInBackground = false;
        }

        public override string ToString()
        {
            return $"{CreatedDate.ToString("MM/dd/yy")} - {Title}";
        }

        #region IEquatable implementation

        public bool Equals(Note other)
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
            if (obj == null || !(obj is Note))
                return false;

            return Equals(obj as Note);
        }

        #endregion
    }
}