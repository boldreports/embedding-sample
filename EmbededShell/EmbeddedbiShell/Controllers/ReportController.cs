using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BoldReports.RDL.DOM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SampleCoreApp.Models;
using static SampleCoreApp.Models.ReportModel;

namespace SampleCoreApp.Controllers
{

    public class ReportController : Controller
    {
        private readonly GlobalAppSettings _globalAppSettings;
        private readonly TenantModel _tenantModel = new TenantModel();
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public ReportController(IHttpContextAccessor contextAccessor, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _globalAppSettings = _tenantModel.GetTenantConfig(contextAccessor.HttpContext.Request.Host.Value);
        }

        internal void WriteLogs(string errorMessage)
        {
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "ReportErrorDetails.txt");

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.AutoFlush = true;
                writer.WriteLine(DateTime.Now.ToString() + "\n" + errorMessage);
            }
        }

        public IActionResult Viewer(string categoryName, string sampleName, int tabid = 0, string email = null, string id = null, string token = null)
        {
            var noResourceFound = false;
            var updatedSettings = _globalAppSettings;
            ServerUser userDetails = null;
            SamplesTreeViewModel model = null;
            ViewBag.UserId = 1;
            if (categoryName == null)
            {
                updatedSettings = _tenantModel.GetUpdateSchema(_globalAppSettings);

                var _model = updatedSettings.SamplesCollection[0];
                if (_model.HasChild && !_model.AsTab)
                {
                    var userID = userDetails == null ? 1 : userDetails.UserId;
                    model = updatedSettings.SamplesCollection.FirstOrDefault(i => (i.ParentId == _model.Id && i.Id != 101 && i.CreatedById == userID));
                    ViewBag.ParentName = model != null ? model.ParentName : "";
                    ViewBag.Name = model != null ? model.Name : "";
                    noResourceFound = model == null;
                }
                else
                {
                    model = _model;
                    ViewBag.Name = model.Name;
                    noResourceFound = true;
                }
                ViewBag.GlobalAppSettings = updatedSettings;
                var reportModel = new ReportModel();
                var reportToken = reportModel.GetTokenString(email);
                ViewBag.ServiceAuthorizationToken = reportToken;
            }
            else
            {
                if (token != null)
                {
                    token = Uri.UnescapeDataString(token);
                }

                noResourceFound = false;
                updatedSettings = _tenantModel.GetUpdateSchema(_globalAppSettings);
                //SamplesTreeViewModel model = null;
                ViewBag.UserId = 1;
                ViewBag.IsBoldReports = true;
                ViewBag.ConnectionToken = token;
                var reportModel = new ReportModel();
                ViewBag.Email = _globalAppSettings.EmbedDetails.ReportEmail;
                ViewBag.edit = false;
                sampleName = sampleName.Contains("&") ? sampleName.Substring(0, sampleName.IndexOf("&")) : sampleName;
                model = updatedSettings.SamplesCollection.FirstOrDefault(i => i.Name.ToLower() == sampleName.ToLower() && i.ParentName.ToLower() == categoryName.ToLower());
                ViewBag.ParentName = model.ParentName;
                ViewBag.Name = model.Name;
                ViewBag.GlobalAppSettings = updatedSettings;
                ViewBag.NoResourceFound = noResourceFound;
                ViewBag.sample = sampleName;
                ViewBag.categoryName = categoryName;
                var reportToken = reportModel.GetTokenString(email);
                ViewBag.ServiceAuthorizationToken = reportToken;
            }

            if (model != null)
            {
                return View(model);
            }

            return View();
        }

        [HttpPost]
        [Route("DeleteItem")]
        public string DeleteItem(string itemId, string userEmail)
        {
            var response = new ReportModel().DeleteReportItem(itemId, userEmail);
            return response.StatusCode.ToString();
        }

        [HttpPost]
        [Route("DataSet")]
        public object DataSet()
        {
            List<ReportModel.ApiItems> response = new ReportModel().GetSharedDatset();
            FileStream readStream = System.IO.File.OpenRead(@response[0].ItemLocation);

            ReportSerializer reportSerializer = new ReportSerializer();
            var dataset = reportSerializer.GetSharedDataSet(readStream);
            string name = response[0].Name;
            DataSet Data = new DataSet();
            if ((string.IsNullOrEmpty(name) != true))
            {
                Data.Name = name;
                if (dataset.DataSet.Fields.Count > 0)
                {
                    Data.Fields = new List<DataSetFields>();
                    for (int i = 0; i <= dataset.DataSet.Fields.Count - 1; i++)
                        Data.Fields.Add(new DataSetFields()
                        {
                            DataField = dataset.DataSet.Fields[i].DataField,
                            Name = dataset.DataSet.Fields[i].Name,
                            TypeName = dataset.DataSet.Fields[i].TypeName
                        });
                    Data.SharedDataSet = new SharedDataSetInfo()
                    {
                        SharedDataSetReference = name
                    };
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(Data);
        }

        [Route("Designer")]
        public IActionResult Designer(string categoryName = null, string sampleName = null, string email = null, string name = null, string dsName = null, string dsList = null, bool edit = false, string token = null, string id = null)
        {
            if (token != null)
            {
                token = Uri.UnescapeDataString(token);
            }
            //var selectedDataInfo = new List<ReportModel.SharedDataInfo>();
            var updatedSettings = _globalAppSettings;
            SamplesTreeViewModel model = null;
            string userToken;
            //var sitename = User.FindFirst(ClaimIdentifiers.ReportsTenantName.ToString()).Value.ToString();
            //var biSiteName = User.FindFirst(ClaimIdentifiers.TenantName.ToString()).Value.ToString();
            //var adminType = int.Parse(User.FindFirst(ClaimIdentifiers.BoldBIAdminType.ToString()).Value.ToString());
            //var reportsUserId = User.FindFirst(ClaimIdentifiers.ReportsUserId.ToString()).Value.ToString();
            var userId = "1";
            ViewBag.UserId = userId;

            var emailId = _globalAppSettings.EmbedDetails.ReportEmail;
            //var reportModel = new ReportModel(sitename);
            ViewBag.ConnectionToken = token;
            var reportId = string.IsNullOrEmpty(id) ? "" : id;
            ViewBag.ReportId = reportId;
            ViewBag.DraftId = reportId;

            userToken = new ReportModel().GetTokenString(emailId);

            //List<ReportModel.SharedDataInfo> sharedDataInfo = await Task.Run(() => this.GetSharedDataInfo(reportModel.GetItems(userToken, "Dataset"), int.Parse(reportsUserId)));

            //if (!string.IsNullOrEmpty(dsName))
            //{
            //    selectedDataInfo = sharedDataInfo.Where(dataInfo => dataInfo.Name.ToLower() == dsName.ToLower()).ToList();
            //    if (selectedDataInfo == null || selectedDataInfo.Count == 0)
            //    {
            //        selectedDataInfo = sharedDataInfo;
            //    }
            //}
            //else if (!string.IsNullOrEmpty(dsList))
            //{
            //    List<string> datasets = JsonConvert.DeserializeObject<List<string>>(dsList);
            //    selectedDataInfo = sharedDataInfo.Where(dataInfo => datasets.Any(data => data == dataInfo.Id.ToString())).ToList();
            //    if (selectedDataInfo == null || selectedDataInfo.Count == 0)
            //    {
            //        selectedDataInfo = sharedDataInfo;
            //    }
            //}

            if (edit && sampleName != null
                && categoryName != null && updatedSettings.SamplesCollection != null && updatedSettings.SamplesCollection.Count > 0)
            {
                updatedSettings = _tenantModel.GetUpdateSchema(_globalAppSettings);

                model = updatedSettings.SamplesCollection.FirstOrDefault(i => i.Name.ToLower() == sampleName.ToLower() && i.ParentName.ToLower() == categoryName.ToLower());

                if (model != null)
                {
                    model.IsEdit = edit;

                    var menuItem = updatedSettings.SamplesSchemaCollection.FirstOrDefault(i => i.Name.ToLower() == categoryName.ToLower());

                    if (menuItem != null && menuItem.Samples != null && menuItem.Samples.Count > 0)
                    {

                        var sample = menuItem.Samples.FirstOrDefault(j => j.Name.ToLower() == sampleName.ToLower());
                        sample.IsEdit = edit;
                    }
                    var reports = new ReportModel().GetItems(userToken, "Report").FirstOrDefault(k => k.Name == model.Name);
                    var reportLocation = reports.ItemLocation.Split("\\");
                    var version = reportLocation[reportLocation.Length - 1];
                    ViewBag.Version = version;
                    string reportPath = model.DashboardPath.Trim('/');
                    ViewBag.CatagoryName = reportPath.Substring(0, reportPath.IndexOf("/"));
                    ViewBag.ReportName = model.Name;
                    ViewBag.isDraft = false;
                }

                ViewBag.IsEdit = edit;
            }
            else if (name != null)
            {
                updatedSettings = _tenantModel.GetUpdateSchema(_globalAppSettings);

                string catagoryName = "UserBI";

                var values = emailId.Split('@');
                var userName = values[0];
                if (!string.IsNullOrEmpty(values[1]))
                {
                    var lastname = values[1].Split('.');
                    foreach (var text in lastname)
                    {
                        userName = userName + "_" + text;
                    }

                    catagoryName = userName;
                }

                var categories = new ReportModel().GetCategories(emailId);
                var categoryExist = categories != null && categories.Count > 0 ? categories.FirstOrDefault(i => i.CreatedById == int.Parse(userId) && i.Name == catagoryName) : null;
                if (categoryExist == null)
                {
                    var categoryId = new ReportModel().AddCategory(catagoryName, userToken);
                    ViewBag.CatagoryId = categoryId;
                }
                else
                {
                    var firstCategory = categories != null && categories.Count > 0 ? categories.FirstOrDefault(i => i.CreatedById == int.Parse(userId) && i.Name == catagoryName) : null;
                    ViewBag.CatagoryId = firstCategory?.Id;
                }

                ViewBag.CatagoryName = catagoryName;
                ViewBag.ReportName = name;
                ViewBag.NewReportName = name;

                ViewBag.IsEdit = edit;
                ViewBag.isDraft = true;
                ViewBag.Version = null;

                //ViewBag.SharedDatasets = await Task.Run(() => this.GetSharedDatasets(reportModel, userToken, selectedDataInfo, null));
            }

            ViewBag.GlobalAppSettings = updatedSettings;
            ViewBag.ParentName = "Reports";
            ViewBag.Name = "";
            ViewBag.CurrentUser = emailId;
            //try
            //{
            //    if (ViewBag.SharedDatasets == null)
            //    {
            //        ViewBag.SharedDatasets = this.GetSharedDatasets(reportModel, userToken, selectedDataInfo, null);
            //    }

            //    ViewBag.SharedDataInfo = this.SerializeDOM(sharedDataInfo);
            //}
            //catch (Exception ex)
            //{
            //    LogExtension.LogError("Error occurred while getting shared datasets", ex, MethodBase.GetCurrentMethod());
            //}

            //ViewBag.AdminType = (AdminType)adminType;
            ViewBag.IsBoldReports = true;
            ViewBag.IsReportDesigner = true;
            ViewBag.ServiceAuthorizationToken = userToken;
            //ViewBag.tenantSiteName = sitename;
            ViewBag.sample = string.IsNullOrWhiteSpace(name) ? sampleName : name;
            ViewBag.categoryName = "reports";
            ViewBag.edit = edit;

            if (model != null)
            {
                return View(model);
            }

            return View();
        }

        [HttpPost]
        [Route("draft-item/report")]
        public async Task<object> AddReportDraft(string itemName, string userEmail)
        {
            var response = new ReportModel().AddReportDraft(itemName, userEmail);
            return response;
        }

        [Route("item-exist/report")]
        public async Task<string> IsReportExist(string itemName, string categoryName, string userEmail)
        {
            var response = await Task.Run(() => new ReportModel().IsItemExist("Report", itemName, categoryName, userEmail));
            return response.ToString();
        }
    }
}
class DataSet
{
    public string __type = "BoldReports.RDL.DOM.DataSet";
    public string Name { get; set; }
    public List<DataSetFields> Fields { get; set; }
    public SharedDataSetInfo SharedDataSet { get; set; }
}

class DataSetFields
{
    public string __type = "BoldReports.RDL.DOM.Field";
    public string DataField { get; set; }
    public string Name { get; set; }
    public string TypeName { get; set; }
}
class SharedDataSetInfo
{
    public string SharedDataSetReference { get; set; }
}

