using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.WinForms;
using Serialcoder.VisualLogParser.Runtime;

namespace Serialcoder.VisualLogParser
{
	/// <summary>
	/// This is the Shell of the application.
	/// </summary>
	public class SqalpApplication : FormShellApplication<MainWorkItem, Forms.MainForm>
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{			
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new MainForm());

			//if (IsLogParserInstalled() == false)
			//{
			//    MessageBox.Show("You need to install the free LogParser component first. Please check www.logparser.com");
			//    Application.Exit();
			//}

			new SqalpApplication().Run();
		}

		protected override void BeforeShellCreated()
		{
			base.BeforeShellCreated();			
		}

		public static InputCollection Inputs
		{
			get
			{
				return (InputCollection)System.Configuration.ConfigurationSettings.GetConfig("inputs");
			}
		}

		public static bool IsLogParserInstalled()
		{
			try
			{
				Type comLogQueryType = Type.GetTypeFromProgID("MSUtil.LogQuery", true);
				object comLogQueryObject = Activator.CreateInstance(comLogQueryType);

				//System.Runtime.InteropServices.Marshal.ReleaseComObject(comLogQueryType);
				GC.SuppressFinalize(comLogQueryType);
				comLogQueryType = null;

				return true;
			}
			catch(Exception ex)
			{
				return false;
			}
		}

		#region Unhandled Exception

		public override void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;

			if (ex != null)
			{
				MessageBox.Show(BuildExceptionString(ex));
			}
			else
			{
				MessageBox.Show("An Exception has occured, unable to get details");
			}

			Environment.Exit(0);
		}

		private string BuildExceptionString(Exception exception)
		{
			string errMessage = string.Empty;

			errMessage +=
				exception.Message + Environment.NewLine + exception.StackTrace;

			while (exception.InnerException != null)
			{
				errMessage += BuildInnerExceptionString(exception.InnerException);

				exception = exception.InnerException;
			}

			return errMessage;
		}

		private string BuildInnerExceptionString(Exception innerException)
		{
			string errMessage = string.Empty;

			errMessage += Environment.NewLine + " InnerException ";
			errMessage += Environment.NewLine + innerException.Message + Environment.NewLine + innerException.StackTrace;

			return errMessage;
		}
		#endregion

	}
}