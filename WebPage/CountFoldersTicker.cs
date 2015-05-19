using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebPage.Hubs;
using WinForm;

namespace WebPage
{
    public class CountFoldersTicker
    {
        // Singleton instance
        private readonly static Lazy<CountFoldersTicker> _instance = new Lazy<CountFoldersTicker>(
            () => new CountFoldersTicker(GlobalHost.ConnectionManager.GetHubContext<MyHub>().Clients));
        public static CountFoldersTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients { get; set; }

        private CountFoldersTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        private ConcurrentDictionary<string, ICountFolders> countFoldersEngines = new ConcurrentDictionary<string, ICountFolders>();
        public void SetConnectionId(string connectionId)
        {
            var countFolders = new CountFolders()
            {
                ConnectionId = connectionId,
                StartTime = DateTime.Now
            };
            countFoldersEngines[connectionId] = countFolders;

            countFolders.ReportStep += ShowStepAndResult;
            try
            {
                Task.Factory.StartNew(() =>
                {
                    DateTime now = DateTime.Now;
                    foreach (var _connectionId in countFoldersEngines.Keys)
                    {
                        if ((now - countFoldersEngines[_connectionId].StartTime).Days > 2)
                        {
                            ICountFolders r;
                            countFoldersEngines.TryRemove(_connectionId, out r);
                        }
                    }
                });
            }
            catch
            {
            }
        }

        public ICountFolders GetCountFolders(string connectionId)
        {
            return countFoldersEngines[connectionId];
        }

        private void ShowStepAndResult(string step, ICountFolders countFolders)
        {
            if (!string.IsNullOrWhiteSpace(countFolders.ConnectionId))
            {
                if (step == null)
                {
                    GridData data;
                    while (countFolders.TryDequeue(out data))
                    {
                        Clients.Client(countFolders.ConnectionId).ReportResult(data.Path, data.FileCount, data.TotalSize);
                    }
                }
                else
                {
                    Clients.Client(countFolders.ConnectionId).ReportStep(step);
                }
            }
        }
    }
}