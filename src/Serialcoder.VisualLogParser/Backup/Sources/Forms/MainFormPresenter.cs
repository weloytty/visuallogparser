using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI;
using System.Windows.Forms;
using System.IO;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Utility;

namespace Serialcoder.VisualLogParser.Forms
{
	public class MainFormPresenter : Controller
	{
		MainForm _view;
		string _helpFilePath = Application.StartupPath + "\\LogParser.chm";

		/// <summary>
		/// Gets or sets the view.
		/// </summary>
		/// <value>The view.</value>
		public MainForm View
		{
			get { return _view; }
			set { _view = value; }
		}

		internal void OpenHelp()
		{
			if (!File.Exists(this._helpFilePath))
			{
				MessageBox.Show("Helpfile (LogParser.chm) could not be found.", "Sqalp", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			Help.ShowHelp(_view, _helpFilePath, HelpNavigator.TableOfContents);
		}

		internal void OpenHelp(string helpToken)
		{
			if (!File.Exists(this._helpFilePath))
			{
				MessageBox.Show("Helpfile (LogParser.chm) could not be found.", "Sqalp", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			Help.ShowHelp(_view, _helpFilePath, HelpNavigator.KeywordIndex, helpToken);
		}

		internal void OpenHelpSearch()
		{
			if (!File.Exists(this._helpFilePath))
			{
				MessageBox.Show("Helpfile (LogParser.chm) could not be found.", "Sqalp", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			Help.ShowHelp(_view, _helpFilePath, HelpNavigator.Find);
		}

		/// <summary>
		/// Opens the index of the help.
		/// </summary>
		internal void OpenHelpIndex()
		{
			if (!File.Exists(this._helpFilePath))
			{
				MessageBox.Show("Helpfile (LogParser.chm) could not be found.", "Sqalp", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			Help.ShowHelp(_view, _helpFilePath, HelpNavigator.Index);
		}

		[EventSubscription(Constants.EventTopics.HelpOnWordRequested)]
		public void HelpOnWordRequested(object sender, DataEventArgs<string> args)
		{
			OpenHelp(args.Data);
		}
	}
}
