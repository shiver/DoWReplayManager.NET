using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using DowReplayManager.NET.Types;

namespace DowReplayManager.NET
{
	/// <summary>
	/// Summary description for frmReplayView.
	/// </summary>
	public class frmReplayView : System.Windows.Forms.Form
	{
		private int mouse_x;
		private int mouse_y;
		private frmPlayerView playerview = new frmPlayerView();

		private Replay replay;
		public Replay Replay
		{
			get
			{
				return replay;
			}
			set
			{
				replay = value;
			}
		}
		private System.Windows.Forms.RichTextBox rtbChat;
		private System.Windows.Forms.TreeView tvPlayers;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblReplayName;
		private System.Windows.Forms.Label lblMapName;
		private System.Windows.Forms.Label lblDuration;
		private System.Windows.Forms.Label lblMapSize;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Panel panel1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmReplayView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
			this.rtbChat = new System.Windows.Forms.RichTextBox();
			this.tvPlayers = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lblReplayName = new System.Windows.Forms.Label();
			this.lblMapName = new System.Windows.Forms.Label();
			this.lblDuration = new System.Windows.Forms.Label();
			this.lblMapSize = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// rtbChat
			// 
			this.rtbChat.BackColor = System.Drawing.Color.White;
			this.rtbChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.rtbChat.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.rtbChat.ForeColor = System.Drawing.Color.Black;
			this.rtbChat.Location = new System.Drawing.Point(24, 192);
			this.rtbChat.Name = "rtbChat";
			this.rtbChat.ReadOnly = true;
			this.rtbChat.Size = new System.Drawing.Size(304, 168);
			this.rtbChat.TabIndex = 3;
			this.rtbChat.Text = "";
			// 
			// tvPlayers
			// 
			this.tvPlayers.BackColor = System.Drawing.Color.White;
			this.tvPlayers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tvPlayers.ForeColor = System.Drawing.Color.Black;
			this.tvPlayers.ImageIndex = -1;
			this.tvPlayers.Location = new System.Drawing.Point(16, 24);
			this.tvPlayers.Name = "tvPlayers";
			this.tvPlayers.SelectedImageIndex = -1;
			this.tvPlayers.Size = new System.Drawing.Size(176, 136);
			this.tvPlayers.TabIndex = 4;
			this.tvPlayers.MouseHover += new System.EventHandler(this.tvPlayers_MouseHover);
			this.tvPlayers.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tvPlayers_MouseMove);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(200, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Replay Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Black;
			this.label2.Location = new System.Drawing.Point(208, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Map Name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(408, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 16);
			this.label3.TabIndex = 7;
			this.label3.Text = "Map Size:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.Black;
			this.label4.Location = new System.Drawing.Point(224, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "Duration:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.Black;
			this.label5.Location = new System.Drawing.Point(8, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 16);
			this.label5.TabIndex = 9;
			this.label5.Text = "Players:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.ForeColor = System.Drawing.Color.Black;
			this.label6.Location = new System.Drawing.Point(8, 176);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(40, 16);
			this.label6.TabIndex = 10;
			this.label6.Text = "Chat:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblReplayName
			// 
			this.lblReplayName.ForeColor = System.Drawing.Color.Black;
			this.lblReplayName.Location = new System.Drawing.Point(288, 16);
			this.lblReplayName.Name = "lblReplayName";
			this.lblReplayName.Size = new System.Drawing.Size(240, 16);
			this.lblReplayName.TabIndex = 11;
			this.lblReplayName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblMapName
			// 
			this.lblMapName.ForeColor = System.Drawing.Color.Black;
			this.lblMapName.Location = new System.Drawing.Point(288, 40);
			this.lblMapName.Name = "lblMapName";
			this.lblMapName.Size = new System.Drawing.Size(120, 16);
			this.lblMapName.TabIndex = 12;
			this.lblMapName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDuration
			// 
			this.lblDuration.ForeColor = System.Drawing.Color.Black;
			this.lblDuration.Location = new System.Drawing.Point(288, 64);
			this.lblDuration.Name = "lblDuration";
			this.lblDuration.Size = new System.Drawing.Size(72, 16);
			this.lblDuration.TabIndex = 13;
			this.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblMapSize
			// 
			this.lblMapSize.ForeColor = System.Drawing.Color.Black;
			this.lblMapSize.Location = new System.Drawing.Point(464, 40);
			this.lblMapSize.Name = "lblMapSize";
			this.lblMapSize.Size = new System.Drawing.Size(72, 16);
			this.lblMapSize.TabIndex = 14;
			this.lblMapSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnClose
			// 
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.ForeColor = System.Drawing.Color.Black;
			this.btnClose.Location = new System.Drawing.Point(528, 0);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(16, 16);
			this.btnClose.TabIndex = 15;
			this.btnClose.Text = "X";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.ForeColor = System.Drawing.Color.White;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(544, 368);
			this.panel1.TabIndex = 16;
			// 
			// frmReplayView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 368);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.lblMapSize);
			this.Controls.Add(this.lblDuration);
			this.Controls.Add(this.lblMapName);
			this.Controls.Add(this.lblReplayName);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tvPlayers);
			this.Controls.Add(this.rtbChat);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.Name = "frmReplayView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Load += new System.EventHandler(this.frmReplayView_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void pbMap_Click(object sender, System.EventArgs e)
		{
		
		}

		private void PopulateTeams()
		{
			//add team roots
			TreeNode node = null;
			for (int index = 0; index < Replay.NumTeams; index++)
			{
				node = new TreeNode("Team " + (index + 1));
				node.Tag = index + 1;
				tvPlayers.Nodes.Add(node);
			}

			//add players to teams
			Player player = null;
			for (int index = 0; index < Replay.Players.NumPlayers; index++)
			{
				player = Replay.Players[index];

				if (player != null)
				{
					node = new TreeNode(player.Name);
					node.Tag = index;
					tvPlayers.Nodes[player.Team - 1].Nodes.Add(node);
				}
			}
			tvPlayers.ExpandAll();
		}

		private void PopulateChat()
		{
			for (int index = 0; index < Replay.Chat.Length; index++)
			{
				string time = TicksToTime((int)Replay.Chat[index].Time);
				string sender = Replay.Chat[index].Sender;
				if (sender.Length == 0)
					sender = "(System)";
				else
					sender = "[" + sender + "]";

				rtbChat.Text += 
					time + " : " + 
					sender + " : " +
					Replay.Chat[index].Text + "\n";
			}
		}

		public static string TicksToTime(int total_num_ticks)
		{
			total_num_ticks = total_num_ticks / 8;
			int secs = total_num_ticks % 60;
			int mins = (total_num_ticks - secs) / 60;
			return(mins.ToString("D2") + ":" + secs.ToString("D2"));
		}

		private void PopulateWinConditions()
		{
			
		}

		private void frmReplayView_Load(object sender, System.EventArgs e)
		{
			lblReplayName.Text = Replay.Name;
			lblMapName.Text = Replay.Map;
			lblMapSize.Text = Replay.MapSize.ToString();
			lblDuration.Text = Replay.Duration.ToString();

			PopulateTeams();
			PopulateChat();
			PopulateWinConditions();
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			playerview.Close();
			this.Close();
		}

		private void tvPlayers_MouseHover(object sender, System.EventArgs e)
		{
			if (playerview.Visible == false)
			{				
				TreeNode node = tvPlayers.GetNodeAt(new Point(mouse_x, mouse_y));
				if (node != null && node.Nodes.Count == 0)
				{
					playerview.Player = Replay.Players[(int)node.Tag];
					Point screen = PointToScreen(new Point(mouse_x, mouse_y));
					playerview.Location = screen;
					playerview.PopulateInfo();
					playerview.Visible = true;
				}
			}
		}

		private void tvPlayers_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (playerview != null && e.X != mouse_x && e.Y != mouse_y)
				playerview.Visible = false;

			mouse_x = e.X;
			mouse_y = e.Y;

			
		}
	}
}
