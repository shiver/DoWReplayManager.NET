using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DowReplayManager.NET
{
	/// <summary>
	/// Summary description for frmMaskRename.
	/// </summary>
	public class frmMaskRename : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtMask;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblMaskDetails;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmMaskRename()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			lblMaskDetails.Text = 
				"The possible Mask details are as follows (only tested for 1v1 matches):\n" +
				"\t%1 - Player 1 Name\t\t\t%2 - Player 2 Name\n" +
				"\t%m - Map Name\t\t\t%d - Date and Time";
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
			this.txtMask = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lblMaskDetails = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtMask
			// 
			this.txtMask.Location = new System.Drawing.Point(64, 112);
			this.txtMask.Name = "txtMask";
			this.txtMask.Size = new System.Drawing.Size(344, 20);
			this.txtMask.TabIndex = 0;
			this.txtMask.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 112);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Mask:";
			// 
			// lblMaskDetails
			// 
			this.lblMaskDetails.Location = new System.Drawing.Point(24, 8);
			this.lblMaskDetails.Name = "lblMaskDetails";
			this.lblMaskDetails.Size = new System.Drawing.Size(400, 96);
			this.lblMaskDetails.TabIndex = 2;
			// 
			// frmMaskRename
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(448, 141);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lblMaskDetails,
																		  this.label1,
																		  this.txtMask});
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "frmMaskRename";
			this.Text = "frmMaskRename";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
