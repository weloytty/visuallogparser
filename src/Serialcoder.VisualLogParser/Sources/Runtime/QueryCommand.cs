using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Threading;
using System.Reflection;

namespace Serialcoder.VisualLogParser.Runtime
{
	/*
	public class QueryCommand
	{
		private Thread searchThread;
		private EventHandler onSearchComplete;
		bool running;

		public event EventHandler SearchComplete;

		private string _query;

		public string Query
		{
			get { return _query; }
			set { _query = value; }
		}

		private object _inputName;

		public object InputName
		{
			get { return _inputName; }
			set { _inputName = value; }
		}


		public void Start()
		{
			searchThread = new Thread(new ThreadStart(ThreadProcedure));
			running = true;
			searchThread.Start();
		}

		public void StopSearch()
		{
			if (!running)
			{
				return;
			}

			if (searchThread.IsAlive)
			{
				searchThread.Abort();
				searchThread.Join();
			}

			searchThread = null;
			running = false;
		}

		// TODO add cancellable thread
		public QueryResult Execute(string query, object inputFormat)
		{			
			Stopwatch watch = Stopwatch.StartNew();
			int maxUnitsInResults = 1000;	
		
			MSUtil.LogQueryClassClass logQuery = new MSUtil.LogQueryClassClass();
			

			QueryResult queryResult = new QueryResult();
				
			MSUtil.ILogRecordset recordset = logQuery.Execute(query, inputFormat);


			// TODO throw event on query finished

			bool columnsExtracted = false;
			long unitProcessed = 0;

			// Browse the recordset
			for (; !recordset.atEnd() && unitProcessed < maxUnitsInResults; recordset.moveNext())
			{
				MSUtil.ILogRecord record = recordset.getRecord();

				// Remplissage des colonnes
				if (!columnsExtracted)
				{
					for (int c = 0; c < recordset.getColumnCount(); c++)
					{
						DataColumn col = new DataColumn(recordset.getColumnName(c));
						//object logRecord = record.getValueEx(c);
						switch (recordset.getColumnType(c))
						{
							case 1: // int32
								col.DataType = typeof(int);
								break;
							case 2: // double
								col.DataType = typeof(double);
								break;
							case 4: // datetime
								col.DataType = typeof(DateTime);
								break;
							default:
								col.DataType = typeof(string);
								break;
						}

						queryResult.DataTable.Columns.Add(col);
					}
					columnsExtracted = true;
				}

				// on insert les datas
				DataRow dr = queryResult.DataTable.NewRow();
				dr[0] = record.getValue(0).ToString();

				//dataTableResults.ExtendedProperties.Add("source", record.getValue(0).ToString());

				for (int c = 1; c < recordset.getColumnCount(); c++)
				{
					if (record.getValue(c) != DBNull.Value)
					{
						dr[c] = record.getValue(c).ToString();
					}
				}
				//logQuery.inputUnitsProcessed;
				unitProcessed++;
				queryResult.DataTable.Rows.Add(dr);
				// MainForm.StatusBar.Text = string.Format("Computing in progress ... {0} line(s)",unitProcessed);
			}
			// MainForm.StatusBar.Text = string.Format("{0} input units processed",recordset.inputUnitsProcessed);
			recordset.close();


			queryResult.Duration = watch.Elapsed;

			return queryResult;
		}

		private void ThreadProcedure()
		{
			QueryResult results = null;
			// Get the search string. Individual 
			// field assigns are atomic in .NET, so you do not
			// need to use any thread synchronization to grab
			// the string value here.
			try
			{
				string query = _query;

				// Now, search the file system.
				//
				Execute(_query);
			}
			finally
			{
				// You are done with the search, so update.
				//
				running = false;

				// Raise an event that notifies the user that
				// the search has terminated.  
				// You do not have to do this through a marshaled call, but
				// marshaling is recommended for the following reason:
				// Users of this control do not know that it is
				// multithreaded, so they expect its events to 
				// come back on the same thread as the control.
				BeginInvoke(onSearchComplete, new object[] { this, results });
			}
		}
	}
	*/
	public class LogParserInvoker
	{
		public static QueryResult Execute(string query, string inputName, Dictionary<PropertyInfo, object> properties)
		{
			Stopwatch watch = Stopwatch.StartNew();
			int maxUnitsInResults = 5000; // TODO add this to user settings

			MSUtil.LogQueryClassClass logQuery = new MSUtil.LogQueryClassClass();
			
			QueryResult queryResult = new QueryResult();
						
			object inputFormat = SqalpApplication.Inputs[inputName].CreateInstance();
			foreach (PropertyInfo propertyInfo in properties.Keys)
			{
				try
				{
					propertyInfo.SetValue(inputFormat, properties[propertyInfo], null);
				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex);
				}
			}

			
			MSUtil.ILogRecordset recordset = logQuery.Execute(query, inputFormat);
						
			bool columnsExtracted = false;
			long unitProcessed = 0;

			// Browse the recordset
			for (; !recordset.atEnd() && unitProcessed < maxUnitsInResults/* && !cancelComputing.WaitOne(0,false) */; recordset.moveNext())
			{
				MSUtil.ILogRecord record = recordset.getRecord();

				// Remplissage des colonnes
				if (!columnsExtracted)
				{
					for (int c = 0; c < recordset.getColumnCount(); c++)
					{
						DataColumn col = new DataColumn(recordset.getColumnName(c));
						//object logRecord = record.getValueEx(c);
						switch (recordset.getColumnType(c))
						{
							case 1: // int32
								col.DataType = typeof(int);
								break;
							case 2: // double
								col.DataType = typeof(double);
								break;
							case 4: // datetime
								col.DataType = typeof(DateTime);
								break;
							default:
								col.DataType = typeof(string);
								break;
						}

						queryResult.DataTable.Columns.Add(col);
					}
					columnsExtracted = true;
				}

				// on insert les datas
				DataRow dr = queryResult.DataTable.NewRow();
				dr[0] = record.getValue(0).ToString();

				//dataTableResults.ExtendedProperties.Add("source", record.getValue(0).ToString());

				for (int c = 1; c < recordset.getColumnCount(); c++)
				{
					if (record.getValue(c) != DBNull.Value)
					{
						dr[c] = record.getValue(c).ToString();
					}
				}
				//logQuery.inputUnitsProcessed;
				unitProcessed++;
				queryResult.DataTable.Rows.Add(dr);
				// MainForm.StatusBar.Text = string.Format("Computing in progress ... {0} line(s)",unitProcessed);
			}
			// MainForm.StatusBar.Text = string.Format("{0} input units processed",recordset.inputUnitsProcessed);
			recordset.close();


			queryResult.Duration = watch.Elapsed;

			if (logQuery != null)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(logQuery);
				GC.SuppressFinalize(logQuery);
				logQuery = null;
			}		

			return queryResult;
		}
	}
}
