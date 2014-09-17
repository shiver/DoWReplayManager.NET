using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DowReplayManager.NET
{
	/// <summary>
	/// Summary description for frmInput.
	/// </summary>
	public class frmInput : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblQuestion;
		private System.Windows.Forms.TextBox txtInput;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string Question
		{
			get
			{
				return lblQuestion.Text;
			}
			set
			{
				lblQuestion.Text = value;
			}
		}

		public string Input
		{
			get
			{
				return txtInput.Text;
			}
			set
			{
				txtInput.Text = value;
			}
		}

		public string Caption
		{
			get
			{
				return this.Text;
			}
			set
			{
				this.Text = value;
			}
		}

		public int MaxLength
		{
			get
			{
				return txtInput.MaxLength;
			}
			set
			{
				txtInput.MaxLength = value;
			}
		}

		public frmInput()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.AcceptButton = btnOk;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblQuestion = new System.Windows.Forms.Label();
			this.txtInput = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblQuestion
			// 
			this.lblQuestion.Location = new System.Drawing.Point(8, 8);
			this.lblQuestion.Name = "lblQuestion";
			this.lblQuestion.Size = new System.Drawing.Size(376, 56);
			this.lblQuestion.TabIndex = 0;
			// 
			// txtInput
			// 
			this.txtInput.Location = new System.Drawing.Point(8, 72);
			this.txtInput.Name = "txtInput";
			this.txtInput.Size = new System.Drawing.Size(248, 20);
			this.txtInput.TabIndex = 1;
			this.txtInput.Text = "";
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(264, 72);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(56, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(328, 72);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(56, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			// 
			// frmInput
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 101);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnCancel,
																		  this.btnOk,
																		  this.txtInput,
																		  this.lblQuestion});
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmInput";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}
		#endregion
	}
}
