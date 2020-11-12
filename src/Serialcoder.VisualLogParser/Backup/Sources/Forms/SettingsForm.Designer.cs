namespace Serialcoder.VisualLogParser.Forms
{
	partial class SettingsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
			this.uxSettingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// uxSettingsPropertyGrid
			// 
			this.uxSettingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.uxSettingsPropertyGrid.Location = new System.Drawing.Point(0, 0);
			this.uxSettingsPropertyGrid.Name = "uxSettingsPropertyGrid";
			this.uxSettingsPropertyGrid.Size = new System.Drawing.Size(295, 351);
			this.uxSettingsPropertyGrid.TabIndex = 0;
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(295, 351);
			this.Controls.Add(this.uxSettingsPropertyGrid);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.Text = "User settings";
			this.Load += new System.EventHandler(this.SettingsForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid uxSettingsPropertyGrid;
	}
}