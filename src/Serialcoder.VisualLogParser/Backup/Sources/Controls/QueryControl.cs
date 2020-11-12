using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Utility;
using Serialcoder.VisualLogParser.Runtime;
using Microsoft.Practices.CompositeUI.SmartParts;

namespace Serialcoder.VisualLogParser.Controls
{
	/// <summary>
	/// Description résumée de QueryControl.
	/// </summary>
	[SmartPart]
	public class QueryControl : TD.SandDock.DockControl
	{
		/// <summary> 
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;
		ICSharpCode.TextEditor.TextEditorControl _textEditorControl;

		private bool _isDirty = false;
		private string _filename = string.Empty;

		// Tab contextual menu
		//private ContextMenu uxMenuTab = null;
		//private System.Windows.Forms.MenuItem mnuTabClose;
		
		//private string inputName;
		//private object inputFormat;
		private Query _query;
		//private QueryResult queryResults;
		
		private static int seed = 0;

		[EventPublication(Constants.EventTopics.HelpOnWordRequested, PublicationScope.WorkItem)]
		public event EventHandler<DataEventArgs<string>> HelpOnWordRequested;

		/// <summary>
		/// Creates a new <see cref="QueryControl"/> instance.
		/// </summary>
		internal QueryControl() //string queryText)
		{
			// Cet appel est requis par le Concepteur de formulaires Windows.Forms.
			InitializeComponent();
			
			//
			seed++;
			
			// Contextual menu
			//this.uxMenuTab = new ContextMenu();
			//this.mnuTabClose = new MenuItem();

			//this.uxMenuTab.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			//                                                                        this.mnuTabClose});
			//base.ContextMenu = this.uxMenuTab;

			//this.mnuTabClose.Index = 0;
			//this.mnuTabClose.Text = "&Close";
			//this.mnuTabClose.Click += new System.EventHandler(this.mnuTabClose_Click);
			
			// The textbox
			this._textEditorControl = new ICSharpCode.TextEditor.TextEditorControl();
			_textEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			_textEditorControl.Location = new System.Drawing.Point(0, 0);
			_textEditorControl.Name = "_textEditorControl";
			//_textEditorControl.ShowSpaces = false;
			//_textEditorControl.ShowTabs = false;
			//_textEditorControl.ShowVRuler = true;
			//_textEditorControl.TabIndex = 1;
			//_textEditorControl.ShowMatchingBracket = true;
			//_textEditorControl.ShowEOLMarkers = false;
			_textEditorControl.ShowInvalidLines= false;
			_textEditorControl.UseAntiAliasFont = true;
			//_textEditorControl.ConvertTabsToSpaces = false;
			//_textEditorControl.ShowLineNumbers = true;
			_textEditorControl.Font = new Font(Properties.Settings.Default.EditorFont, FontStyle.Regular);
									
			//ICSharpCode.TextEditor.Document.IHighlightingStrategy highlightingStrategy = ICSharpCode.TextEditor.Document.HighlightingStrategyFactory.CreateHighlightingStrategy("SQL");
			//_textEditorControl.Document.HighlightingStrategy = highlightingStrategy;

			//_textEditorControl.Document.HighlightingStrategy.

			
			//qtx.Text = queryText;
			//this._textEditorControl.Document.TextContentChanged += new EventHandler(Document_TextContentChanged);
			this.Controls.Add(_textEditorControl);

			this.Collapsible = true;
			this.Closable = true;


			string appPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider(appPath));

			_textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy("LOGPARSER");
			
			this._query = new Query();

			_isDirty = false;
			_textEditorControl.ActiveTextAreaControl.TextArea.Document.DocumentChanged += new DocumentEventHandler(Document_DocumentChanged);
		}

		void Document_DocumentChanged(object sender, DocumentEventArgs e)
		{
			this._isDirty = true;
			if (!base.TabText.EndsWith("*"))
			{
				base.Text = base.Text + "*";
			}
		}

		//void qtx_KeyUp(object sender, KeyEventArgs e)
		//{
		//    if (e.KeyCode == Keys.F1)
		//    {
		//        if (HelpOnWordRequested != null)
		//        {
		//            HelpOnWordRequested(this, new DataEventArgs<string>(this.CurrentWord));
		//        }
		//    }
		//}


		/// <summary>
		/// Gets the print document.
		/// </summary>
		/// <value>The print document.</value>
		public System.Drawing.Printing.PrintDocument PrintDocument
		{
			get { return _textEditorControl.PrintDocument; }
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

		#region Code généré par le Concepteur de composants
		/// <summary> 
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion


		#region Public properties

		public string Filename
		{
			get {return this._filename;}
			set {this._filename = value;}
		}
		
		#endregion
		

		public string QueryText
		{
			get
			{
				string queryText = _textEditorControl.ActiveTextAreaControl.SelectionManager.HasSomethingSelected ? _textEditorControl.ActiveTextAreaControl.SelectionManager.SelectedText : _textEditorControl.Text;					
				System.Diagnostics.Debug.WriteLine(queryText);
				return queryText; 
			}
		}

		public Query Query
		{
			get {return this._query;}
		}
	
		
		public bool Save()
		{
			return Save(this._filename.Length == 0);
		}


		public bool Save(bool saveas)
		{
			if (saveas)
			{
				SaveFileDialog diag = new SaveFileDialog();
				diag.Title = "Save query...";
				diag.Filter = "Visual LogParser files(*.vlp)|*.vlp|All files(*.*)|*.*";
				diag.FileName = this._filename;

				if (diag.ShowDialog() == DialogResult.OK)
				{
					this._filename = diag.FileName;					
				}
				else
				{
					return false;
				}
			}

			this._query.Text = this._textEditorControl.Text;

			if (System.IO.Path.GetExtension(_filename) == ".vlp")
			{
				this._query.Save(this._filename);
			}
			else
			{
				// TODO ajouter un export en .bat 
				this._textEditorControl.SaveFile(this._filename);
			}
			
			if (Properties.Settings.Default.RecentFilesList == null)
			{
				Properties.Settings.Default.RecentFilesList = new System.Collections.Specialized.StringCollection();
			}

			if (!Properties.Settings.Default.RecentFilesList.Contains(this._filename))
			{
				Properties.Settings.Default.RecentFilesList.Add(this._filename);
			}
			
			this.Text = this.TabText = System.IO.Path.GetFileName(this._filename);
			this._isDirty = false;
			return true;
		}
		

		public void Open(string filename)
		{
			//this._textEditorControl.Document.TextContentChanged -= new EventHandler(Document_TextContentChanged);
			this._filename = filename;
			if (System.IO.Path.GetExtension(filename) == ".vlp")
			{
				this._query.Load(filename);
				this._textEditorControl.Text = this._query.Text;
			}
			else
			{
				this._textEditorControl.Text = System.IO.File.OpenText(this._filename).ReadToEnd();
			}

			
			this.Text = this.TabText = System.IO.Path.GetFileName(this._filename);
			this.ToolTipText = this._filename;
			this._query.AcceptChanges();
			this._isDirty = false;
			//this._textEditorControl.Document.TextContentChanged += new EventHandler(Document_TextContentChanged);
			_textEditorControl.ActiveTextAreaControl.TextArea.Document.DocumentChanged += new DocumentEventHandler(Document_DocumentChanged);
		
			
			//if (!Properties.Settings.Default.RecentFilesList.Contains(filename))
			//{
			//    Properties.Settings.Default.RecentFilesList.Add(filename);
			//}
		}

		//public new void Close()
		//{
		//    base.Close();

		//    if (this._isDirty)
		//    {
		//        this.Save();
		//    }

			
		//}


		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (_isDirty)
			{
				DialogResult result = MessageBox.Show(string.Format("The query is the {0} file was changed.\r\nDo you want to save the changes?", _filename), "Visual LogParser", MessageBoxButtons.YesNoCancel);
				switch (result)
				{
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
					case DialogResult.No:
						e.Cancel = false;
						break;
					case DialogResult.Yes:
						Save();
						break;
					default:
						break;
				}
			}
		}

		public string CurrentWord
		{
			get
			{
				int pos = this._textEditorControl.ActiveTextAreaControl.Caret.Offset;
				string word = ICSharpCode.TextEditor.Document.TextUtilities.GetWordAt(_textEditorControl.Document, pos);
				if(word.Length==0 && (_textEditorControl.Text.Length > pos-1))
				{
					word = ICSharpCode.TextEditor.Document.TextUtilities.GetWordAt(_textEditorControl.Document, pos-1);
				}
				return word;
			}
		}

		

		// a lancer dans un thread
		/*
		public void ExecuteQuery(object state)
		{
			this.query.Text = this.QueryText;
			this.query.Execute();
		}
		*/
	}
}
