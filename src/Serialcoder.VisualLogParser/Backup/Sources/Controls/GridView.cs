using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Serialcoder.VisualLogParser.Controls
{
	/// <summary>
	/// Description résumée de GridView.
	/// </summary>
	public class GridView : System.Windows.Forms.ListView
	{
		/// <summary> 
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem mnuCopyRow;
		private System.Windows.Forms.ToolStripMenuItem mnuCopyCell;
		private System.Windows.Forms.ToolStripMenuItem mnuCopyAllRows;
		private System.Windows.Forms.ToolStripMenuItem mnuSaveResults;
		
		private string currentRowText;
		private string currentCellText;

		public GridView()
		{
			// Cet appel est requis par le Concepteur de formulaires Windows.Forms.
			InitializeComponent();

			// TODO : ajoutez les initialisations après l'appel à InitializeComponent
			this.View = View.Details;
			this.GridLines = true;
			this.FullRowSelect = true;
			this.BorderStyle = BorderStyle.FixedSingle;
			this.HeaderStyle = ColumnHeaderStyle.Clickable;
			this.VirtualMode = true;
			this.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(GridView_RetrieveVirtualItem);
			
			this.currentRowText = string.Empty;
			this.currentCellText = string.Empty;

			this.contextMenu = new System.Windows.Forms.ContextMenuStrip();
			this.mnuCopyCell = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCopyRow = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCopyAllRows = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuSaveResults = new System.Windows.Forms.ToolStripMenuItem();

			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
																								  this.mnuCopyCell,
																								  this.mnuCopyRow,
																								  this.mnuCopyAllRows,
																								  this.mnuSaveResults});
			this.contextMenu.Opened += new System.EventHandler(this.contextMenu_Popup);
			// 
			// mnuCopyCell
			// 
			//this.mnuCopyCell.Index = 0;
			this.mnuCopyCell.Text = "&Copy cell";
			this.mnuCopyCell.Click += new System.EventHandler(this.mnuCopyCell_Click);
			// 
			// mnuCopyRow
			// 
			//this.mnuCopyRow.Index = 1;
			this.mnuCopyRow.Text = "Copy selected &rows";
			this.mnuCopyRow.Click += new System.EventHandler(this.mnuCopyRow_Click);
			// 
			// mnuCopyAllRows
			// 
			//this.mnuCopyAllRows.Index = 2;
			this.mnuCopyAllRows.Text = "Copy &all rows";
			this.mnuCopyAllRows.Click += new System.EventHandler(this.mnuCopyAllRows_Click);
			// 
			// mnuSaveResults
			// 
			//this.mnuSaveResults.Index = 3;
			this.mnuSaveResults.Text = "&Save as...";
			this.mnuSaveResults.Click += new EventHandler(mnuSaveResults_Click);
			
			//this.FindForm().Controls.Add(this.contextMenuForResults);
			this.ContextMenuStrip = this.contextMenu;
		}

		void GridView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			if (_table == null)
				return;

			DataRow row = _table.Rows[e.ItemIndex];

			string[] subItems = new string[_table.Columns.Count];
			for (int i = 0; i < _table.Columns.Count; i++)
			{
				subItems[i] = string.Format("{0}", row[i]);
			}
			e.Item = new ListViewItem(subItems);
		}

		/// <summary> 
		/// Nettoyage des ressources utilisées.
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

		DataTable _table = null;

		public void Fill(DataTable table)
		{
			this.Columns.Clear();
			this.Items.Clear();


			// ajout des headers
			foreach(DataColumn col in table.Columns)
			{
				ColumnHeader header = new ColumnHeader();
				header.Text = col.ColumnName;
				header.Width = 80;
				header.TextAlign = HorizontalAlignment.Left;

				if(col.DataType == typeof(Int32))
				{
					//header.Width = 80;
					header.TextAlign = HorizontalAlignment.Right;
				}
				else if(col.DataType == typeof(DateTime))
				{
					//header.Width = 120;
				}

				this.Columns.Add(header);
				
			}

			// ajout des lignes
			//int maxRow = table.Rows.Count > 500 ? 500 : table.Rows.Count;

			_table = table;

			if (_table.Rows.Count == 0)
			{
				this.VirtualMode = false;
			}
			else
			{
				this.VirtualMode = true;
				this.VirtualListSize = _table.Rows.Count;
				this.EnsureVisible(0);
			}
			//this.BeginUpdate();

			//for(int index=0; index<maxRow; index++)
			//{
			//    DataRow row  = table.Rows[index];

			//    string[] subItems = new string[table.Columns.Count];
			//    for(int i=0; i<table.Columns.Count; i++)
			//    {
			//        subItems[i] = string.Format("{0}", row[i]);
			//    }
			//    ListViewItem item = new ListViewItem(subItems);
			//    this.Items.Add(item);
			//}

			//this.EndUpdate();

			//this.Invalidate();
			this.Visible = true;
		}

		#region Code généré par le Concepteur de composants
		/// <summary> 
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// GridView
			// 
			this.Size = new System.Drawing.Size(216, 192);
			//this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GridView_MouseUp);

		}
		#endregion

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			//e.X
			ListViewItem item = this.GetItemAt(e.X, e.Y);

			if (item == null)
				return;

			// get the current rows as cvs
			this.currentRowText = GetSeletectedRowsAsCsv;

			// get the current cell text
			int currentPos = 0;
			for (int i = 0; i < this.Columns.Count; i++)
			{
				if (e.X >= currentPos && e.X < currentPos + this.Columns[i].Width)
					this.currentCellText = item.SubItems[i].Text;

				currentPos += this.Columns[i].Width;
			}
		}

		//private void GridView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		//{
		//    //e.X
		//    ListViewItem item = this.GetItemAt(e.X, e.Y);

		//    if (item == null)
		//        return;
			
		//    // get the current rows as cvs
		//    this.currentRowText = GetSeletectedRowsAsCsv;
			
		//    // get the current cell text
		//    int currentPos = 0; 
		//    for(int i = 0; i<this.Columns.Count; i++)
		//    {
		//        if (e.X >= currentPos && e.X < currentPos + this.Columns[i].Width)
		//            this.currentCellText = item.SubItems[i].Text;

		//        currentPos += this.Columns[i].Width;
		//    }
		//}

		/// <summary>
		/// Gets the get current row as CSV.
		/// </summary>
		/// <value></value>
		public string GetCurrentRowAsCsv
		{
			get {return this.currentRowText;}
		}

		/// <summary>
		/// Gets the get current cell content.
		/// </summary>
		/// <value></value>
		public string GetCurrentCellContent
		{
			get {return this.currentCellText;}
		}


		/// <summary>
		/// Gets the all rows formated as CSV .
		/// </summary>
		/// <returns></returns>
		public string GetAllRowsAsCsv
		{
			get
			{
				System.Text.StringBuilder stb = new System.Text.StringBuilder();
				for (int i = 0; i < _table.Rows.Count; i++)
				{
					stb.Append(GetRowAsCvs(_table.Rows[i]));
					stb.Append(Environment.NewLine);
				}				
				return stb.ToString();
			}
		}

		public string GetSeletectedRowsAsCsv
		{
			get
			{
				System.Text.StringBuilder stb = new System.Text.StringBuilder();
				foreach (int indice in this.SelectedIndices)
				{
					stb.Append(GetRowAsCvs(_table.Rows[indice]));
					stb.Append(Environment.NewLine);
				}
				//foreach(ListViewItem item in this.SelectedItems)
				//{
				//    stb.Append(GetRowAsCvs(item));
				//    stb.Append(Environment.NewLine);
				//}
				return stb.ToString();
			}
		}

		private string GetRowAsCvs(DataRow row)
		{
			System.Text.StringBuilder stb = new System.Text.StringBuilder();
			for (int i = 0; i < row.Table.Columns.Count; i++)
			{
				stb.Append(row[i]);
				if (i < row.Table.Columns.Count - 1)
					stb.Append(';');
			}
			//stb.Append(Environment.NewLine);
			return stb.ToString();
		}

		//private string GetRowAsCvs(ListViewItem item)
		//{
		//    System.Text.StringBuilder stb = new System.Text.StringBuilder();
		//    for(int i = 0; i<this.Columns.Count; i++)
		//    {
		//        stb.Append(item.SubItems[i].Text);
		//        if (i < this.Columns.Count - 1)
		//            stb.Append(';');
		//    }
		//    //stb.Append(Environment.NewLine);
		//    return stb.ToString();
		//}

		#region Contextual menu events
		
		/// <summary>
		/// When the menu pops up, this function change the text of the menu item to match current cell Contents.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void contextMenu_Popup(object sender, System.EventArgs e)
		{
			if (this.GetCurrentCellContent.Length > 0)
			{
				mnuCopyCell.Text = string.Format("Copy '{0}'", this.GetCurrentCellContent);
				this.mnuCopyCell.Enabled = true;
			}
			else
			{
				mnuCopyCell.Visible = false;
				this.mnuCopyCell.Enabled = false;
			}
		}



		private void mnuCopyRow_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(this.GetCurrentRowAsCsv, true);
		}

		private void mnuCopyCell_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(this.GetCurrentCellContent, true);
		}

		
		private void mnuCopyAllRows_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(this.GetAllRowsAsCsv, true);
		}
		
		private void mnuSaveResults_Click(object sender, EventArgs e)
		{
			SaveFileDialog diag = new SaveFileDialog();
			diag.Title = "Export results to csv file";
			diag.Filter = "CSV files(*.csv)|*.csv|Text files(*.txt)|*.txt|All files(*.*)|*.*";
			if (diag.ShowDialog() == DialogResult.OK)
			{
				System.IO.StreamWriter writer = System.IO.File.CreateText(diag.FileName);
				writer.Write(this.GetAllRowsAsCsv);
				writer.Close();
			}
		}

		#endregion
	}
}
