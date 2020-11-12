using System;
using System.Data;

namespace Serialcoder.VisualLogParser.Runtime
{
	/// <summary>
	/// Summary description for QueryResult.
	/// </summary>
	public class QueryResult
	{
		private TimeSpan duration;
		private DataTable dataTable;
		private bool isCancelled;

		//private DateTime start;

		public QueryResult()
		{
			this.dataTable = new DataTable("Results");
			this.isCancelled = false;
		}

		/*
		public QueryResult(DataTable dataTable)
		{
			this.isCancelled = false;
			this.dataTable = dataTable;
		}*/

		public bool IsCancelled
		{
			get {return this.isCancelled;}
			set {this.isCancelled = value;}
		}

		public TimeSpan Duration
		{
			get {return this.duration;}
			set {this.duration = value;}
		}

		public DataTable DataTable
		{
			get {return this.dataTable;}
			set {this.dataTable = value;}
		}

		public void SaveToXml(string filename)
		{
			DataSet ds = new DataSet();
			ds.Tables.Add(this.dataTable);
			ds.WriteXml(filename);
		}
	}
}
