using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HackathonBot.Models;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace HackathonBot.Services
{
    [Serializable]
    public static class AzureSearchService
    {
        private static readonly string queryString = $"https://hackathonbot.search.windows.net/indexes/hackathondb-index/docs?api-key=4BF395AAFEDD2C4FBC1DE72443766E6F&api-version=2016-09-01&";

        public static async Task<SearchResult> SearchByTitle(string title)
        {
            using (var httpClient = new HttpClient())
            {
                string nameQuery = $"{queryString}search={title}";
                string response = await httpClient.GetStringAsync(nameQuery);
                var result = JsonConvert.DeserializeObject<SearchResult>(response);
                var values = (JArray)(JObject.Parse(response)["value"]);
                for (int i = 0; i < result.Values.Count; i++)
                {
                    var jsonStr = string.Empty;
                    switch (result.Values[i].Type)
                    { // so many dirty hacks here...
                        case SearchValueType.Guide:
                            jsonStr = values[i]["sections"].ToString();
                            values[i]["sections"] = JArray.Parse(jsonStr);
                            result.Values[i].Article = values[i].ToObject<Guide>();
                            break;
                        case SearchValueType.Procedure:
                            jsonStr = values[i]["stagesByTargets"].ToString();
                            values[i]["stagesByTargets"] = JArray.Parse(jsonStr);
                            result.Values[i].Article = values[i].ToObject<Procedure>();
                            break;
                    }
                }
                return result;
            }
        }
    }
}
