using MySql.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsTicketToJIRA
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Issue
    {
        public int OsTicketId { get; set; }

        public string Number { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("reporter")]
        public string Reporter { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("assignee")]
        public string Assignee { get; set; }

        [JsonProperty("comments")]
        public List<Comment> Comments { get; set; }

        [JsonProperty("customFieldValues")]
        public List<CustomFieldValue> CustomFieldValues { get; set; }
        public Issue()
        {
            this.Comments = new List<Comment>();
            this.CustomFieldValues = new List<CustomFieldValue>();
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CustomFieldValue
    {
        [JsonProperty("fieldName")]
        public string FieldName { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("fieldType")]
        public string FieldType { get; set; }

    }
}
