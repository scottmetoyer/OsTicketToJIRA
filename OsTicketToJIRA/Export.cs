using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsTicketToJIRA
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Export
    {
        [JsonProperty("projects")]
        public List<Project> Projects { get; set; }

        public Export()
        {
            this.Projects = new List<Project>();
        }
    }
}
