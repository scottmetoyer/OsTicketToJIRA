using MySql.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsTicketToJIRA
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Comment
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}
