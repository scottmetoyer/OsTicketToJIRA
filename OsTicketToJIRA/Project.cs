using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsTicketToJIRA
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Project
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("issues")]
        public List<Issue> Issues
        {
            get;set;
        }

        public Project ()
        {
            this.Issues = new List<Issue>();
        }
    }
}
