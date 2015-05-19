using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinForm
{
    public class CountFolders : ICountFolders
    {
        public event Action<string, ICountFolders> ReportStep;
        private void callReportStep(string step)
        {
            if (ReportStep != null)
            {
                ReportStep(step, this);
            }
        }


        /// <summary>
        /// 開始
        /// </summary>
        public void DoWork(string path)
        {
            int fileCount;
            long totalSize;
            explore(path, out fileCount, out totalSize);
            QueueAndIndicate(path, fileCount, totalSize);
            if (stop)
                callReportStep("執行已取消");
            else
                callReportStep("執行完成！");
        }

        private bool stop = false;
        private void explore(string path, out int fileCount, out long totalSize)
        {
            callReportStep(string.Format("正在計算{0}...", path));
            Thread.Sleep(1000);

            fileCount = 0;
            totalSize = 0;
            //使用遞迴搜索所有目錄
            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    if (stop)
                        return;

                    int _fileCount;
                    long _totalSize;
                    explore(dir, out _fileCount, out _totalSize);
                    QueueAndIndicate(dir, _fileCount, _totalSize);

                    fileCount += _fileCount;
                    totalSize += _totalSize;
                }
            }
            catch
            {
            }
            foreach (string file in Directory.GetFiles(path))
            {
                if (stop)
                    return;

                fileCount++;
                //加總檔案大小
                FileInfo fi = new FileInfo(file);
                totalSize += fi.Length;
            }
        }

        private ConcurrentQueue<GridData> queue = new ConcurrentQueue<GridData>();
        private void QueueAndIndicate(string path, int fileCount, long totalSize)
        {
            queue.Enqueue(new GridData() { Path = path, FileCount = fileCount, TotalSize = totalSize });
            callReportStep(null);
        }


        /// <summary>
        /// 停止
        /// </summary>
        public void StopWork()
        {
            stop = true;
        }


        /// <summary>
        /// 取得記錄
        /// </summary>
        public bool TryDequeue(out GridData validated)
        {
            return queue.TryDequeue(out validated);
        }
    }

    public class GridData
    {
        public string Path { get; set; }
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
    }

}
