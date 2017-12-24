using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Net;
using HackathonBot.Services;
using HackathonBot.Models;
using System.IO;
using Newtonsoft.Json.Linq;

namespace HackathonBot.Dialogs
{
    [LuisModel("c4022486-30f8-4239-9bf9-e4c74ce9d19c", "c90c469857dc473d8dd14507467c9b93", LuisApiVersion.V2)]
    [Serializable]
    public class IntentDialog : LuisDialog<IArticle>
    {
        private SearchResult searchResult;

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("抱歉，没有理解您的话……");
            context.Wait(MessageReceived);
        }

        [LuisIntent("问答")]
        public async Task QnAs(IDialogContext context, LuisResult result)
        {
            var answers = await QnAMakerService.GenerateAnswer(result.Query);
            var answer = JObject.Parse(answers)["answers"][0]["answer"].ToString();
            await context.PostAsync(answer);
            context.Done<IArticle>(null); // 不返回文段
        }

        [LuisIntent("流程 & 指南")]
        public async Task Procedures(IDialogContext context, LuisResult result)
        {
            var tasks = result.Entities.Where(e => e.Type == "任务").Select(e => e.Entity); // 找出任务目标
            var taskToSearch = string.Concat(tasks.Select(t => t + " "));

            var targets = result.Entities.Where(e => e.Type == "人物").Select(e => e.Entity); // 找出服务对象
            var target = targets.FirstOrDefault();

            if (target != null)
            {
                context.ConversationData.SetValue("procedure_target", target);
            }

            var query = taskToSearch == string.Empty ? result.Query : taskToSearch + target;
            searchResult = await AzureSearchService.SearchByTitle(query);

            PromptDialog.Confirm(context, DecideAfterConfirmAsync,
                "小财为您找到了这条记录：" + searchResult.Values[0].Article.Title +
                "（类型：" + (searchResult.Values[0].Type == SearchValueType.Guide ? "指南" : "流程") + "）  " + 
                "请问这是您想找的吗？",
                "您的输入有误！");
        }

        private async Task DecideAfterConfirmAsync(IDialogContext context, IAwaitable<bool> confirmResult)
        {
            bool confirm = await confirmResult;
            
            if (!confirm)
            {
                var leftChoices = searchResult.Values.Skip(1);
                PromptDialog.Choice(
                    context, 
                    DecideArticle, 
                    leftChoices.Select(v => v.Article.Title),
                    "以下哪条记录是您要寻找的？",
                    "您的输入有误！",
                    descriptions:leftChoices.Select(v => string.Concat(v.Article.Title, "（类型：", v.Type == SearchValueType.Guide ? "指南" : "流程", "）")));
            }
            else
            { // 第一个就是符合要求的
                context.Done(searchResult.Values[0].Article);
            }
        }
        
        private async Task DecideArticle(IDialogContext context, IAwaitable<string> result)
        {
            var title = await result;
            var article = searchResult.Values.Find(v => v.Article.Title == title).Article;
            context.Done(article);
        }
    }
}
