using System;
namespace WinForm
{
    public interface ICountFolders
    {
        event Action<string, ICountFolders> ReportStep;
        void DoWork(string path);
        void StopWork();
        bool TryDequeue(out GridData validated);
    }
}
