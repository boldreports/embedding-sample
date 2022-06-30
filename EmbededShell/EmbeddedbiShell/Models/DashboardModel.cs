using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SampleCoreApp.Models
{
    public class DashboardModel
    {
        private readonly GlobalAppSettings _globalAppSettings;

        public DashboardModel()
        {
            _globalAppSettings = new GlobalAppSettings();
            var filePath = Startup.BasePath + "/App_Data/" + "default";
            _globalAppSettings = new TenantModel().GetEmbedDetails(filePath, _globalAppSettings);
        }


        public string GetDashboards(string email = null)
        {
            var token = GetToken(email);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                client.DefaultRequestHeaders.Add("ETag", RandomString());
                var result = client.GetAsync(_globalAppSettings.EmbedDetails.RootUrl + "/api/" + _globalAppSettings.EmbedDetails.SiteIdentifier + "/v2.0/items?ItemType=2").Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return resultContent;
            }
        }

        public List<ApiItems> GetCategories(string email)
        {
            var result = new List<ApiItems>();
            var token = GetToken(email);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                var response = client.GetAsync(_globalAppSettings.EmbedDetails.RootUrl + "/api/" + _globalAppSettings.EmbedDetails.SiteIdentifier + "/v2.0/items?ItemType=Category").Result;
                string resultContent = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<List<ApiItems>>(resultContent);
            }

            return result;
        }

        public HttpResponseMessage DeleteDashboard(string itemId, string email)
        {
            var token = GetToken(email);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                var response = client.DeleteAsync(_globalAppSettings.EmbedDetails.RootUrl + "/api/" + _globalAppSettings.EmbedDetails.SiteIdentifier + "/v2.0/items/" + itemId).Result;
                return response;
            }
        }

        public DesignerApiResponse MakeItemPublic(string itemId, string itemType, bool isPublic, string userEmail)
        {
            var token = GetToken(userEmail);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                var content = new FormUrlEncodedContent(new[]
               {
                    new KeyValuePair<string, string>("ItemType", "Dashboard"),
                    new KeyValuePair<string, string>("ItemId", itemId),
                    new KeyValuePair<string, string>("IsPublic", isPublic.ToString().ToLower())
                });

                client.DefaultRequestHeaders.Add("Authorization", "bearer" + " " + token.AccessToken);
                var result = client.PostAsync(_globalAppSettings.EmbedDetails.RootUrl + "/api/" + _globalAppSettings.EmbedDetails.SiteIdentifier + "/public-item/edit", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<DesignerApiResponse>(resultContent);
            }
        }

        public Token GetToken(string email = "")
        {
            email = string.IsNullOrWhiteSpace(email) ? _globalAppSettings.EmbedDetails.Email : email;
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "embed_secret"),
                    new KeyValuePair<string, string>("Username", email),
                    new KeyValuePair<string, string>("embed_secret", _globalAppSettings.EmbedDetails.EmbedSecret)
                });

                var result = client.PostAsync(_globalAppSettings.EmbedDetails.RootUrl + "/api/" + _globalAppSettings.EmbedDetails.SiteIdentifier + "/token", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                var tokenObj = JsonConvert.DeserializeObject<Token>(resultContent);
                return tokenObj;
            }
        }

        public DesignerApiResponse RefreshDashboard(string itemId)
        {
            var token = GetToken();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                var response = client.GetAsync(_globalAppSettings.EmbedDetails.RootUrl + "/api/" + _globalAppSettings.EmbedDetails.SiteIdentifier + "/v4.0/dashboard/" + itemId + "/refresh-data").Result;
                string resultContent = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<DesignerApiResponse>(resultContent);
                return result;
            }
        }

        public DesignerApiResponse AddDatasource(Token token, string name, UserEmbedDetails embedDetails, int userId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                var content = new FormUrlEncodedContent(new[]
               {
                    new KeyValuePair<string, string>("Name", name),
                    new KeyValuePair<string, string>("Type", embedDetails.DatasourceMode),
                    new KeyValuePair<string, string>("Connection", embedDetails.Credentials),
                    new KeyValuePair<string, string>("UserId", userId.ToString())
                });

                client.DefaultRequestHeaders.Add("Authorization", "bearer" + " " + token.AccessToken);
                var result = client.PostAsync(_globalAppSettings.EmbedDetails.RootUrl + "/api/" + _globalAppSettings.EmbedDetails.SiteIdentifier + "/v4.0/data-source/connection", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<DesignerApiResponse>(resultContent);
            }
        }

        public static string RandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 30)
              .Select(s => s[random.Next(s.Length)]).ToArray()).ToString().ToLower();
        }
    }
}
