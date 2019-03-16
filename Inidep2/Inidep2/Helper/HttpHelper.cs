using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Inidep2.Helper
{
    public static class HttpHelper
    {
        public static readonly HttpClient HttpClient = new HttpClient { BaseAddress = new Uri(ConfigUtil.GetString("api_Url")) };
        
        public static T WaitAndGetValue<T>(this Task<T> task)
        {
            try
            {
                task.Wait();
                return task.Result;
            }
            catch (Exception e)
            {
                throw e.InnerException ?? e;
            }
        }

         public static HttpResponseMessage CreateRequest(string uri, HttpMethod method, object obj, string apiKey = null)
        {
            apiKey = apiKey ?? ConfigUtil.GetString("api_Key");
            using (var request = new HttpRequestMessage(method, uri))
            {
                request.Headers.Add("Authorization", "token " + apiKey);
                if (obj != null)
                    request.Content = new StringContent(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json");

                var response = HttpClient.SendAsync(request).WaitAndGetValue();
                Thread.Sleep(50);
                return response;
            }
        }

         public static HttpResponseMessage CreateRequestByteContent(string uri, HttpMethod method, byte[] content = null, string contentType = null, string apiKey = null)
         {
             apiKey = apiKey ?? ConfigUtil.GetString("api_Key");
             using (var request = new HttpRequestMessage(method, uri))
             {
                 request.Headers.Add("Authorization", "token " + apiKey);
                 if (content != null)
                 {
                     request.Content = new ByteArrayContent(content);
                     request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                 }
                 var response = HttpClient.SendAsync(request).WaitAndGetValue();
                 Thread.Sleep(50);
                 return response;
             }
         }
    }
}
