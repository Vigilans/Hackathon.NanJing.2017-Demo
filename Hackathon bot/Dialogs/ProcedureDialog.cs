using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using HackathonBot.Models;

namespace HackathonBot.Dialogs
{
    [Serializable]
    public class ProcedureDialog : IDialog<object>
    {
        private Procedure procedure;

        public ProcedureDialog(Procedure procedure)
        {
            this.procedure = procedure;
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (procedure.StagesByTargets.Count != 1)
            { // 有多种服务对象
                int targetID = -1;
                if (context.ConversationData.TryGetValue("procedure_target", out string target))
                {
                    targetID = procedure.StagesByTargets.FindIndex(0, procedure.StagesByTargets.Count, t => t.Name == target);
                    context.ConversationData.RemoveValue("procedure_target");
                    context.ConversationData.SetValue("targetID", targetID);
                }

                if (targetID == -1 || targetID == procedure.StagesByTargets.Count)
                {
                    //PromptDialog.Choice(
                    //    context,
                    //    DecideTarget,
                    //    procedure.StagesByTargets.Select(t => t.Name),
                    //    "该流程有不同的适用对象，请选择您所需要查询的一个：",
                    //    descriptions: procedure.StagesByTargets.Select(t => t.Name + "（" + t.Explanation + "）"));
                    targetID = 0;
                    context.ConversationData.SetValue("targetID", targetID);
                }
            }

            await ChooseSection(context);
        }

        public async Task DecideTarget(IDialogContext context, IAwaitable<string> result)
        {
            var name = await result;
            int targetID = procedure.StagesByTargets.FindIndex(0, procedure.StagesByTargets.Count, t => t.Name == name);
            context.ConversationData.SetValue("targetID", targetID);
        }

        public async Task ChooseSection(IDialogContext context)
        {

            PromptDialog.Choice( // 选择要查看的内容
                context,
                async (ct, ret) =>
                {
                    string choice = await ret;
                    switch (choice)
                    {
                        case "流程":
                            await ShowSteps(ct);
                            break;
                        case "注意事项":
                            await ShowNotes(ct);
                            break;
                    }
                },
                new string[] { "流程", "注意事项" },
                "您想要查看哪个部分？");
        }

        public async Task ShowSteps(IDialogContext context)
        {
            var message = context.Activity.AsMessageActivity();
            var reply = ((Activity)message).CreateReply();

            var targetID = context.ConversationData.TryGetValue("targetID", out int ID) ? ID : 0;
            var stages = procedure.StagesByTargets[targetID].Stages;
            context.ConversationData.RemoveValue("targetID");

            reply.Text += $"### {procedure.StagesByTargets[targetID]}任务流程：\n\n\n";
            foreach (var stage in stages)
            {
                reply.Text += stage.Title.Any() ? $"**{stage.Title}**:\n\n" : "";
                for (int i = 0; i < stage.Steps.Count; i++)
                {
                    switch (stage.Steps[i].Type)
                    {
                        case ContentType.Text:
                            reply.Text += $"{i + 1}. {stage.Steps[i].Value}\n\n";
                            break;
                        case ContentType.Image:
                            reply.Text += $"![]({context.ConversationData.GetValue<string>("domain") + "/" + stage.Steps[i].Value})" + "\n\n";
                            break;
                    }    
                }
                reply.Text += "\n";
            }

            var connector = new ConnectorClient(new Uri(reply.ServiceUrl));
            await connector.Conversations.SendToConversationAsync(reply);

            await CheckContinue(context);
        }

        public async Task ShowNotes(IDialogContext context)
        {
            var message = context.Activity.AsMessageActivity();
            var reply = ((Activity)message).CreateReply();

            reply.Text += "### 注意事项: \n\n";
            for (int i = 0; i < procedure.Notes.Count; i++)
            {
                reply.Text += $"{i + 1}. {procedure.Notes[i]}\n\n"; 
            }

            var connector = new ConnectorClient(new Uri(reply.ServiceUrl));
            await connector.Conversations.SendToConversationAsync(reply);

            await CheckContinue(context);
        }

        public async Task CheckContinue(IDialogContext context)
        {
            PromptDialog.Confirm(
                context,
                async (ct, ret) =>
                {
                    if (await ret)
                    {
                        await ChooseSection(ct);
                    }
                    else
                    {
                        ct.Done(true);
                    }
                },
                "是否需要继续查看该流程？",
                "您的输入有误！");
        }
    }
}
