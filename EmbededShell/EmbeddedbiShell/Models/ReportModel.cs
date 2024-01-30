using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SampleCoreApp.Models
{
    public class ReportModel
    {
        private readonly GlobalAppSettings _globalAppSettings;
        string ReportServerApiUrl = string.Empty;
        private string tokenurl = string.Empty;
        private string getItemsURL = string.Empty;
        private string nonce = Guid.NewGuid().ToString();
        private string timeStamp = DateTimeToUnixTimeStamp(DateTime.UtcNow).ToString();

        public ReportModel()
        {
            _globalAppSettings = new GlobalAppSettings();
            var filePath = Startup.BasePath + "/App_Data/" + "default";
            _globalAppSettings = new TenantModel().GetEmbedDetails(filePath, _globalAppSettings);
            ReportServerApiUrl = this._globalAppSettings.EmbedDetails.ReportRootUrl + "/api/" + _globalAppSettings.EmbedDetails.ReportSiteIdentifier;

        }

        public ReportModel(string sitename)
        {
            _globalAppSettings = new GlobalAppSettings();
            var filePath = Startup.BasePath + "/App_Data/" + "default";
            _globalAppSettings = new TenantModel().GetEmbedDetails(filePath, _globalAppSettings);
            ReportServerApiUrl = this._globalAppSettings.EmbedDetails.ReportRootUrl + "/api/" + _globalAppSettings.EmbedDetails.ReportSiteIdentifier;
        }

        public List<ReportModel.ApiItems> GetSharedDatset()
        {
            string email = null;
            var token = GetToken(email);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                client.DefaultRequestHeaders.Add("ETag", RandomString());
                var result = client.GetAsync(_globalAppSettings.EmbedDetails.ReportRootUrl + "/api/" + _globalAppSettings.EmbedDetails.ReportSiteIdentifier + "/v1.0/items?ItemType=dataset").Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                List<ReportModel.ApiItems> response = JsonConvert.DeserializeObject<List<ReportModel.ApiItems>>(resultContent);
                return response;
            }
        }

        public string GetReports(string email = null)
        {
            var token = GetToken(email);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                client.DefaultRequestHeaders.Add("ETag", RandomString());
                var result = client.GetAsync(_globalAppSettings.EmbedDetails.ReportRootUrl + "/api/" + _globalAppSettings.EmbedDetails.ReportSiteIdentifier + "/v1.0/items?ItemType=3").Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                return resultContent;
            }
        }

        public List<ApiItems> GetItems(string token, string itemType)
        {
            var result = new List<ApiItems>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.ConnectionClose = false;
                client.DefaultRequestHeaders.Add("Authorization", token);
                var response = client.GetAsync(this.ReportServerApiUrl + "/v1.0/items?itemType=" + itemType).Result;
                string resultContent = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<List<ReportModel.ApiItems>>(resultContent);
            }

            return result == null ? new List<ApiItems>() : result;
        }

        public List<ApiItems> GetCategories(string email)
        {
            var result = new List<ApiItems>();
            var token = GetToken(email);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.ReportRootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                var response = client.GetAsync(_globalAppSettings.EmbedDetails.ReportRootUrl + "/api/" + _globalAppSettings.EmbedDetails.ReportSiteIdentifier + "/v1.0/items?ItemType=Category").Result;
                string resultContent = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<List<ApiItems>>(resultContent);
            }

            return result;
        }

        public string AddCategory(string userName, string token)
        {
            var categoryId = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Name", userName.ToLower())
                });
                client.DefaultRequestHeaders.ConnectionClose = false;
                client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Add("ETag", DashboardModel.RandomString());
                var result = client.PostAsync(this.ReportServerApiUrl + "/v1.0/categories", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<ApiResponse>(resultContent);
                categoryId = response.Status ? response.Data.ToString() : string.Empty;
            }

            return categoryId;
        }

        public HttpResponseMessage DeleteReportItem(string itemId, string email)
        {
            var token = GetToken(email);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.AccessToken);
                var response = client.DeleteAsync(_globalAppSettings.EmbedDetails.ReportRootUrl + "/api/" + _globalAppSettings.EmbedDetails.ReportSiteIdentifier + "/v1.0/items/" + itemId).Result;
                return response;
            }
        }

        public Token GetToken(string email = "")
        {
            email = string.IsNullOrWhiteSpace(email) ? _globalAppSettings.EmbedDetails.ReportEmail : email;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.ReportRootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                string embedMessage = "embed_nonce=" + nonce + "&user_email=" + email + "&timestamp=" + timeStamp;
                string embedSecretKey = _globalAppSettings.EmbedDetails.ReportEmbedSecret;
                string signature = SignURL(embedMessage.ToLower(), embedSecretKey);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "embed_secret"),
                    new KeyValuePair<string, string>("username", email),
                    new KeyValuePair<string, string>("embed_nonce", nonce),
                    new KeyValuePair<string, string>("embed_signature", signature),
                    new KeyValuePair<string, string>("timestamp", timeStamp)
                });

                var result = client.PostAsync(_globalAppSettings.EmbedDetails.ReportRootUrl + "/api/" + _globalAppSettings.EmbedDetails.ReportSiteIdentifier + "/token", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                var tokenObj = JsonConvert.DeserializeObject<Token>(resultContent);
                return tokenObj;
            }
        }

        static double DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        static string SignURL(string embedMessage, string secretcode)
        {
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(secretcode);
            var messageBytes = encoding.GetBytes(embedMessage);
            using (var hmacsha1 = new HMACSHA256(keyBytes))
            {
                var hashMessage = hmacsha1.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashMessage);
            }
        }

        public string GetTokenString(string email = "")
        {
            email = string.IsNullOrWhiteSpace(email) ? _globalAppSettings.EmbedDetails.ReportEmail : email;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.ReportRootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                string embedMessage = "embed_nonce=" + nonce + "&user_email=" + email + "&timestamp=" + timeStamp;
                string embedSecretKey = _globalAppSettings.EmbedDetails.ReportEmbedSecret;
                string signature = SignURL(embedMessage.ToLower(), embedSecretKey);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "embed_secret"),
                    new KeyValuePair<string, string>("username", email),
                    new KeyValuePair<string, string>("embed_nonce", nonce),
                    new KeyValuePair<string, string>("embed_signature", signature),
                    new KeyValuePair<string, string>("timestamp", timeStamp)
                });

                var result = client.PostAsync(_globalAppSettings.EmbedDetails.ReportRootUrl + "/api/" + _globalAppSettings.EmbedDetails.ReportSiteIdentifier + "/token", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                var tokenvalue = JsonConvert.DeserializeObject<Token>(resultContent);
                return tokenvalue.TokenType + " " + tokenvalue.AccessToken;

            }
        }

        //public DesignerApiResponse AddDatasource(Token token, string name, UserEmbedDetails embedDetails, int userId)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.RootUrl);
        //        client.DefaultRequestHeaders.Accept.Clear();

        //        var content = new FormUrlEncodedContent(new[]
        //       {
        //            new KeyValuePair<string, string>("Name", name),
        //            new KeyValuePair<string, string>("Type", embedDetails.DatasourceMode),
        //            new KeyValuePair<string, string>("Connection", embedDetails.Credentials),
        //            new KeyValuePair<string, string>("UserId", userId.ToString())
        //        });

        //        client.DefaultRequestHeaders.Add("Authorization", "bearer" + " " + token.AccessToken);
        //        var result = client.PostAsync(_globalAppSettings.EmbedDetails.RootUrl + "/api/" + _globalAppSettings.EmbedDetails.SiteIdentifier + "/v4.0/data-source/connection", content).Result;
        //        string resultContent = result.Content.ReadAsStringAsync().Result;
        //        return JsonConvert.DeserializeObject<DesignerApiResponse>(resultContent);
        //    }
        //}

        public static string RandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 30)
              .Select(s => s[random.Next(s.Length)]).ToArray()).ToString().ToLower();
        }

        public object AddReportDraft(string itemName, string email)
        {
            var token = GetTokenString(email);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.ReportRootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.ConnectionClose = false;
                client.DefaultRequestHeaders.Add("Authorization", token);
                var response = client.PostAsync(this.ReportServerApiUrl + "/v2.0/reports/draft?name="+itemName,null).Result;
                string resultContent = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<object>(resultContent);
                return result;
            }
        }

        public List<ApiItems> GetReportDrafts(string email = null)
        {
            var result = new List<ApiItems>();
            var token = GetTokenString(email);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.ReportRootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.ConnectionClose = false;
                client.DefaultRequestHeaders.Add("Authorization", token);
                var response = client.GetAsync(this.ReportServerApiUrl + "/v2.0/reports/drafts").Result;
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    string resultContent = response.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<List<ApiItems>>(resultContent);
                }
                return result;
            }
        }

        public async Task<string> IsItemExist(string itemType, string itemName, string categoryName, string email)
        {
            var token = GetTokenString(email);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.ReportRootUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                var content = new FormUrlEncodedContent(new[]
               {
                    new KeyValuePair<string, string>("ItemType", itemType),
                    new KeyValuePair<string, string>("ItemName", itemName),
                    new KeyValuePair<string, string>("CategoryName", categoryName.ToString().ToLower())
                });

                client.DefaultRequestHeaders.Add("Authorization", token);
                var response = client.PostAsync(this.ReportServerApiUrl + "/v1.0/items/is-name-exists", content).Result;
                string resultContent = response.Content.ReadAsStringAsync().Result;
                return resultContent;
            }
        }

        public string IsDraftExists(string name,string email)
        {
            var drafts = GetReportDrafts(email);
            bool isExist = drafts.Any(draft => draft.Name == name);
            return isExist ? "true" : "false";
        }
        public string GetItemLocation(string itemType, string ItemPath)
        {
            var token = GetTokenString(_globalAppSettings.EmbedDetails.Email);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_globalAppSettings.EmbedDetails.ReportRootUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.ConnectionClose = false;
                client.DefaultRequestHeaders.Add("Authorization", token);
                var response = client.GetAsync(this.ReportServerApiUrl + "/v3.0/items/location?itemType=" + itemType + "&serverPath=" + ItemPath).Result;
                string resultContent = response.Content.ReadAsStringAsync().Result;
                return resultContent;
            }
        }

        public class ApiItems
        {
            public bool CanRead { get; set; }

            public bool CanWrite { get; set; }

            public bool CanDelete { get; set; }

            public bool CanSchedule { get; set; }

            public bool CanDownload { get; set; }

            public bool CanOpen { get; set; }

            public bool CanMove { get; set; }

            public bool CanCopy { get; set; }

            public bool CanClone { get; set; }

            public bool CanCreateItem { get; set; }

            public Guid? CategoryId { get; set; }

            public string CategoryName { get; set; }

            public int ModifiedById { get; set; }

            public string CreatedByDisplayName { get; set; }

            public int CreatedById { get; set; }

            public string ModifiedByFullName { get; set; }

            public string ItemLocation { get; set; }

            public Guid Id { get; set; }

            public string CreatedDate { get; set; }

            public string ModifiedDate { get; set; }

            public DateTime ItemModifiedDate { get; set; }

            public DateTime ItemCreatedDate { get; set; }

            public Guid ReportId { get; set; }

            public string ReportName { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public bool IsDraft { get; set; }
            public bool IsPublic {  get; set; }
        }
    }
}