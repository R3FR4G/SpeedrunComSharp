using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Dynamic;
using System.IO;
using System.Net;

namespace SpeedrunComSharp
{
    internal static class JSON
    {
        public static ExpandoObject FromResponse(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                return FromStream(stream);
            }
        }

        public static ExpandoObject FromStream(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string json = "";

            try
            {
                json = reader.ReadToEnd();
            }
            catch
            {
            }

            return FromString(json);
        }

        public static ExpandoObject FromString(string value)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(value, new ExpandoObjectConverter());
        }

        public static ExpandoObject FromUri(Uri uri, string userAgent, string accessToken, TimeSpan timeout)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = (int)timeout.TotalMilliseconds;
            request.UserAgent = userAgent;

            if (!string.IsNullOrEmpty(accessToken))
            { 
                request.Headers.Add("X-API-Key", accessToken.ToString());
            }

            var response = request.GetResponse();

            return FromResponse(response);
        }

        public static ExpandoObject FromUriPut(Uri uri, string userAgent, string accessToken, TimeSpan timeout, string putBody)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.Timeout = (int)timeout.TotalMilliseconds;
            request.Method = "PUT";
            request.UserAgent = userAgent;

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Add("X-API-Key", accessToken.ToString());
            }

            request.ContentType = "application/json";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(putBody);
            }

            var response = request.GetResponse();

            return FromResponse(response);
        }

        public static ExpandoObject FromUriPost(Uri uri, string userAgent, string accessToken, TimeSpan timeout, string postBody)
        {
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            request.Timeout = (int)timeout.TotalMilliseconds;
            request.Method = "POST";
            request.UserAgent = userAgent;

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Add("X-API-Key", accessToken.ToString());
            }

            request.ContentType = "application/json";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postBody);
            }

            var response = request.GetResponse();

            return FromResponse(response);
        }
    }
}
