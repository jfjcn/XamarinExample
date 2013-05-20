using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace SpecsForWebApiExamples
{
    public class RestClientForTesting 
    {
        protected readonly bool _requiresAuth;
        protected readonly string _baseUrl;

        public string AuthCookieName { get; protected set; }
        public string AuthCookieValue { get; protected set; }

        public RestClientForTesting(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public virtual void AuthenticateThisClient(string username, string password,
            string relativePathForAuth)
        {
            var requestResource = string.Format(relativePathForAuth,
                              username, password);
            var clientAndRequest = GetRestClientAndRestRequest(_baseUrl, requestResource, Method.GET, null);
            var client = clientAndRequest.Item1;
            var request = clientAndRequest.Item2;
            var response = client.Execute(request);
            var authCookie = response.Cookies[0];
            AuthCookieName = authCookie.Name;
            AuthCookieValue = authCookie.Value;
        }

        public RestClientResponse<List<T>> GetMany<T>(string resourceRelativePath) 
        {
            var clientAndRequest = GetRestClientAndRestRequest(_baseUrl, resourceRelativePath, Method.GET, null);
            var client = clientAndRequest.Item1;
            var request = clientAndRequest.Item2;
            var response = client.Execute(request);
            var responseContent = response.Content;
            List<T> responseAsObject = null;
            if (!IsJsonpCall(request))
            {
                var trimmedResponseContent = TrimStartAndEndOfJson(responseContent);
                responseAsObject = JsonConvert.DeserializeObject<List<T>>(trimmedResponseContent);
            }
            var getResult =
                new RestClientResponse<List<T>>
                {
                    ReturnedObject = responseAsObject,
                    HttpStatusCode = (int)response.StatusCode,
                    HttpReasonPhrase = response.StatusDescription
                };
            return getResult;
        }

        public RestClientResponse<T> GetSingle<T>(string relativePath) where T : class, new()
        {
            return GetSingle<T>(relativePath, null);
        }


        public RestClientResponse<T> GetSingle<T>(string relativePath, Dictionary<string, string> parameters) where T : class, new()
        {
            var clientAndRequest = GetRestClientAndRestRequest(_baseUrl, relativePath, Method.GET, parameters);
            var client = clientAndRequest.Item1;
            var request = clientAndRequest.Item2;
            var response = client.Execute(request);
            var responseContent = response.Content;
            T responseAsObject = null;
            if (!IsJsonpCall(request))
            {
                var trimmedResponseContent = TrimStartAndEndOfJson(responseContent);
                responseAsObject = JsonConvert.DeserializeObject<T>(trimmedResponseContent);
            }
            var getResult =
                new RestClientResponse<T>()
                {
                    ReturnedObject = responseAsObject,
                    HttpStatusCode = (int)response.StatusCode,
                    HttpReasonPhrase = response.StatusDescription
                };
            return getResult;
        }

        public RestClientResponse Post<T>(string resourceRelativePath, T resourceToCreate)
        {
            var clientAndRequest = GetRestClientAndRestRequest(_baseUrl, resourceRelativePath, Method.POST, null);
            var client = clientAndRequest.Item1;
            var request = clientAndRequest.Item2;
            request.RequestFormat = DataFormat.Json;
            request.AddObject(resourceToCreate);
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var location = GetLocationHeaderFrom(response);
                return
                    new RestClientResponse()
                        {
                            Success = true,
                            ResourceUri = new Uri(location),
                            HttpStatusCode = (int) response.StatusCode,
                            HttpReasonPhrase = response.StatusDescription
                        };
            }
            return
                new RestClientResponse
                    {
                        Success = false,
                        ResourceUri = null,
                        HttpReasonPhrase = response.Content,
                        HttpStatusCode = (int) response.StatusCode
                    };
        }

        public RestClientResponse Put<T>(string resourceRelativePath, T resouceToUpdate)
        {
            var clientAndRequest = GetRestClientAndRestRequest(_baseUrl, resourceRelativePath, Method.PUT, null);
            var client = clientAndRequest.Item1;
            var request = clientAndRequest.Item2;
            request.RequestFormat = DataFormat.Json;
            request.AddObject(resouceToUpdate);
            var response = client.Execute(request);
            //TODO: figure out if this is the right REST stuff and eliminate copy/paste code
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var location = GetLocationHeaderFrom(response);
                return
                    new RestClientResponse
                    {
                        Success = true,
                        ResourceUri = new Uri(location),
                        HttpStatusCode = (int)response.StatusCode
                    };
            }
            return
                new RestClientResponse
                {
                    Success = false,
                    ResourceUri = null,
                    HttpStatusCode = (int)response.StatusCode
                };
        }


        public RestClientResponse Delete(string resourceRelativePath, int id)
        {
            var clientAndRequest = GetRestClientAndRestRequest(_baseUrl, resourceRelativePath, Method.DELETE, null);
            var client = clientAndRequest.Item1;
            var request = clientAndRequest.Item2;
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute(request);
            //TODO: figure out if this is the right REST stuff and eliminate copy/paste code
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return
                    new RestClientResponse
                    {
                        Success = true,
                        ResourceUri = null,
                        HttpStatusCode = (int)response.StatusCode
                    };
            }
            return
                new RestClientResponse
                {
                    Success = false,
                    ResourceUri = null,
                    HttpStatusCode = (int)response.StatusCode
                };
        }

        protected Tuple<RestClient, RestRequest> GetRestClientAndRestRequest(
            string baseUrl, string relativePath, Method httpMethod, Dictionary<string, string> parameters)
        {
            var client = new RestClient();
            client.BaseUrl = baseUrl;
            var request = new RestRequest(httpMethod);
            request.AddHeader("Accept", @"text/html, application/xhtml+xml, */*");
            if (parameters != null &&
                parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    request.AddParameter(parameter.Key, parameter.Value);
                }
            }
            if (_requiresAuth &&
                (string.IsNullOrEmpty(AuthCookieName) ||
                string.IsNullOrEmpty(AuthCookieValue)))
            {
                request.AddCookie(AuthCookieName, AuthCookieValue);
            }
            request.Resource = relativePath;
            return new Tuple<RestClient, RestRequest>(client, request);
        }

        internal static bool IsJsonpCall(RestRequest requestToTest)
        {
            if (requestToTest.Parameters.Contains(
                new Parameter { Name = "callback" },
                new ContainsParameterNameComparer()))
            {
                return true;
            }
            return false;
        }

        private static string TrimStartAndEndOfJson(string responseContent)
        {
            if (!string.IsNullOrEmpty(responseContent))
            {
                while (responseContent[0] != '{' && responseContent[0] != '[')
                {
                    responseContent = responseContent.Remove(0, 1);
                }
                while (responseContent[responseContent.Length - 1] != '}' && responseContent[responseContent.Length - 1] != ']')
                {
                    responseContent = responseContent.Remove(responseContent.Length - 1, 1);
                }
            }
            return responseContent;
        }

        protected static string GetLocationHeaderFrom(IRestResponse response)
        {
            var locationHeader = response.Headers.Where(
                x => x.Name.ToLowerInvariant() == "location").ToList();
            if (locationHeader.Count < 1)
            {
                return null;
            }
            return locationHeader[0].Value.ToString();
        }

        private class ContainsParameterNameComparer : IEqualityComparer<Parameter>
        {

            public bool Equals(Parameter x, Parameter y)
            {
                return x.Name.ToUpperInvariant().Equals(y.Name.ToUpperInvariant());
            }

            public int GetHashCode(Parameter obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }

    public class RestClientResponse
    {
        public bool Success { get; set; }
        public Uri ResourceUri { get; set; }
        public string ResourceParsedId { get; set; }
        public int HttpStatusCode { get; set; }
        public string HttpReasonPhrase { get; set; }
    }

    public class RestClientResponse<T> : RestClientResponse
    {
        public T ReturnedObject { get; set; }
    }

    class zzzzBasicRestClient
    {
        

        

//        public string DoGet(string baseUrl, string relativePath)
//        {
//            var clientAndRequest = GetRestClientAndRestRequest(baseUrl, relativePath, Method.GET, null);
//            var client = clientAndRequest.Item1;
//            var request = clientAndRequest.Item2;
//            var response = client.Execute(request);
//            var responseContent = response.Content;
//            return responseContent;
//        }

  
        

        
    }
}
