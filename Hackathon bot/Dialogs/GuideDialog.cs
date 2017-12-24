using HackathonBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;

namespace HackathonBot.Dialogs
{
    [Serializable]
    public class GuideDialog : IDialog<object>
    {
        private Guide guide;

        public GuideDialog(Guide guide)
        {
            this.guide = guide;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await CheckSection(context, true);
        }

        public async Task CheckSection(IDialogContext context, bool isFirstTime)
        {
            PromptDialog.Choice(
                context,
                ShowSectionContents,
                guide.Sections.Select(s => s.Title),
                (isFirstTime? "该指南由以下几部分组成，" : "") + "您希望查看哪一项？",
                "您的输入有误！");
        }

        public async Task ShowSectionContents(IDialogContext context, IAwaitable<string> result)
        {
            var title = await result;
            var section = guide.Sections.Find(s => s.Title == title);
            var message = context.Activity.AsMessageActivity();
            var reply = ((Activity)message).CreateReply();

            foreach (var content in section.Contents)
            {
                switch (content.Type)
                {
                    case ContentType.Text:
                        reply.Text += content.Value + "\n\n";
                        break;
                    case ContentType.Image:
                        reply.Text += $"![]({context.ConversationData.GetValue<string>("domain") + "/" + content.Value})" + "\n\n";
                        break;
                }
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
                        await CheckSection(ct, true);
                    }
                    else
                    {
                        ct.Done(true);
                    }
                },
                "是否需要继续查看该指南？",
                "您的输入有误！");
        }
    }
}
