using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Torrefactor.Tests.Integration.Clients
{
    public static class ApiResponseExtensions
    {
        public static Task<ApiResponse> ToApiResponse(this Task<HttpResponseMessage> httpResponseTask)
        {
            return ApiResponse.CreateFrom(httpResponseTask);
        }
    }
    
    public class ApiResponse
    {
        private ApiResponse(HttpStatusCode statusCode, string content, Uri requestUri)
        {
            StatusCode = statusCode;
            Content = content;
            RequestUri = requestUri;
        }

        public HttpStatusCode StatusCode { get; }
        
        public string Content { get; }
        public Uri RequestUri { get; }

        public JObject Json
        {
            get
            {
                EnsureSuccessStatusCode();
                return JObject.Parse(Content);
            }
        }

        public T Model<T>()
        {
            EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(Content);
        }

        private void EnsureSuccessStatusCode()
        {
            if (StatusCode == HttpStatusCode.OK || StatusCode == HttpStatusCode.NoContent)
                return;
            
            throw new InvalidOperationException($"Request {RequestUri} has failed. Status code is: {StatusCode}. Content is: `{Content}`");
        }

        public static async Task<ApiResponse> CreateFrom(Task<HttpResponseMessage> httpResponseTask)
        {
            var httpResponse = await httpResponseTask;
            var content = await httpResponse.Content.ReadAsStringAsync();
            return new ApiResponse(httpResponse.StatusCode, content, httpResponse.RequestMessage.RequestUri);
        }
    }
}