using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.ObjectBuilder;
using Serialcoder.VisualLogParser.Runtime;
using System.Drawing.Printing;
using System.Reflection;

namespace Serialcoder.VisualLogParser.Forms
{
	public partial class MainForm : Form
	{
		private PrinterSettings _printerSettings = new PrinterSettings();
		private PageSettings _pageSettings = new PageSettings();

		Serialcoder.Windows.Forms.EditMenuManager _editMenuManager;
		    
		#region Presenter injection

		/// <summary>
		/// The controller will get injected into the smartpart
		/// when it is added to the workitem.
		/// </summary>
		private MainFormPresenter _presenter = null;

		[CreateNew]
		public MainFormPresenter Presenter
		{
			set { this._presenter = value; this._presenter.View = this; }
			get { return this._presenter; }
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
			
			// Register the .vlp file extension with the application
			RegisterFileType();	

			_editMenuManager = new Serialcoder.Windows.Forms.EditMenuManager();
			_editMenuManager.ConnectMenus(this.editMenu);

			uxQueryContainer.ActiveDocumentChanged += new TD.SandDock.ActiveDocumentEventHandler(uxQueryContainer_ActiveDocumentChanged);
		}
			

		#region Form override

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			// we open an empty query designer at startup
			this.uxQueryContainer.CreateQueryControl();
			
						
			// Fill and bind the inputs list
			uxInputToolStripComboBox.ComboBox.DisplayMember = "Name";
			uxInputToolStripComboBox.ComboBox.ValueMember = "Name";
			uxInputToolStripComboBox.ComboBox.DataSource = SqalpApplication.Inputs;
			
			uxInputToolStripComboBox.ComboBox.SelectedIndexChanged += new EventHandler(OnComboBoxSelectedIndexChanged);
			//uxInputToolStripComboBox.ComboBox.SelectedValue = Properties.Settings.Default.InputName;

			uxInputToolStripComboBox.ComboBox.DataBindings.Add("SelectedValue", Serialcoder.VisualLogParser.Properties.Settings.Default, "InputName", true, DataSourceUpdateMode.OnPropertyChanged);


			rightSandDock.Visible = Properties.Settings.Default.ShowQueryHelp;
						
			FillRecentFilesMenuItem();
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			Serialcoder.VisualLogParser.Properties.Settings.Default.Save();
		}

		#endregion

		#region Event handlers

		private void OnNewQueryClick(object sender, EventArgs e)
		{
			this.uxQueryContainer.CreateQueryControl();
			
			//Properties.Settings.Default.InputName = uxInputToolStripComboBox.ComboBox.SelectedValue.ToString();
			uxQueryContainer.ActiveQuery.Query.InputName = Properties.Settings.Default.InputName;
			uxInputFormatPropertyGrid.SelectedObject = uxQueryContainer.ActiveQuery.Query.InputFormat;
		}

		private void OnOpenQueryClick(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Sqalp files (*.vlp)|*.vlp|Query files (*.sql)|*.sql|all files (*.*)|*.*";
			ofd.Title = "Open query...";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				OpenFile(ofd.FileName);

				// add to recent files
				if (!Properties.Settings.Default.RecentFilesList.Contains(ofd.FileName))
				{
					Properties.Settings.Default.RecentFilesList.Add(ofd.FileName);
				}
			}
		}

		private void OnSaveClick(object sender, EventArgs e)
		{
			if (this.uxQueryContainer.ActiveQuery != null)
			{
				this.uxQueryContainer.ActiveQuery.Save();
				UpdateFormTile();
			}
		}

		private void OnSaveAsQueryClick(object sender, EventArgs e)
		{
			if (this.uxQueryContainer.ActiveQuery != null)
			{
				this.uxQueryContainer.ActiveQuery.Save(true);
				UpdateFormTile();
			}
		}

		void OnPrintSetupClick(object sender, EventArgs e)
		{
			PageSetupDialog pageSetupDialog = new PageSetupDialog();
			pageSetupDialog.PageSettings = _pageSettings;
			pageSetupDialog.PrinterSettings = _printerSettings;
			pageSetupDialog.AllowOrientation = true;
			pageSetupDialog.AllowMargins = true;
			pageSetupDialog.ShowDialog();

		}

		void OnPrintPreviewClick(object sender, EventArgs e)
		{
			if (uxQueryContainer.Queries.Length > 0)
			{
				PrintPreviewDialog dlg = new PrintPreviewDialog();
				dlg.Document = uxQueryContainer.ActiveQuery.PrintDocument; ;
				dlg.ShowDialog();
			}
		}

		void OnPrintClick(object sender, EventArgs e)
		{
			if (uxQueryContainer.Queries.Length > 0)
			{
				uxQueryContainer.ActiveQuery.PrintDocument.DefaultPageSettings = _pageSettings;
				
				PrintDialog dlg = new PrintDialog();
				dlg.Document = uxQueryContainer.ActiveQuery.PrintDocument;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					uxQueryContainer.ActiveQuery.PrintDocument.Print();
				}
			}
		}			

		private void OnExitClick(object sender, EventArgs e)
		{
			Application.Exit();
		}			

		/// <summary>
		/// Handles the Click event of the StatusBarToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
		{
			statusStrip.Visible = statusBarToolStripMenuItem.Checked;
		}
		
		/// <summary>
		/// Handles the Click event of the aboutToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void OnAboutClick(object sender, EventArgs e)
		{
			new AboutBoxForm().ShowDialog();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the ComboBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		void OnComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (uxInputToolStripComboBox.ComboBox.SelectedValue == null)
			{
				return;
			}

			//Properties.Settings.Default.InputName = uxInputToolStripComboBox.ComboBox.SelectedValue.ToString();

			string baseUrl;

			if (Properties.Settings.Default.InputName == "EVT" || Properties.Settings.Default.InputName == "IISW3C" || Properties.Settings.Default.InputName == "FS" || Properties.Settings.Default.InputName == "NETMON")
			{
				baseUrl = string.Format(@"mk:@MSITStore:{0}\LogParser.chm::/html/{1}", Application.StartupPath, Properties.Settings.Default.InputName);
			}
			else
			{
				baseUrl = string.Format(@"mk:@MSITStore:{0}\LogParser.chm::/html/{1}I", Application.StartupPath, Properties.Settings.Default.InputName);
			}
			string url1 = baseUrl + "_FromEntity.htm";
			string url2 = baseUrl + "_Fields.htm";
			string url3 = baseUrl + "_Parameters.htm";
			string url4 = baseUrl + "_Examples.htm";
			axWebBrowser1.Navigate(url1);
			axWebBrowser2.Navigate(url2);
			axWebBrowser3.Navigate(url3);
			axWebBrowser4.Navigate(url4);
			
			// update de l'inputFormat
			dckParameters.Text = string.Format("{0} parameters", Properties.Settings.Default.InputName);

			// TODO warn is current query is not saved
			if (uxQueryContainer.ActiveQuery != null)
			{
				uxQueryContainer.ActiveQuery.Query.InputName = Properties.Settings.Default.InputName;
				uxInputFormatPropertyGrid.SelectedObject = uxQueryContainer.ActiveQuery.Query.InputFormat;
			}
		}

		void uxRecentFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			OpenFile(item.Tag.ToString());
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new SettingsForm().ShowDialog();
		}
		
		private void queryHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			rightSandDock.Visible = queryHelpToolStripMenuItem.Checked;
		}
		
		private void resultsPaneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (resultsPaneToolStripMenuItem.Checked)
			{
				dckResultsGrid.Open(TD.SandDock.DockLocation.Bottom);
			}
			else
			{
				dckResultsGrid.Close();
			}
		}

		private void messagePaneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (resultsPaneToolStripMenuItem.Checked)
			{
				dckOutput.Open(TD.SandDock.DockLocation.Bottom);
			}
			else
			{
				dckOutput.Close();
			}			
		}

		private void indexToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.OpenHelpIndex();
		}

		private void searchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.OpenHelpSearch();
		}

		private void helpToolStripButton_Click(object sender, EventArgs e)
		{
			Presenter.OpenHelp();
		}

		private void uxExecuteQueryToolStripButton_Click(object sender, EventArgs e)
		{
			if (this.uxQueryContainer.ActiveQuery == null) return;

			if (this.uxQueryContainer.ActiveQuery.Query.InputFormat == null)
			{
				MessageBox.Show("You must select an input format before executing a query.", Properties.Settings.Default.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			this.toolStripStatusLabel.Text = "Executing query batch...";
			//Cursor.Current = Cursors.WaitCursor;
			uxExecuteQueryToolStripButton.Enabled = uxExecuteToolStripMenuItem.Enabled = false;
			uxStopQueryToolStripButton.Enabled = uxStopToolStripMenuItem.Enabled = true;
			toolStripStatusLabel1.Visible = true;
			txbOutput.Clear();

			Dictionary<PropertyInfo, object> properties = new Dictionary<PropertyInfo, object>();

			foreach (PropertyInfo property in this.uxQueryContainer.ActiveQuery.Query.InputFormat.GetType().GetProperties())
			{
				if (property.CanWrite)
				{
					try
					{
						object propertyValue = null;
						propertyValue = property.GetValue(this.uxQueryContainer.ActiveQuery.Query.InputFormat, null);
						properties.Add(property, propertyValue);
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex);
					}					
				}
			}

			backgroundWorker.RunWorkerAsync(new BGParam(this.uxQueryContainer.ActiveQuery.QueryText, this.uxQueryContainer.ActiveQuery.Query.InputName , properties));
			
#if SYNCRUN
			/*
			QueryResult results = null;

			try
			{
				results = LogParserInvoker.Execute(this.uxQueryContainer.ActiveQuery.QueryText, this.uxQueryContainer.ActiveQuery.Query.InputFormat);

				txbOutput.Clear();
				txbOutput.Text += string.Format("({0} rows affected){1}{1}", results.DataTable.Rows.Count, Environment.NewLine);
				txbOutput.Text += string.Format("Query time : {0}{1}{1}", results.Duration, Environment.NewLine);

				DateTime start = DateTime.Now;
				gridView.Fill(results.DataTable);
				txbOutput.Text += string.Format("Display time : {0}{1}{1}", DateTime.Now.Subtract(start), Environment.NewLine);

				dckResultsGrid.Activate();
				this.toolStripStatusLabel.Text = "Query batch completed.";
			}			
			catch (Exception ex)
			{
				txbOutput.Text = ex.Message;
				txbOutput.SelectionStart = txbOutput.SelectionLength = 0;
				dckOutput.Activate();
				this.toolStripStatusLabel.Text = "Query batch completed with errors.";
			}

			//Cursor.Current = Cursors.Default;
			uxExecuteQueryToolStripButton.Enabled = uxExecuteToolStripMenuItem.Enabled = true;
			uxStopQueryToolStripButton.Enabled = uxStopToolStripMenuItem.Enabled = false;
			*/
#endif
		}

		class BGParam
		{
			public string QueryText;
			//public Object InputFormat;
			public Dictionary<PropertyInfo, object> Properties;
			public string InputName;

			public BGParam() { }
			public BGParam(string query, string inputName, Dictionary<PropertyInfo, object> properties)
			{
				QueryText = query;
				Properties = properties;
				InputName = inputName;
			}
		}

		private void uxStopQueryToolStripButton_Click(object sender, EventArgs e)
		{
			backgroundWorker.StopImmediately();

			this.toolStripStatusLabel.Text = "Query batch cancelled by user.";
			this.gridView.Items.Clear();
			this.dckOutput.Activate();
			txbOutput.Clear();
			txbOutput.Text += string.Format("(Query batch cancelled by user.){0}{0}", Environment.NewLine);

			//Cursor.Current = Cursors.Default;
			uxExecuteQueryToolStripButton.Enabled = uxExecuteToolStripMenuItem.Enabled = true;
			uxStopQueryToolStripButton.Enabled = uxStopToolStripMenuItem.Enabled = false;
			this.toolStripStatusLabel1.Visible = false;
		}

		private void logParsercomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.logparser.com");
		}

		private void fileMenu_DropDownOpened(object sender, EventArgs e)
		{
			if (uxQueryContainer.ActiveQuery == null)
			{
				saveAsToolStripMenuItem.Enabled = false;
				saveToolStripMenuItem.Enabled = false;
				printPreviewToolStripMenuItem.Enabled = false;
				printToolStripMenuItem.Enabled = false;
				closeAllToolStripMenuItem.Enabled = false;
				closeToolStripMenuItem.Enabled = false;
			}
			else
			{
				saveAsToolStripMenuItem.Enabled = true;
				saveToolStripMenuItem.Enabled = true;
				printPreviewToolStripMenuItem.Enabled = true;
				printToolStripMenuItem.Enabled = true;
				closeAllToolStripMenuItem.Enabled = true;
				closeToolStripMenuItem.Enabled = true;
			}
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (uxQueryContainer.ActiveQuery != null)
			{
				uxQueryContainer.ActiveQuery.Close();
			}
		}

		private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = uxQueryContainer.Queries.Length - 1; i >= 0; i--)
			{
				uxQueryContainer.Queries[i].Close();
			}
		}

		void uxQueryContainer_ActiveDocumentChanged(object sender, TD.SandDock.ActiveDocumentEventArgs e)
		{
			if (e.NewActiveDocument == null)
			{
				SelectDefaultInput();
				this.uxInputFormatPropertyGrid.SelectedObject = null;
				dckParameters.Text = "No query";				
			}
			else
			{				
				// Set current input as default input
				Properties.Settings.Default.InputName = (e.NewActiveDocument as Serialcoder.VisualLogParser.Controls.QueryControl).Query.InputName;
				SelectDefaultInput();

				this.uxInputFormatPropertyGrid.SelectedObject = null;
				this.uxInputFormatPropertyGrid.SelectedObject = (e.NewActiveDocument as Serialcoder.VisualLogParser.Controls.QueryControl).Query.InputFormat;

				// update de l'inputFormat
				dckParameters.Text = string.Format("{0} parameters", Properties.Settings.Default.InputName);
			}
		}

		private void SelectDefaultInput()
		{
			foreach (Serialcoder.VisualLogParser.Runtime.Input input in this.uxInputToolStripComboBox.Items)
			{
				if (input.Name == Properties.Settings.Default.InputName)
				{
					uxInputToolStripComboBox.SelectedItem = input;
				}
			}
		}

		#endregion

		#region Private methods
	
		void OpenFile(string filename)
		{
			Serialcoder.VisualLogParser.Controls.QueryControl ctrl = uxQueryContainer.GetByFilename(filename); 
			if (ctrl != null)
			{
				ctrl.Activate();
				return;
				//this.uxQueryContainer.ActiveQuery = ctrl;
			}

			this.uxQueryContainer.CreateQueryControl();
			this.uxQueryContainer.ActiveQuery.Open(filename);
			
			foreach (Serialcoder.VisualLogParser.Runtime.Input input in this.uxInputToolStripComboBox.Items)
			{
				if (input.Name == uxQueryContainer.ActiveQuery.Query.InputName)
				{
					uxInputToolStripComboBox.SelectedItem = input;
				}
			}
			this.uxInputFormatPropertyGrid.SelectedObject = uxQueryContainer.ActiveQuery.Query.InputFormat;
			
			// update de l'inputFormat
			dckParameters.Text = string.Format("{0} parameters", Properties.Settings.Default.InputName);

			#region Recent files management

			if (Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList != null && Properties.Settings.Default.RecentFilesList.Contains(filename))
			{
				Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList.Remove(filename);
			}

			if (Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList == null)
			{
				Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList = new System.Collections.Specialized.StringCollection();
			}

			// we keep only 
			if (Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList.Count == 5)
			{
				Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList.RemoveAt(0);
			}

			Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList.Add(filename);

			Serialcoder.VisualLogParser.Properties.Settings.Default.Save();
			FillRecentFilesMenuItem();

			#endregion
		}

		private void FillRecentFilesMenuItem()
		{
			if (Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList == null)
				return;

			uxRecentFilesToolStripMenuItem.DropDownItems.Clear();
			for (int i = Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList.Count - 1; i >= 0; i--)
			{
				ToolStripMenuItem recentFilesItemToolStripMenuItem = new ToolStripMenuItem(Serialcoder.VisualLogParser.Properties.Settings.Default.RecentFilesList[i]);
				recentFilesItemToolStripMenuItem.Click += new EventHandler(recentFilesItemToolStripMenuItem_Click);
				uxRecentFilesToolStripMenuItem.DropDownItems.Add(recentFilesItemToolStripMenuItem);
			}
		}

		void recentFilesItemToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem recentFilesItemToolStripMenuItem = sender as ToolStripMenuItem;
			OpenFile(recentFilesItemToolStripMenuItem.Text);
		}		

		//void OpenHelp()
		//{
		//    if (this._helpFilePath.Length == 0)
		//    {
		//        MessageBox.Show("Helpfile (LogParser.chm) could not be found.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		//        return;
		//    }
		//    HelpNavigator navigator = HelpNavigator.TableOfContents;
		//    string helpToken = this.uxQueryContainer.CurrentWord;
		//    Help.ShowHelp(this, _helpFilePath, navigator, helpToken);
		//}

		void UpdateFormTile()
		{
			if (this.uxQueryContainer.Queries.Length > 0 && Properties.Settings.Default.ShowFullPathInTitle)
			{
				this.Text = string.Format(Properties.Settings.Default.ApplicationName + " - {0}", this.uxQueryContainer.ActiveQuery.Filename.Length > 0 ? this.uxQueryContainer.ActiveQuery.Filename : this.uxQueryContainer.ActiveQuery.Text);
			}
			else
			{
				this.Text = string.Format(Properties.Settings.Default.ApplicationName + " - {0}", typeof(SqalpApplication).Assembly.GetName().Version);
			}
		}

		/// <summary>
		/// Registers the type of the file.
		/// </summary>
		void RegisterFileType()
		{
			Serialcoder.Win32.FileAssociation FA = new Serialcoder.Win32.FileAssociation();
			FA.Extension = "vlp";
			FA.ContentType = "application/Visual LogParser";
			FA.FullName = "Visual LogParser query files";
			FA.ProperName = "Visual LogParser query";
			FA.IconPath = Application.ExecutablePath;
			FA.IconIndex = 0;
			FA.AddCommand("Open", Application.ExecutablePath + " \"%1\"");
			FA.Create();
		}  

		#endregion
				
		#region BackgroundWorker

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BGParam param = e.Argument as BGParam;
			e.Result =  LogParserInvoker.Execute(param.QueryText, param.InputName, param.Properties);
		}		

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				if (e.Error is Exception)
				{
					if (e.Error.InnerException != null)
					{
						txbOutput.Text = e.Error.InnerException.Message;
					}
					else
					{
						txbOutput.Text = e.Error.Message;
					}
					
					txbOutput.SelectionStart = txbOutput.SelectionLength = 0;
					dckOutput.Activate();
					this.toolStripStatusLabel.Text = "Query batch completed with errors.";
				}
				else
				{
					txbOutput.Text = "Unexpected result type: " + e.Error.GetType().FullName;
					dckOutput.Activate();
					this.toolStripStatusLabel.Text = "Query batch completed with errors.";
				}
			}
			else if (e.Cancelled || backgroundWorker.CancellationPending)
			{
				this.toolStripStatusLabel.Text = "Query batch cancelled by user.";
			}
			else if (e.Result is QueryResult)
			{				
				QueryResult results = (QueryResult)e.Result;

				txbOutput.Clear();
				txbOutput.Text += string.Format("({0} rows affected){1}{1}", results.DataTable.Rows.Count, Environment.NewLine);
				txbOutput.Text += string.Format("Query time : {0}{1}{1}", results.Duration, Environment.NewLine);

				DateTime start = DateTime.Now;
				System.Diagnostics.Stopwatch displayStopwatch = System.Diagnostics.Stopwatch.StartNew();
				gridView.Fill(results.DataTable);
				txbOutput.Text += string.Format("Display time : {0}ms{1}{1}", displayStopwatch.ElapsedMilliseconds, Environment.NewLine);

				dckResultsGrid.Activate();
				this.toolStripStatusLabel.Text = "Query batch completed.";
			}

			//Cursor.Current = Cursors.Default;
			uxExecuteQueryToolStripButton.Enabled = uxExecuteToolStripMenuItem.Enabled = true;
			uxStopQueryToolStripButton.Enabled = uxStopToolStripMenuItem.Enabled = false;
		}

		#endregion							
	}
}
