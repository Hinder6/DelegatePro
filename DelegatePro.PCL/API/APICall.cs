using System;
using System.Net.Http;

namespace DelegatePro.PCL
{
    public class APICall
    {
        //public string SoapXml { get; set; }
        //public string Action { get; set; }
        public string URLFragment { get; set; }
        public bool IncludeAuthHeader { get; set; } = true;
        public HttpMethod Method { get; set; } = HttpMethod.Get;
    }

    public static class APICalls
    {
        public const string StandardServicePrefix = "api/mobile/";

        public static APICall Login = new APICall { URLFragment = "v1/login", IncludeAuthHeader = false, Method = HttpMethod.Post };
        public static APICall GetCases = new APICall { URLFragment = "v1/cases?showClosed={0}" };
        public static APICall AddCase = new APICall { URLFragment = "v1/cases", Method = HttpMethod.Post };
        public static APICall UpdateCase = new APICall { URLFragment = "v1/cases", Method = HttpMethod.Post };
        public static APICall GetGrievanceStatuses = new APICall { URLFragment = "v1/grievencestatuses" };
        public static APICall GetUsers = new APICall { URLFragment = "v1/users" };
        public static APICall AddUser = new APICall { URLFragment = "v1/users", Method = HttpMethod.Post };
        public static APICall UpdateUser = new APICall { URLFragment = "v1/users", Method = HttpMethod.Post };
        public static APICall GetNotes = new APICall { URLFragment = "v1/notes", Method = HttpMethod.Get };
        public static APICall GetNote = new APICall { URLFragment = "v1/note/{0}", Method = HttpMethod.Get };
        public static APICall AddNote = new APICall { URLFragment = "v1/notes", Method = HttpMethod.Post };
        public static APICall UpdateNote = new APICall { URLFragment = "v1/notes", Method = HttpMethod.Post };
        public static APICall DeleteNote = new APICall { URLFragment = "v1/note/{0}", Method = HttpMethod.Delete };

        //public static APICall Login = new APICall { Action = "Authenticate", SoapXml = @"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance\"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><Authenticate xmlns=""http://tempuri.org/""><user>{0}</user><password>{1}</password></Authenticate></soap:Body></soap:Envelope>" };
        //public static APICall GetCases = new APICall {  };
        //public static APICall AddCase = new APICall {  };
        //public static APICall UpdateCase = new APICall {  };
    }
}

