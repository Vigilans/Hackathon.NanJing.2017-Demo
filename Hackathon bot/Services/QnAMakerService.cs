using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HackathonBot.Services
{
    public static class QnAMakerService
    {
        static private string knowledgeBaseID = "3db92060-f4a3-4e6d-8907-30ea6df2acfd";
        static private string subscriptionKey = "b5f9d3d0e78f471587595bf11d8acaf8";

        static public async Task<string> GenerateAnswer(string question)
        {
            string responseString = string.Empty;

            //Build the URI
            Uri uriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0");
            var builder = new UriBuilder($"{uriBase}/knowledgebases/{knowledgeBaseID}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{question}\", \"top\": 3}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }

            return responseString;
        }
    }
}
