using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonBot.Models
{
    [Serializable]
    public enum ContentType
    {
        Text,
        Image
    }

    [Serializable]
    public class Content
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentType Type { get; set; }
        public string Value { get; set; }
    }
}
