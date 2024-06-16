using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace DelegatePro.PCL
{
    public class JsonContent : StringContent
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        private const string JsonMediaType = "application/json";

        public JsonContent(object content)
            : this(JsonConvert.SerializeObject(content, JsonContent.Settings))
        {
            
        }

        public JsonContent(string jsonData)
            : base(jsonData, Encoding.UTF8, JsonMediaType)
        {
            
        }
    }
}

