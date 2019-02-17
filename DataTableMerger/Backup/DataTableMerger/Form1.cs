using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Qazi.GUI.CommonDialogs;
using Qazi.DataPreprocessing;
using Qazi.Common;

namespace DataTableMerger
{
    public partial class Form1 : Form
    {
        private DataTableSelectorWnd dtwnd;
        private DataSet ds;
        private List<DataTable> dataTableCollection;
        private DataTable infoTable;
        private DataRow row;
        private DataTableMergeManager dtmanager;
        private DataTable _MergedDataTable;
        public Form1()
        {
            InitializeComponent();
            dataTableCollection = new List<DataTable>();
            infoTable = new DataTable();
            infoTable.Columns.Add("TableName");
            infoTable.Columns.Add("Path");
            infoTable.Columns.Add("RecordCount");
            dataGridView1.DataSource = infoTable;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            ds = new DataSet();
            ds.ReadXml(openFileDialog1.FileName);
            dtwnd = new DataTableSelectorWnd("Select DataTable", ds);
            dtwnd.ShowDialog();
            string tableName = dtwnd.TableName;
            dataTableCollection.Add(ds.Tables[tableName]);

            row = infoTable.NewRow();
            row["TableName"] = tableName;
            row["Path"] = openFileDialog1.FileName;
            row["RecordCount"] = ds.Tables[dtwnd.TableName].Rows.Count.ToString();
            infoTable.Rows.Add(row);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dtmanager = new DataTableMergeManager(dataTableCollection);
            dtmanager.ProgressUpdate += new WorkerProgressUpdateEventHandler(dtmanager_ProgressUpdate);
            dtmanager.ProgressCompleted += new WorkerCompletedEventHandler(dtmanager_ProgressCompleted);
            dtmanager.Run();
        }

        void dtmanager_ProgressCompleted(object sender, WorkerCompletedEventArg e)
        {
            _MergedDataTable = (DataTable)e.Result;
            dataGridView2.DataSource = _MergedDataTable;
            lblStatus.Text  = "Total Merged Records: " + _MergedDataTable.Rows.Count.ToString();
            progressBar1.Value = 0;
            Application.DoEvents();
        }

        void dtmanager_ProgressUpdate(object sender, WorkerProgressEventArg e)
        {
            progressBar1.Value = (int)e.ProgressPercentage;
            lblStatus.Text = e.UserState;
            Application.DoEvents();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            _MergedDataTable.WriteXml(saveFileDialog1.FileName);
            MessageBox.Show("XML file saved");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }
    }
}