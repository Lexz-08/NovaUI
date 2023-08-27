namespace Demo
{
	partial class Main
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
			this.novaFileInput1 = new NovaUI.Controls.NovaFileInput();
			this.SuspendLayout();
			// 
			// novaFileInput1
			// 
			this.novaFileInput1.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(107)))), ((int)(((byte)(127)))));
			this.novaFileInput1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
			this.novaFileInput1.BorderRadius = 6;
			this.novaFileInput1.BorderWidth = 1;
			this.novaFileInput1.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
			this.novaFileInput1.DialogFilter = "All Files|*.*";
			this.novaFileInput1.DialogTitle = "Please select a file to open...";
			this.novaFileInput1.FileInputType = NovaUI.Enums.FileInputType.OpenFile;
			this.novaFileInput1.FilterIndex = 1;
			this.novaFileInput1.Location = new System.Drawing.Point(153, 123);
			this.novaFileInput1.Name = "novaFileInput1";
			this.novaFileInput1.ReadOnly = true;
			this.novaFileInput1.Size = new System.Drawing.Size(200, 32);
			this.novaFileInput1.TabIndex = 0;
			this.novaFileInput1.Text = "novaFileInput1";
			this.novaFileInput1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			this.novaFileInput1.UnderlineBorder = false;
			this.novaFileInput1.UseUserSchemeCursor = true;
			// 
			// Main
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
			this.ClientSize = new System.Drawing.Size(496, 380);
			this.Controls.Add(this.novaFileInput1);
			this.Font = new System.Drawing.Font("Source Sans Pro SemiBold", 10F);
			this.MaximizeBox = false;
			this.Name = "Main";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Demo";
			this.UseAeroShadow = false;
			this.ResumeLayout(false);

		}

		#endregion

		private NovaUI.Controls.NovaFileInput novaFileInput1;
	}
}

