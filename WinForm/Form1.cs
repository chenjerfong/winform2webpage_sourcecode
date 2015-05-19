using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm
{
    public partial class Form1 : Form
    {
        ICountFolders my_CountFolders;

        public Form1()
        {
            InitializeComponent();

            my_CountFolders = new CountFolders();
            my_CountFolders.ReportStep += my_CountFolders_ReportStep;
        }

        private void my_CountFolders_ReportStep(string step, ICountFolders arg2)
        {
            backgroundWorker1.ReportProgress(0, step);
        }

        string path;
        private void button1_Click(object sender, EventArgs e)
        {
            path = textBox1.Text;

            button1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
            button2.Enabled = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            my_CountFolders.DoWork(path);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState == null)
            {
                GridData info;
                while (my_CountFolders.TryDequeue(out info))
                {
                    dataGridView1.Rows.Add(info.Path, info.FileCount, info.TotalSize.ToString("0,000"));
                }
            }
            else
            {
                label1.Text = e.UserState.ToString();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button2.Enabled = false;
            if (e.Error != null)
            {
                label1.Text = e.Error.Message;
            }
            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            my_CountFolders.StopWork();
            backgroundWorker1.CancelAsync();
        }
    }
}
