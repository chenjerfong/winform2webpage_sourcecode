using System;
namespace WinForm
{
    public interface ICountFolders
    {
        event Action<string, ICountFolders> ReportStep;
        string ConnectionId { get; set; }
        DateTime StartTime { get; set; }
        void DoWork(string path);
        void StopWork();
        bool TryDequeue(out GridData validated);
    }
}
