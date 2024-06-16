using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DelegatePro.PCL
{
    public class GrievanceStatus : DataItemInt
    {
        [JsonProperty("value")]
        public override int ID { get; set; }
        [JsonProperty("display")]
        public string Name { get; set; }

        public static async Task<APIResponse<List<GrievanceStatus>>> GetStatusesAsync()
        {
            if (!APIHelper.HasInternetConnection)
            {
                var data = DataAccess.Instance.GetItemsInt<GrievanceStatus>();
                return APIResponse.CreateWithData(data);
            }

            var response = await APIHelper.MakeApiCall(APICalls.GetGrievanceStatuses);
            if (!response.Result)
            {
                return APIResponse.CreateAsFailure<List<GrievanceStatus>>(response.Message);
            }

            var statuses = APIHelper.DataAsList<GrievanceStatus>(response.Data);
            DataAccess.Instance.SaveItemsInt(statuses);

            return APIResponse.CreateWithData(statuses);
        }

        public static List<GrievanceStatus> GetStatusesLocal()
        {
            return DataAccess.Instance.GetItemsInt<GrievanceStatus>();
        }
    }
}