using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.IO;
using HackathonBot;
using Microsoft.Extensions.Configuration;
using System.Threading;
using HackathonBot.Models;

namespace HackathonBot.Dialogs
{
    [Serializable]
    public class FinanceDialog : IDialog<object>
    {
        string domain = string.Empty;

        public FinanceDialog(string domain)
        {
            this.domain = domain;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.ConversationData.SetValue("domain", domain);
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            //Activity reply = ((Activity)message).CreateReply();
            //reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            //List<CardImage> cardImages = new List<CardImage>();
            //cardImages.Add(new CardImage(url: "http://mmbiz.qpic.cn/mmbiz_png/2rc3HjMpRnaYD3AJZLHdqfLdibmcS0VqQDVzzTAcbywHcPSySEalu7GyvB0bS3C5ibnPtfKBvAH88Nd0YdiaeEGBA/640.png?tp=webp&wxfrom=5&wx_lazy=1"));

            //HeroCard card = new HeroCard()
            //{
            //    Title = "仪器设备报销",
            //    Text = "仪器设备报销流程",
            //    Images = cardImages
            //};

            //reply.Attachments.Add(card.ToAttachment());
            //ConnectorClient connector = new ConnectorClient(new Uri(reply.ServiceUrl));
            //await connector.Conversations.SendToConversationAsync(reply);

            await context.Forward(new IntentDialog(), ResumeAfterConfirmAsync, message, CancellationToken.None);
        }

        private async Task ResumeAfterConfirmAsync(IDialogContext context, IAwaitable<IArticle> articleResult)
        { // 将指南/流程分流到对应的对话中
            var article = await articleResult;
            if (article == null)
            {

            }
            else if (article.GetType() == typeof(Guide))
            {
                var guide = (Guide)article;
                context.Call(new GuideDialog(guide), EndConversationAsync);
            }
            else if (article.GetType() == typeof(Procedure))
            {
                var procedure = (Procedure)article;
                context.Call(new ProcedureDialog(procedure), EndConversationAsync);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private async Task EndConversationAsync(IDialogContext context, IAwaitable<object> articleResult)
        {
            await context.PostAsync("此轮对话完毕。请再次输入以开启新一轮对话。");
            context.Wait(MessageReceivedAsync); // 重新等待消息输入
        }
    }
}
