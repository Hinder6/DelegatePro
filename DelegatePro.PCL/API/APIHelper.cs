using System;
using System.Linq;
using Plugin.Connectivity;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DelegatePro.PCL
{
    public static class APIHelper
    {
        private static HttpClient _client;
        private static HttpClientHandler _handler;

        internal static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    CreateHttpClient();
                }
                return _client;
            }
        }

        public static bool HasInternetConnection
        {
            get { return CrossConnectivity.Current.IsConnected; }
        }

        public static void CreateHttpClient()
        {
            var url = AppSettings.WebServiceURL;
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler);
            _client.BaseAddress = new Uri(url);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        internal static void ResetHttpClient()
        {
            _handler = null;
            _client = null;
        }

        private static void UpdateAuthHeader()
        {
            _client.DefaultRequestHeaders.Add("Token", AppSettings.CurrentUser.Token.ToString());
        }

        public static async Task<APIResponse<string>> MakeApiCall(APICall apiCall, HttpContent postBody = null, params object[] args)
        {
            try
            {
                if (!HasInternetConnection)
                {
                    return APIResponse.CreateAsFailure<string>(Constants.NoInternetMessage);
                }

                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (apiCall.IncludeAuthHeader)
                {
                    UpdateAuthHeader();
                }

                var url = APICalls.StandardServicePrefix + string.Format(apiCall.URLFragment, args);
                using (var request = new HttpRequestMessage(apiCall.Method, url))
                {
                    request.Content = postBody;

                    using (var response = await Client.SendAsync(request))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            return APIResponse.CreateAsFailure<string>("API call failed. Please try again.");
                        }

                        var content = string.Empty;
                        if (response.Content != null)
                        {
                            content = await response.Content.ReadAsStringAsync();
                        }

                        using (var apiResponse = JsonConvert.DeserializeObject<APIResponse>(content))
                        {
                            if (apiResponse == null)
                            {
                                return APIResponse.CreateAsFailure<string>("API call failed. Please try again.");
                            }

                            if (!apiResponse.Result)
                            {
                                return APIResponse.CreateAsFailure<string>(apiResponse.Message);
                            }

                            return new APIResponse<string>
                            {
                                Result = true,
                                Data = content
                            };
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return APIResponse.CreateAsFailure<string>(ex.Message);
            }
        }

        public static T DataAsObject<T>(string content)
        {
            var json = JObject.Parse(content);
            var jObject = json["Data"];
            if (jObject == null)
                return default(T);

            return jObject.ToObject<T>();
        }

        public static List<T> DataAsList<T>(string content)
        {
            var json = JObject.Parse(content);
            var jObject = json["Data"];
            if (jObject == null)
                return default(List<T>);

            return jObject.ToObject<List<T>>();
        }
    }
}




//private static HttpWebRequest CreateWebRequest(APICall apiCall)
//{
//    var url = $"{AppSettings.WebServiceURL}{APICalls.StandardServicePrefix}{apiCall.Action}";
//    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
//    webRequest.Headers[@"SOAP"] = "Action";
//    webRequest.ContentType = "text/xml;charset=\"utf-8\"";
//    webRequest.Accept = "text/xml";
//    webRequest.Method = "POST";
//    return webRequest;
//}

//private static XDocument CreateSoapEnvelope(APICall apiCall, params object[] args)
//{
//    return XDocument.Parse(string.Format(apiCall.SoapXml, args));
//}

//private static async Task InsertSoapEnvelopeIntoWebRequest(XDocument soapEnvelopeXml, HttpWebRequest webRequest)
//{
//    using (var stream = await webRequest.GetRequestStreamAsync())
//    {
//        soapEnvelopeXml.Save(stream);
//    }
//}




//var soapEnvelopeXml = CreateSoapEnvelope(apiCall, args);
//HttpWebRequest webRequest = CreateWebRequest(apiCall);
//await InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

//string jsonData = string.Empty;
//using (var webResponse = await webRequest.GetResponseAsync())
//{
//    string soapResult;
//    using (var rd = new StreamReader(webResponse.GetResponseStream()))
//    {
//        soapResult = rd.ReadToEnd();
//    }

//    var responseData = XDocument.Parse(soapResult);

//    try
//    {
//        jsonData = responseData.Descendants().ToList()[3].Value;
//    }
//    catch{}
//}

//if (string.IsNullOrEmpty(jsonData))
//{
//    return APIResponse.CreateAsFailure<string>("API error");
//}

//return APIResponse.CreateWithData(jsonData);