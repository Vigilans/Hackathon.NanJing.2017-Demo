using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonBot.Models
{
    //Data model for search
    [Serializable]
    public class SearchResult
    {
        [JsonProperty("@odata.context")]
        public string ODataContext { get; set; }
        [JsonProperty("value")]
        public List<SearchValue> Values { get; set; }
    }

    [Serializable]
    public enum SearchValueType
    {
        Guide,
        Procedure,
        Answer
    }

    [Serializable]
    public class SearchValue
    {
        [JsonProperty("@search.score")]
        public float Score { get; set; }
        [JsonProperty("class")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchValueType Type { get; set; }
        [JsonIgnore]
        public IArticle Article { get; set; }
        public string ID { get; set; }
        public string RID { get; set; }
    }
}
