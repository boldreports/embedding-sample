using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SampleCoreApp.Models
{
    [Serializable]
    [DataContract]
    public class APIResponse
    {
        [JsonProperty("Id")]
        public string Id
        {
            get;
            set;
        }

        [JsonProperty("Name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("DashboardPath")]
        public string DashboardPath
        {
            get;
            set;
        }

        [JsonProperty("Description")]
        public string Description
        {
            get;
            set;
        }

        [JsonProperty("ItemLocation")]
        public string ItemLocation { get; set; }

        [JsonProperty("CategoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("CreatedById")]
        public int CreatedById
        {
            get;
            set;
        }

        [JsonProperty("CreatedByDisplayName")]
        public string CreatedByDisplayName
        {
            get;
            set;
        }

        [JsonProperty("IsPublic")]
        public bool IsPublic
        {
            get;
            set;
        }
    }
}