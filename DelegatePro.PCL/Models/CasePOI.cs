using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace DelegatePro.PCL
{
    public class CasePOI : DataItem, IEquatable<CasePOI>
    {
        [JsonProperty("GUID")]
        public override Guid ID { get; set; }

        [JsonProperty("GUID_Case")]
        public Guid CaseID { get; set; }

        [JsonProperty("GUID_POI")]
        public Guid PoiID { get; set; }

        private POI _poi;
        [Ignore, JsonIgnore]
        public POI POI
        {
            get 
            {
                return _poi ?? (_poi = (PoiID != Guid.Empty) ? DataAccess.Instance.GetItem<POI>(PoiID) : new POI()); 
            }
        }

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

        [JsonIgnore]
        public bool IsNew { get; set; }

        [JsonIgnore]
        public bool IsEdited { get; set; }

        [JsonIgnore]
        public bool HasChanged { get; set; }

        [Ignore, JsonIgnore]
        public bool IsDeleted
        {
            get { return DeletedDate.HasValue; }
        }

        public static CasePOI Create()
        {
            return new CasePOI
            {
                IsNew = true,
                ID = Guid.NewGuid()
            };
        }

        public static CasePOI Create(Guid poiID)
        {
            return new CasePOI
            {
                IsNew = true,
                ID = Guid.NewGuid(),
                PoiID = poiID
            };
        }

        public static CasePOI Create(Guid poiID, Guid caseID)
        {
            return new CasePOI
            {
                ID = Guid.NewGuid(),
                PoiID = poiID,
                CaseID = caseID,
                IsNew = false,
                HasChanged = true,
                CreatedByID = AppSettings.CurrentUser.UserID,
                CreatedDate = DateTime.UtcNow,
                CreatedByName = AppSettings.CurrentUser.ToString()
            };
        }

        public override string ToString()
        {
            return $"{POI.FirstName} {POI.LastName}";
        }

        public Result Save()
        {
            try
            {
                if (this.CreatedByID == Guid.Empty)
                {
                    this.CreatedDate = DateTime.UtcNow;
                    this.CreatedByID = AppSettings.CurrentUser.UserID;
                    this.CreatedByName = AppSettings.CurrentUser.ToString();
                }

                DataAccess.Instance.SaveItem(this);
            }
            catch (Exception ex)
            {
                return Result.CreateAsFailure(ex.Message);
            }

            return Result.CreateAsSuccess();
        }

        public Result SoftDelete()
        {
            try
            {
                this.DeletedDate = DateTime.UtcNow;
                this.DeletedByID = AppSettings.CurrentUser.UserID;
                this.DeletedByName = AppSettings.CurrentUser.ToString();

                DataAccess.Instance.SaveItem(this);
            }
            catch (Exception ex)
            {
                return Result.CreateAsFailure(ex.Message);
            }

            return Result.CreateAsSuccess();
        }

        public Result HardDelete()
        {
            try
            {
                DataAccess.Instance.DeleteItem(this);
            }
            catch (Exception ex)
            {
                return Result.CreateAsFailure(ex.Message);
            }

            return Result.CreateAsSuccess();
        }

        #region IEquatable implementation

        public bool Equals(CasePOI other)
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
            if (obj == null || !(obj is CasePOI))
                return false;

            return Equals(obj as CasePOI);
        }

        #endregion
    }
}