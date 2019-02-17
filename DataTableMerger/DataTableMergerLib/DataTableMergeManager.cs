using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Qazi.Common;

namespace Qazi.DataPreprocessing
{
    public class DataTableMergeManager
    {
        public DataTable _MegeredDataTable;
        private List<DataTable> _ListOfDataTables;

        public event WorkerProgressUpdateEventHandler ProgressUpdate;
        public event WorkerCompletedEventHandler ProgressCompleted;
        public WorkerProgressEventArg ProgressArg;

        public DataTableMergeManager(List<DataTable> listOfTables)
        {
            _ListOfDataTables = listOfTables;
            _MegeredDataTable = new DataTable();
            _MegeredDataTable = _ListOfDataTables[0].Clone();
            ProgressArg = new WorkerProgressEventArg();
        }

        public void Run()
        {
            int rowIndex;
            DataRow row;
            int totalRows;
            float progress;
            int tableIndex = 0;
            foreach (DataTable dt in _ListOfDataTables)
            {
                tableIndex++;
                totalRows = dt.Rows.Count;
                for(rowIndex = 0; rowIndex < totalRows; rowIndex++)
                {
                    row = dt.Rows[rowIndex];
                    _MegeredDataTable.ImportRow(row);
                    progress = (((float)(rowIndex + 1)) / ((float)(totalRows)) * 100);
                    if (ProgressUpdate != null)
                    {
                        ProgressArg.ProgressPercentage = progress;
                        ProgressArg.UserState = "Merging table no. " + tableIndex.ToString();
                        ProgressUpdate(this, ProgressArg);
                    }
                }

            }

            if (ProgressCompleted != null)
            { 
                WorkerCompletedEventArg e = new WorkerCompletedEventArg();
                e.Result = _MegeredDataTable;
                e.UserStateMessage = "Finished";
                ProgressCompleted(this, e);
            }
        }
    }
}
