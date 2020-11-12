using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Serialcoder.VisualLogParser.Controls
{
	/// <summary>
	/// Summary description for QueryContainer.
	/// </summary>
	public class QueryContainer : TD.SandDock.DocumentContainer	
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public event CancelEventHandler QueryClosing;
		public event CancelEventHandler QueryClosed;

		public QueryContainer() : base()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Events trigger

		/// <summary>
		/// Raises the <see cref="QueryClosing" /> event.
		/// </summary>
		/// <param name="column">The <see cref="UserColumns"/> which has fired the event.</param>
		public void OnQueryClosing(QueryControl qc, CancelEventArgs e)
		{
			if(QueryClosing != null)
			{
				QueryClosing(qc, e);
			}	
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		public QueryControl[] Queries
		{
			get
			{
				QueryControl[] arr = new QueryControl[base.Documents.Length];
				for(int i=0; i<base.Documents.Length; i++)
				{
					arr[i] = base.Documents[i] as QueryControl;
				}
				return arr;
			}
		}

		public string CurrentWord
		{
			get
			{
				if (this.Queries.Length > 0)
				{
					return this.ActiveQuery.CurrentWord;
				}
				else
				{
					return string.Empty;
				}
			}
		}

		public QueryControl ActiveQuery
		{
			get {return base.ActiveDocument as QueryControl;}
		}


		/// <summary>
		/// Adds the queryControl to this container.
		/// </summary>
		/// <remarks>Raise an event when a attempt is made to close this control</remarks>
		/// <param name="c">C.</param>
		private void AddQuery(QueryControl c)
		{
			c.Closing += new CancelEventHandler(qc_Closing);
			base.AddDocument(c);
		}

		[Obsolete("Use typed method instead: ActiveQuery")]
		public new TD.SandDock.DockControl ActiveDocument
		{
			get
			{
				return base.ActiveDocument;
			}
		}

		[Obsolete("Use typed method instead : AddQuery")]
		public new void AddDocument(TD.SandDock.DockControl c)
		{
			base.AddDocument(c);
		}


		/// <summary>
		/// Creates a new QueryControl and add it to the documents collection.
		/// </summary>
		/// <returns></returns>
		public QueryControl CreateQueryControl()
		{
			QueryControl qc = new QueryControl();
			this.AddQuery(qc);
			
			qc.Name = string.Format("{0}", this.Queries.Length); //this.Text = string.Format("query - input name - untitled {0}", QueryControl.seed);
			qc.Text = string.Format("Untitled {0}", this.Queries.Length);
			//qc.Query.InputName = inputName;
			qc.Activate();
			return qc;
		}
		
		private void qc_Closing(object sender, CancelEventArgs e)
		{
			// forward this event to the QueryContainer.OnQueryClosing event
			OnQueryClosing(sender as QueryControl, e);		
		}

		public QueryControl GetByFilename(string filename)
		{
			foreach (QueryControl queryControl in Queries)
			{
				if (queryControl.Filename.Equals(filename))
				{
					return queryControl;
				}
			}
			return null;
		}
	}
}
