using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using HackathonBot.Dialogs;

namespace HackathonBot.Controllers
{

    [Route("api/[controller]")]
    [BotAuthentication]
    public class MessagesController : Controller
    {
        private ILogger logger;

        public MessagesController(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            this.logger = loggerFactory.CreateLogger<MessagesController>();
        }

        public virtual async Task<IActionResult> Post([FromBody] Activity activity)
        {
            if (activity != null)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // one of these will have an interface and process it
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        ///await Conversation.SendAsync(activity, MakeRoot);
                        await Conversation.SendAsync(activity, () => new FinanceDialog(Request.Scheme + "://" + Request.Host.Value));
                        break;

                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        logger.LogWarning("Unknown activity type ignored: {0}", activity.GetActivityType());
                        break;
                }
            }
            return Ok();
        }
    }
}