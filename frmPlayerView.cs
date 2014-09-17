using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using DowReplayManager.NET.Types;

namespace DowReplayManager.NET
{
	/// <summary>
	/// Summary description for frmPlayerView.
	/// </summary>
	public class frmPlayerView : System.Windows.Forms.Form
	{
		private bool bMove;

		private Player player;
		private System.Windows.Forms.Label lblPlayerName;
		private System.Windows.Forms.PictureBox pbBanner;
		private System.Windows.Forms.Label lblPlayerBanner;
		private System.Windows.Forms.PictureBox pbBadge;
		private System.Windows.Forms.Label lblPlayerBadge;
		private System.Windows.Forms.PictureBox pbArmyColour;
	
		public Player Player
		{
			get
			{
				return player;
			}
			set
			{
				player = value;
			}
		}
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmPlayerView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Opacity = 0.9;
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
			this.pbArmyColour = new System.Windows.Forms.PictureBox();
			this.lblPlayerName = new System.Windows.Forms.Label();
			this.pbBanner = new System.Windows.Forms.PictureBox();
			this.lblPlayerBanner = new System.Windows.Forms.Label();
			this.pbBadge = new System.Windows.Forms.PictureBox();
			this.lblPlayerBadge = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// pbArmyColour
			// 
			this.pbArmyColour.BackColor = System.Drawing.Color.Transparent;
			this.pbArmyColour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbArmyColour.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.pbArmyColour.Location = new System.Drawing.Point(56, 24);
			this.pbArmyColour.Name = "pbArmyColour";
			this.pbArmyColour.Size = new System.Drawing.Size(120, 16);
			this.pbArmyColour.TabIndex = 15;
			this.pbArmyColour.TabStop = false;
			// 
			// lblPlayerName
			// 
			this.lblPlayerName.BackColor = System.Drawing.Color.Transparent;
			this.lblPlayerName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblPlayerName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPlayerName.ForeColor = System.Drawing.Color.Black;
			this.lblPlayerName.Location = new System.Drawing.Point(-1, -1);
			this.lblPlayerName.Name = "lblPlayerName";
			this.lblPlayerName.Size = new System.Drawing.Size(224, 16);
			this.lblPlayerName.TabIndex = 10;
			this.lblPlayerName.Text = "PlayerName";
			this.lblPlayerName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pbBanner
			// 
			this.pbBanner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbBanner.Location = new System.Drawing.Point(8, 64);
			this.pbBanner.Name = "pbBanner";
			this.pbBanner.Size = new System.Drawing.Size(96, 96);
			this.pbBanner.TabIndex = 8;
			this.pbBanner.TabStop = false;
			// 
			// lblPlayerBanner
			// 
			this.lblPlayerBanner.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPlayerBanner.ForeColor = System.Drawing.Color.Black;
			this.lblPlayerBanner.Location = new System.Drawing.Point(8, 48);
			this.lblPlayerBanner.Name = "lblPlayerBanner";
			this.lblPlayerBanner.Size = new System.Drawing.Size(112, 16);
			this.lblPlayerBanner.TabIndex = 14;
			this.lblPlayerBanner.Text = "PlayerBanner";
			this.lblPlayerBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pbBadge
			// 
			this.pbBadge.BackColor = System.Drawing.Color.Transparent;
			this.pbBadge.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbBadge.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.pbBadge.Location = new System.Drawing.Point(120, 64);
			this.pbBadge.Name = "pbBadge";
			this.pbBadge.Size = new System.Drawing.Size(96, 96);
			this.pbBadge.TabIndex = 9;
			this.pbBadge.TabStop = false;
			// 
			// lblPlayerBadge
			// 
			this.lblPlayerBadge.BackColor = System.Drawing.Color.Transparent;
			this.lblPlayerBadge.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPlayerBadge.ForeColor = System.Drawing.Color.Black;
			this.lblPlayerBadge.Location = new System.Drawing.Point(120, 48);
			this.lblPlayerBadge.Name = "lblPlayerBadge";
			this.lblPlayerBadge.Size = new System.Drawing.Size(100, 16);
			this.lblPlayerBadge.TabIndex = 13;
			this.lblPlayerBadge.Text = "PlayerBadge";
			this.lblPlayerBadge.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// frmPlayerView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(214, 166);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pbArmyColour,
																		  this.lblPlayerName,
																		  this.pbBanner,
																		  this.lblPlayerBanner,
																		  this.pbBadge,
																		  this.lblPlayerBadge});
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "frmPlayerView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Load += new System.EventHandler(this.frmPlayerView_Load);
			this.MouseLeave += new System.EventHandler(this.frmPlayerView_MouseLeave);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Visible = false;
		}

		public void PopulateInfo()
		{
			if (Player.Banner != null)
			{
				pbBanner.Size = Player.Banner.Size;
				pbBanner.Image = Player.Banner;
			}

			if (Player.Badge != null)
			{
				pbBadge.Size = Player.Badge.Size;
				pbBadge.Image = Player.Badge;
			}
			
			//paint the players army colours
			if (Player.ArmyColours != null)
			{
				Bitmap armycolours = new Bitmap(120, 16);
				for (int colour_index = 0; colour_index < 5; colour_index++)
				{
					for (int y = 0; y < 16; y++)
					{
						for (int x = 0; x < 24; x++)
						{
							armycolours.SetPixel(x + (colour_index * 24), y, Player.ArmyColours[colour_index]);
						}
					}
				}
				pbArmyColour.Image = armycolours;
			}

			if (Player.BadgeName != null)
				lblPlayerBadge.Text = Player.BadgeName;

			if (Player.BannerName != null)
				lblPlayerBanner.Text = Player.BannerName;

			lblPlayerName.Text = Player.Name + " (" + Player.Race + ")";
		}

		private void frmPlayerView_Load(object sender, System.EventArgs e)
		{
		}

		private void frmPlayerView_MouseLeave(object sender, System.EventArgs e)
		{
			this.Visible = false;
		}
	}
}
