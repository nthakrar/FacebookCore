using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rest.Net;
using Rest.Net.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VideoSmart.Core.Api;
using static FacebookCore.FacebookCursor;
namespace FacebookCore
{
    /// <summary>
    /// FacebookClient is in-charge of the making the actual API calls to the Facebook http API
    /// </summary>
    public class FacebookClient
    {

        internal string ClientId { get; }

        internal string ClientSecret { get; }

        internal IRestClient RestClient { get; }
        internal IHttpClientService HttpClientService{ get; }

        public string GraphApiVersion { get; set; } = "v8.0";

        /// <summary>
        /// Application API
        /// </summary>
       
        public FacebookClient(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RestClient = new RestClient("https://graph.facebook.com/");
            HttpClientService = new HttpClientService();
        }

        public FacebookClient(FacebookConfig facebookConfig)
            : this(facebookConfig.ClientId, facebookConfig.ClientSecret)
        {
            GraphApiVersion = facebookConfig.GraphApiVersion;
        }

    
        /// <summary>
        /// Makes a request to the Facebook http API
        /// </summary>
        /// <param name="path">API endpoint</param>
        /// <param name="accessToken">Authentication token</param>
        /// <param name="cursor">Cursor of current available data (if exists)</param>
        /// <param name="cursorDirection">What set of results should be taken (after or before the current cursor)</param>
        /// <returns></returns>
        public async Task<JObject> GetAsync(string path, string accessToken = null, FacebookCursor cursor = null, Direction cursorDirection = Direction.None)
        {
            path = StandardizePath(path);
            path = AddAccessTokenToPathIfNeeded(path, accessToken);
            path = AddCursorToPathIfNeeded(path, cursor, cursorDirection);

            //var test = HttpClientService.GetAsync<object>($"https://graph.facebook.com/{GraphApiVersion}{path}");
            return SerializeResponse(await RestClient.GetAsync($"/{GraphApiVersion}{path}", false));
        }

        public async Task PostAsync(string path, string accessToken = null)
        {
            try
            {
                path = StandardizePath(path);
                //path = AddAccessTokenToPathIfNeeded(path, accessToken);
                //var content = new FormUrlEncodedContent(new Dictionary<string, string>
                //{
                //    {"name", "My campaign #1"},
                //    {"objective", "LINK_CLICKS"},
                //    {"status", "PAUSED"},
                //    {"special_ad_categories", "NONE"},
                //    {"access_token", accessToken}
                //});
                var jsonContent = JsonConvert.SerializeObject( new
                {
                    name = "My campaign #1",
                    objective = "LINK_CLICKS",
                    status = "PAUSED",
                    special_ad_categories = new object[]{"NONE"},
                    access_token = "accessToken"
                });
                var content = new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json");
                var result = await RestClient.PostAsync($"/{GraphApiVersion}{path}", content, false);
            }
            catch (Exception e)
            {

            }
           
        }


        private string StandardizePath(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            return path;
        }

        private string AddAccessTokenToPathIfNeeded(string path, string accessToken)
        {
            var accessTokenPart = string.Empty;
            if (accessToken != null)
            {
                accessTokenPart = (path.Contains("?") ? "&" : "?") + "access_token=" + accessToken;
            }

            return path + accessTokenPart;
        }

        private string AddCursorToPathIfNeeded(string path, FacebookCursor cursor, Direction cursorDirection)
        {
            string cursorPart = string.Empty;

            if (cursor != null && cursorDirection != Direction.None)
            {
                if (!string.IsNullOrWhiteSpace(cursor.After) &&
                   (cursorDirection == Direction.After || cursorDirection == Direction.Next))
                {
                    cursorPart = (path.Contains("?") ? "&" : "?") + "after=" + cursor.After;
                }
                else if (!string.IsNullOrWhiteSpace(cursor.Before))
                {
                    cursorPart = (path.Contains("?") ? "&" : "?") + "before=" + cursor.Before;
                }
            }

            return path + cursorPart;
        }

        internal JObject SerializeResponse(IRestResponse<string> response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    var jsreader = new JsonTextReader(new StringReader(response.RawData.ToString()));
                    return (JObject)new JsonSerializer().Deserialize(jsreader);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                throw new FacebookApiException(response.RawData.ToString(), response.Exception);
            }
        }


    }
}
