using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;

namespace WebPage.Hubs
{
    [HubName("myHub")]
    public class MyHub : Hub
    {
        private readonly CountFoldersTicker _instance;

        public MyHub() :
            this(CountFoldersTicker.Instance)
        {
        }

        public MyHub(CountFoldersTicker instance)
        {
            _instance = instance;
        }

        public void Hello()
        {
            Clients.All.hello();
        }

        public string Register()
        {
            if (Context.User == null || Thread.CurrentPrincipal.Identity.IsAuthenticated == false)
            {
                return "對不起,未授權或逾時無法使用. 請重新登入.";
            }

            _instance.SetConnectionId(Context.ConnectionId);
            return "";
        }
    }
}