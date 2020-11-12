using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Diagnostics;

namespace Serialcoder.VisualLogParser.Runtime
{
	/// <summary>
	/// Summary description for Query.
	/// </summary>
	[Serializable]
	public class Query : IDisposable	
	{
		//private InputEntry m_input;
		private string inputName;
		private string text;
		//private Hashtable parameters;
		private int maxUnitsInResults;
		private int rowCount;
		private bool isRunning;
		private bool isDirty;
		private QueryResult results;

		private object inputFormat;

		private MSUtil.LogQueryClassClass logQuery;

		private static volatile object syncRoot = new object();
		
		/// <summary>
		/// Creates a new <see cref="Query"/> instance.
		/// </summary>
		public Query() : this(string.Empty)
		{			
		}

		/// <summary>
		/// Creates a new <see cref="Query"/> instance.
		/// </summary>
		/// <param name="queryText">Query text.</param>
		public Query(string queryText)
		{
			this.text = queryText;
			this.inputName = string.Empty;
			//this.parameters = new System.Collections.Hashtable();
			this.logQuery = new MSUtil.LogQueryClassClass();
			this.rowCount = 0;
			this.maxUnitsInResults = 1000;
			this.isRunning = false;
			this.isDirty = queryText.Length > 0;
		}

		#region Public properties		
		
		[XmlAttribute("inputName")]
		public string InputName
		{
			get {return this.inputName;}
			set
			{
				if (this.inputName != value)
				{
					this.inputName = value;
					this.inputFormat = null;
					this.inputFormat = SqalpApplication.Inputs[this.inputName].CreateInstance();
				}
			}
		}

		[XmlElement("InputFormat")]
		public object InputFormat
		{
			get {return this.inputFormat;}
			//set {this.inputFormat = value;}
		}

		[XmlIgnore]
		public QueryResult Results
		{
			get {return this.results;}
		}

		/// <summary>
		/// Gets the query text.
		/// </summary>
		/// <value></value>
		[XmlElement("text")]
		public string Text
		{
			get { return text; }
			set
			{
				if (this.text != value)
				{
					this.text = value;
					this.isDirty = true;
				}
			}
		}
				
		[XmlIgnore]
		public int MaxUnitsInResults
		{
			get { return this.maxUnitsInResults;}
		}

		[XmlIgnore]
		public int RowCount
		{
			get { return this.rowCount;}
			set { this.rowCount = value;}
		}

		[XmlIgnore]
		public bool IsDirty
		{
			get { return this.isDirty;}
		}

		#endregion

		public void AcceptChanges()
		{
			this.isDirty = false;
		}


		// TODO add cancellable thread
		[Obsolete("removed, use QueryInvoker instead", true)]
		public QueryResult Execute()
		{
			lock(syncRoot)
			{
				this.isRunning = true;
			}
			Stopwatch watch = Stopwatch.StartNew();

			//if (this.logQuery != null)
			//{
			//    System.Runtime.InteropServices.Marshal.ReleaseComObject(this.logQuery);
			//    GC.SuppressFinalize(this.logQuery);
			//    this.logQuery = null;
			//}
			
			//this.logQuery = new MSUtil.LogQueryClassClass();

			this.results = null;
			QueryResult queryResult = new QueryResult();
					

			//MSUtil.COMChartOutputContextClassClass outPutFormat = new MSUtil.COMChartOutputContextClassClass();
			//outPutFormat.chartType = "PieExploded3D";
			//outPutFormat.view = true;
						
//			MSUtil.COMXMLOutputContextClassClass outPutFormat = new MSUtil.COMChartOutputContextClassClass();
//			this.logQuery.ExecuteBatch(Text, this.inputFormat, outPutFormat);
//			return null;

			
			MSUtil.ILogRecordset recordset = this.logQuery.Execute(Text, this.inputFormat);
			
			
			// TODO throw event on query finished

			bool columnsExtracted = false;
			long unitProcessed = 0;
			
			// Browse the recordset
			for(; !recordset.atEnd() && unitProcessed < this.maxUnitsInResults/* && !cancelComputing.WaitOne(0,false) */; recordset.moveNext())
			{
				MSUtil.ILogRecord record = recordset.getRecord();

				// Remplissage des colonnes
				if (!columnsExtracted)
				{
					for (int c = 0; c < recordset.getColumnCount(); c++)
					{
						DataColumn col = new DataColumn(recordset.getColumnName(c));
						//object logRecord = record.getValueEx(c);
						switch(recordset.getColumnType(c))
						{
							case 1 : // int32
								col.DataType = typeof(int);
								break;
							case 2 : // double
								col.DataType = typeof(double);
								break;
							case 4 : // datetime
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

			lock(syncRoot)
			{
				this.isRunning = false;
			}
			this.results = queryResult;
			return this.results;			
		}

		#region IDisposable Members
		
		public void Dispose()
		{
			Dispose(true);
		}

		public virtual void Dispose(bool disposing)
		{
			if (this.logQuery != null)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(this.logQuery);
				GC.SuppressFinalize(this.logQuery);
				this.logQuery = null;
			}			
		}

		#endregion
	
		/// <summary>
		/// Serializes the entity to xml and puts the data into a file.
		/// </summary>
		/// <param name="entity">The Entity to serialize.</param>
		/// <param name="path">The Path to the destination file.</param>
		public void SerializeXml(string path)
		{
			//XmlSerializer ser = new XmlSerializer(typeof(Query));
			XmlSerializer ser = new XmlSerializer(typeof(MSUtil.COMIISW3CInputContextClassClass));
			StreamWriter sw = new StreamWriter(path);
			ser.Serialize(sw, this.inputFormat);
			sw.Close();
		}

		public void Save(string fileName)
		{
			XmlTextWriter writer = new XmlTextWriter(fileName, System.Text.Encoding.ASCII);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument(true);
			writer.WriteStartElement("query");
			writer.WriteElementString("inputName", this.inputName );
			writer.WriteElementString("text", this.text);

			writer.WriteStartElement("inputFormat");
			Type t = this.inputFormat.GetType();
			foreach (PropertyInfo property in t.GetProperties())
			{
				if (property.CanRead)
				{
					try
					{
						writer.WriteElementString(property.Name, string.Format("{0}", property.GetValue(this.inputFormat, null)));
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex);
					}
				}
			}
			writer.WriteEndElement();
			

			writer.WriteEndElement();
			writer.Flush();
			writer.Close();
		}

		public bool Load(string fileName)
		{
			XmlDocument xmlDoc = new XmlDocument();

			try
			{
				xmlDoc.Load(fileName);
				this.InputName = xmlDoc.SelectSingleNode("//inputName").InnerText;
				this.text = xmlDoc.SelectSingleNode("//text").InnerText;

				Type t = this.inputFormat.GetType();
				XmlNode inputNode = xmlDoc.SelectSingleNode("/query/inputFormat");
				foreach(XmlNode node in inputNode.ChildNodes)
				{
					PropertyInfo propertyInfo = t.GetProperty(node.Name,System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public );
					if(propertyInfo == null)
					{
						throw new System.ArgumentException(node.Name + " parameter does not exists for inputFormat " + this.inputName);
					}

					// read the actual (default) value
					object _defaultValue = propertyInfo.GetValue(inputFormat, null);
					
					//  read actual value and update only if different
					System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(propertyInfo.PropertyType);
					object _value = typeConverter.ConvertFromString(node.InnerText);

					if (!_defaultValue.Equals(_value))
					{
						propertyInfo.SetValue(inputFormat, _value, null);
					}
				}

				xmlDoc = null;
			}
			catch(XmlException xex)
			{
				System.Diagnostics.Debug.Write(xex);
				return false;
			}
//			catch(System.Reflection.TargetInvocationException ex)
//			{
//				System.Diagnostics.Debug.Write(ex);
//				return false;
//			}

			return true;

			/*

			XmlTextReader reader = new XmlTextReader(fileName);
			reader.Read();
			reader.ReadStartElement("query");
			this.InputName = reader.ReadElementString("inputName");
			Type t = this.inputFormat.GetType();
			
			this.text = reader.ReadElementString("text");
			
			reader.ReadStartElement("inputFormat");
						
			while (reader.Read())
			{				
				if (reader.NodeType != XmlNodeType.Element)
					continue;
				
				//PropertyInfo property = t.GetProperty(reader.Name);

				PropertyInfo propertyInfo = t.GetProperty(reader.Name,System.Reflection.BindingFlags.IgnoreCase 
					| System.Reflection.BindingFlags.Instance 
					| System.Reflection.BindingFlags.Public );
				if(propertyInfo == null)
				{
					throw new System.ArgumentException(reader.Name + " parameter does not exists for inputFormat " + this.inputName);
				}
				System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(propertyInfo.PropertyType);
				object _value = typeConverter.ConvertFromString(reader.ReadInnerXml());
				propertyInfo.SetValue(inputFormat, _value, null);
				//property.SetValue(this.inputFormat, reader.Value, null);
			}
			
			reader.Close();		
			*/	
		}
	}
}
