using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Configuration;

using DowReplayManager.NET.Readers;
using DowReplayManager.NET.Types;
using DowReplayManager.NET.Code;
using DowReplayManager.NET.Handlers;

namespace DowReplayManager.NET
{
	/// <summary>
	/// Summary description for frmMain.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.ColumnHeader chPlayerGames;
		private System.Windows.Forms.ColumnHeader chPlayerRank;
		private System.Windows.Forms.TreeView tvCategories;
		private System.Windows.Forms.ColumnHeader chMapRank;
		private System.Windows.Forms.ColumnHeader chMapGames;

		private StoreReader storeReader;
		private System.Windows.Forms.ColumnHeader chReplayName;
		private System.Windows.Forms.ListView lvReplays;
		private System.Windows.Forms.ListView lvPlayers;
		private System.Windows.Forms.ListView lvMaps;
		private System.Windows.Forms.ColumnHeader chReplayDateAdded;
		private System.Windows.Forms.ColumnHeader chReplayDateModified;
		private System.Windows.Forms.ColumnHeader chReplayFile;
		private System.Windows.Forms.Button btnClearSearch;
		private System.Windows.Forms.ColumnHeader chPlayerName;

		private DataView dvPlayers;
		private DataView dvMaps;
		private DataView dvReplays;
		private ReplayManager replayManager;
		private System.Windows.Forms.ColumnHeader chMapName;
		private System.Windows.Forms.ContextMenu cmPReplay;
		private System.Windows.Forms.MenuItem cmPRename;
		private System.Windows.Forms.MenuItem cmPFileRename;
		private System.Windows.Forms.MenuItem cmPAvailable;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.ContextMenu cmPCat;
		private int selectedCategory;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem miPCatRename;
		private System.Windows.Forms.MenuItem miPCatNew;
		private System.Windows.Forms.MenuItem miPCatDelete;
		private System.Windows.Forms.MenuItem cmPReplayView;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.NotifyIcon niMain;
		private System.Windows.Forms.ContextMenu cmNotify;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem miShow;
		private System.Windows.Forms.MenuItem miMaskRename;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem cmPDelete;
		private System.Windows.Forms.StatusBarPanel sbpMessage;
		private System.Windows.Forms.Splitter splitStatusMain;
		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.Panel pnlTopToolbar;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel pnlReplayDetail;
		private System.Windows.Forms.Splitter splitter3;
		private System.Windows.Forms.Panel pnlCategories;
		private System.Windows.Forms.Splitter splitter4;
		private System.Windows.Forms.Panel pnlViewMain;
		private System.Windows.Forms.Splitter splitter5;
		private System.Windows.Forms.Panel pnlViewTop;
		private System.Windows.Forms.Splitter splitter6;
		private System.Windows.Forms.Panel pnlMainTopRight;
		private System.Windows.Forms.ToolBar tbMain;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.StatusBar sbMain;
		private System.Windows.Forms.StatusBarPanel sbpNumReplays;
		private System.Windows.Forms.StatusBarPanel sbpBlank;
		private Logging log = null;

		public frmMain()
		{
			InitializeComponent();

			log = new Logging("DoWRM.log");
			log.Open();
			
			if (ConfigurationSettings.AppSettings["LogLevel"] != null)
				log.LogLevel = Convert.ToInt32(ConfigurationSettings.AppSettings["LogLevel"]);

			log.Write(LogType.Info, 5, "Logging started...");

			//create the replay manager
			log.Write(LogType.Info, 5, "Create ReplayManager object");
			replayManager = new ReplayManager(Application.ExecutablePath, "Replays.zip");
			replayManager.log = log;
			//poll for new replays in the playback folder
			log.Write(LogType.Info, 5, "Polling replay folder (" + replayManager.DoWPlaybackFolder + ")...");
			object[] replayHashes = replayManager.PollPlaybackFolder(false);
			log.Write(LogType.Info, 5, "Found " + replayHashes.Length.ToString() + " replays");

			//load the store
			log.Write(LogType.Info, 5, "Create StoreReader object");
			storeReader = new StoreReader("cat.dat", "rec.dat");
			storeReader.log = log;
			storeReader.Read();
			
			//check if we have the replays found in the poll
			object[] newreplays = null;
			if (replayHashes != null)
			{
				newreplays = storeReader.HasHashes(replayHashes);
				log.Write(LogType.Info, 5, "Found " + newreplays.Length.ToString() + " NEW replay(s)");
			}

			//add replays that we dont have...
			if (newreplays != null && newreplays.Length > 0)
			{
				log.Write(LogType.Info, 5, "Adding replay(s) to store...");

				replayManager.StoreReader = storeReader;
				replayManager.Archive = ReplayManager.ArchiveType.FileStore;
				replayManager.AddReplays(newreplays);
			}
			log.Write(LogType.Info, 5, "Check for Available replays...");
			Methods.htAvailable = replayManager.GetAvailable();
			if (Methods.htAvailable != null)
				log.Write(LogType.Info, 5, "Found " + Methods.htAvailable.Count.ToString() + " Available replays");

			log.Write(LogType.Info, 5, "Creating and populating views...");
			Methods.PopulateCategoriesView(tvCategories, storeReader);

			dvPlayers = Methods.CreatePlayerDataView(storeReader, storeReader.Replays);
			Methods.PopulatePlayersView(lvPlayers, dvPlayers, null, "Games DESC");

			dvMaps = Methods.CreateMapDataView(storeReader, storeReader.Replays);
			Methods.PopulateMapsView(lvMaps, dvMaps, null, "Games DESC");

			dvReplays = Methods.CreateReplayDataView(storeReader, storeReader.Replays);
			Methods.PopulateReplaysView(lvReplays, dvReplays, null, "Added DESC");

			//set the replay number in the status bar
			System.Text.StringBuilder sNumReplays = new System.Text.StringBuilder();
			sNumReplays.Append("Replays: ");
			sNumReplays.Append(storeReader.Replays.Replay.Count.ToString());
			sbpNumReplays.Text = sNumReplays.ToString();

			//dont think i need these
			storeReader.Replays.AcceptChanges();
			storeReader.Categories.AcceptChanges();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			log.Close();
			if( disposing )
			{
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.tvCategories = new System.Windows.Forms.TreeView();
			this.cmsPCategories = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miNewCategory = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.miRenameCategory = new System.Windows.Forms.ToolStripMenuItem();
			this.miDeleteCategory = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.makeDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cmPCat = new System.Windows.Forms.ContextMenu();
			this.miPCatRename = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.miPCatNew = new System.Windows.Forms.MenuItem();
			this.miPCatDelete = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.lvReplays = new System.Windows.Forms.ListView();
			this.chReplayName = new System.Windows.Forms.ColumnHeader();
			this.chReplayDateModified = new System.Windows.Forms.ColumnHeader();
			this.chReplayDateAdded = new System.Windows.Forms.ColumnHeader();
			this.chReplayFile = new System.Windows.Forms.ColumnHeader();
			this.cmsPReplays = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miAvailableReplay = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.miRenameReplay = new System.Windows.Forms.ToolStripMenuItem();
			this.replayRenameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.customRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.miDeleteReplay = new System.Windows.Forms.ToolStripMenuItem();
			this.cmPReplay = new System.Windows.Forms.ContextMenu();
			this.cmPAvailable = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.cmPReplayView = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.cmPDelete = new System.Windows.Forms.MenuItem();
			this.cmPRename = new System.Windows.Forms.MenuItem();
			this.cmPFileRename = new System.Windows.Forms.MenuItem();
			this.miMaskRename = new System.Windows.Forms.MenuItem();
			this.lvPlayers = new System.Windows.Forms.ListView();
			this.chPlayerRank = new System.Windows.Forms.ColumnHeader();
			this.chPlayerName = new System.Windows.Forms.ColumnHeader();
			this.chPlayerGames = new System.Windows.Forms.ColumnHeader();
			this.lvMaps = new System.Windows.Forms.ListView();
			this.chMapRank = new System.Windows.Forms.ColumnHeader();
			this.chMapName = new System.Windows.Forms.ColumnHeader();
			this.chMapGames = new System.Windows.Forms.ColumnHeader();
			this.btnClearSearch = new System.Windows.Forms.Button();
			this.niMain = new System.Windows.Forms.NotifyIcon(this.components);
			this.cmNotify = new System.Windows.Forms.ContextMenu();
			this.miShow = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.sbMain = new System.Windows.Forms.StatusBar();
			this.sbpMessage = new System.Windows.Forms.StatusBarPanel();
			this.sbpNumReplays = new System.Windows.Forms.StatusBarPanel();
			this.sbpBlank = new System.Windows.Forms.StatusBarPanel();
			this.splitStatusMain = new System.Windows.Forms.Splitter();
			this.pnlMain = new System.Windows.Forms.Panel();
			this.pnlViewMain = new System.Windows.Forms.Panel();
			this.pnlViewTop = new System.Windows.Forms.Panel();
			this.pnlMainTopRight = new System.Windows.Forms.Panel();
			this.splitter6 = new System.Windows.Forms.Splitter();
			this.splitter5 = new System.Windows.Forms.Splitter();
			this.splitter4 = new System.Windows.Forms.Splitter();
			this.pnlCategories = new System.Windows.Forms.Panel();
			this.splitter3 = new System.Windows.Forms.Splitter();
			this.pnlReplayDetail = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.pnlTopToolbar = new System.Windows.Forms.Panel();
			this.tbMain = new System.Windows.Forms.ToolBar();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.cmsPCategories.SuspendLayout();
			this.cmsPReplays.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sbpMessage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpNumReplays)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpBlank)).BeginInit();
			this.pnlMain.SuspendLayout();
			this.pnlViewMain.SuspendLayout();
			this.pnlViewTop.SuspendLayout();
			this.pnlMainTopRight.SuspendLayout();
			this.pnlCategories.SuspendLayout();
			this.pnlTopToolbar.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvCategories
			// 
			this.tvCategories.AllowDrop = true;
			this.tvCategories.BackColor = System.Drawing.Color.White;
			this.tvCategories.ContextMenuStrip = this.cmsPCategories;
			this.tvCategories.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvCategories.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tvCategories.ForeColor = System.Drawing.Color.Black;
			this.tvCategories.LabelEdit = true;
			this.tvCategories.Location = new System.Drawing.Point(0, 0);
			this.tvCategories.Name = "tvCategories";
			this.tvCategories.ShowRootLines = false;
			this.tvCategories.Size = new System.Drawing.Size(153, 293);
			this.tvCategories.TabIndex = 0;
			this.tvCategories.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvCategories_DragDrop);
			this.tvCategories.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvCategories_AfterLabelEdit);
			this.tvCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvCategories_AfterSelect);
			this.tvCategories.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvCategories_DragEnter);
			this.tvCategories.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvCategories_BeforeLabelEdit);
			// 
			// cmsPCategories
			// 
			this.cmsPCategories.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.miNewCategory,
									this.toolStripSeparator4,
									this.miRenameCategory,
									this.miDeleteCategory,
									this.toolStripSeparator1,
									this.makeDefaultToolStripMenuItem});
			this.cmsPCategories.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
			this.cmsPCategories.Name = "cmsPCategories";
			this.cmsPCategories.Size = new System.Drawing.Size(167, 104);
			// 
			// miNewCategory
			// 
			this.miNewCategory.Name = "miNewCategory";
			this.miNewCategory.Size = new System.Drawing.Size(166, 22);
			this.miNewCategory.Text = "New Category...";
			this.miNewCategory.Click += new System.EventHandler(this.miNewCategoryClick);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(163, 6);
			// 
			// miRenameCategory
			// 
			this.miRenameCategory.Name = "miRenameCategory";
			this.miRenameCategory.Size = new System.Drawing.Size(166, 22);
			this.miRenameCategory.Text = "Rename";
			this.miRenameCategory.Click += new System.EventHandler(this.miRenameCategoryClick);
			// 
			// miDeleteCategory
			// 
			this.miDeleteCategory.Name = "miDeleteCategory";
			this.miDeleteCategory.Size = new System.Drawing.Size(166, 22);
			this.miDeleteCategory.Text = "Delete";
			this.miDeleteCategory.Click += new System.EventHandler(this.miDeleteCategoryClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(163, 6);
			// 
			// makeDefaultToolStripMenuItem
			// 
			this.makeDefaultToolStripMenuItem.Enabled = false;
			this.makeDefaultToolStripMenuItem.Name = "makeDefaultToolStripMenuItem";
			this.makeDefaultToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.makeDefaultToolStripMenuItem.Text = "Make Default";
			// 
			// cmPCat
			// 
			this.cmPCat.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
									this.miPCatRename,
									this.menuItem4,
									this.miPCatNew,
									this.miPCatDelete,
									this.menuItem7,
									this.menuItem8});
			// 
			// miPCatRename
			// 
			this.miPCatRename.DefaultItem = true;
			this.miPCatRename.Index = 0;
			this.miPCatRename.Text = "Rename...";
			this.miPCatRename.Click += new System.EventHandler(this.miPCatRename_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "-";
			// 
			// miPCatNew
			// 
			this.miPCatNew.Index = 2;
			this.miPCatNew.Text = "New Category...";
			this.miPCatNew.Click += new System.EventHandler(this.miPNewCat_Click);
			// 
			// miPCatDelete
			// 
			this.miPCatDelete.Index = 3;
			this.miPCatDelete.Text = "Delete Category...";
			this.miPCatDelete.Click += new System.EventHandler(this.miPCatDelete_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 4;
			this.menuItem7.Text = "-";
			// 
			// menuItem8
			// 
			this.menuItem8.Enabled = false;
			this.menuItem8.Index = 5;
			this.menuItem8.Text = "Make Default";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(165, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Search:";
			// 
			// txtSearch
			// 
			this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSearch.BackColor = System.Drawing.Color.White;
			this.txtSearch.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSearch.ForeColor = System.Drawing.Color.Black;
			this.txtSearch.Location = new System.Drawing.Point(220, 3);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(414, 21);
			this.txtSearch.TabIndex = 0;
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// lvReplays
			// 
			this.lvReplays.AllowColumnReorder = true;
			this.lvReplays.BackColor = System.Drawing.Color.White;
			this.lvReplays.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.chReplayName,
									this.chReplayDateModified,
									this.chReplayDateAdded,
									this.chReplayFile});
			this.lvReplays.ContextMenuStrip = this.cmsPReplays;
			this.lvReplays.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lvReplays.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvReplays.ForeColor = System.Drawing.Color.Black;
			this.lvReplays.FullRowSelect = true;
			this.lvReplays.Location = new System.Drawing.Point(0, 134);
			this.lvReplays.Name = "lvReplays";
			this.lvReplays.Size = new System.Drawing.Size(539, 159);
			this.lvReplays.TabIndex = 5;
			this.lvReplays.UseCompatibleStateImageBehavior = false;
			this.lvReplays.View = System.Windows.Forms.View.Details;
			this.lvReplays.DoubleClick += new System.EventHandler(this.lvReplays_DoubleClick);
			this.lvReplays.SelectedIndexChanged += new System.EventHandler(this.lvReplays_SelectedIndexChanged);
			this.lvReplays.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvReplays_KeyDown);
			this.lvReplays.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvReplays_ColumnClick);
			this.lvReplays.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvReplays_ItemDrag);
			// 
			// chReplayName
			// 
			this.chReplayName.Text = "Name";
			this.chReplayName.Width = 197;
			// 
			// chReplayDateModified
			// 
			this.chReplayDateModified.Text = "Last Modified";
			this.chReplayDateModified.Width = 126;
			// 
			// chReplayDateAdded
			// 
			this.chReplayDateAdded.Text = "Date Added";
			this.chReplayDateAdded.Width = 125;
			// 
			// chReplayFile
			// 
			this.chReplayFile.Text = "Filename";
			this.chReplayFile.Width = 380;
			// 
			// cmsPReplays
			// 
			this.cmsPReplays.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.miAvailableReplay,
									this.toolStripSeparator3,
									this.miRenameReplay,
									this.replayRenameToolStripMenuItem1,
									this.customRenameToolStripMenuItem,
									this.toolStripSeparator2,
									this.miDeleteReplay});
			this.cmsPReplays.Name = "cmsPReplays";
			this.cmsPReplays.Size = new System.Drawing.Size(176, 148);
			// 
			// miAvailableReplay
			// 
			this.miAvailableReplay.CheckOnClick = true;
			this.miAvailableReplay.Name = "miAvailableReplay";
			this.miAvailableReplay.Size = new System.Drawing.Size(175, 22);
			this.miAvailableReplay.Text = "Available";
			this.miAvailableReplay.Click += new System.EventHandler(this.miAvailableReplayClick);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(172, 6);
			// 
			// miRenameReplay
			// 
			this.miRenameReplay.Name = "miRenameReplay";
			this.miRenameReplay.Size = new System.Drawing.Size(175, 22);
			this.miRenameReplay.Text = "Rename";
			this.miRenameReplay.Click += new System.EventHandler(this.miRenameReplayClick);
			// 
			// replayRenameToolStripMenuItem1
			// 
			this.replayRenameToolStripMenuItem1.Enabled = false;
			this.replayRenameToolStripMenuItem1.Name = "replayRenameToolStripMenuItem1";
			this.replayRenameToolStripMenuItem1.Size = new System.Drawing.Size(175, 22);
			this.replayRenameToolStripMenuItem1.Text = "Replay Rename...";
			this.replayRenameToolStripMenuItem1.Visible = false;
			// 
			// customRenameToolStripMenuItem
			// 
			this.customRenameToolStripMenuItem.Enabled = false;
			this.customRenameToolStripMenuItem.Name = "customRenameToolStripMenuItem";
			this.customRenameToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.customRenameToolStripMenuItem.Text = "Custom Rename...";
			this.customRenameToolStripMenuItem.Visible = false;
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(172, 6);
			// 
			// miDeleteReplay
			// 
			this.miDeleteReplay.Name = "miDeleteReplay";
			this.miDeleteReplay.Size = new System.Drawing.Size(175, 22);
			this.miDeleteReplay.Text = "Delete";
			this.miDeleteReplay.Click += new System.EventHandler(this.miDeleteReplayClick);
			// 
			// cmPReplay
			// 
			this.cmPReplay.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
									this.cmPAvailable,
									this.menuItem6,
									this.cmPReplayView,
									this.menuItem2,
									this.cmPDelete,
									this.cmPRename,
									this.cmPFileRename,
									this.miMaskRename});
			// 
			// cmPAvailable
			// 
			this.cmPAvailable.Index = 0;
			this.cmPAvailable.Text = "Available";
			this.cmPAvailable.Click += new System.EventHandler(this.cmPAvailable_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 1;
			this.menuItem6.Text = "-";
			// 
			// cmPReplayView
			// 
			this.cmPReplayView.DefaultItem = true;
			this.cmPReplayView.Index = 2;
			this.cmPReplayView.Text = "View";
			this.cmPReplayView.Click += new System.EventHandler(this.cmPReplayView_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 3;
			this.menuItem2.Text = "-";
			// 
			// cmPDelete
			// 
			this.cmPDelete.Index = 4;
			this.cmPDelete.Text = "Delete";
			this.cmPDelete.Click += new System.EventHandler(this.cmPDelete_Click);
			// 
			// cmPRename
			// 
			this.cmPRename.Index = 5;
			this.cmPRename.Text = "Rename...";
			this.cmPRename.Click += new System.EventHandler(this.cmPRename_Click);
			// 
			// cmPFileRename
			// 
			this.cmPFileRename.Index = 6;
			this.cmPFileRename.Text = "File Rename...";
			this.cmPFileRename.Click += new System.EventHandler(this.cmPFileRename_Click);
			// 
			// miMaskRename
			// 
			this.miMaskRename.Index = 7;
			this.miMaskRename.Text = "Mask Rename...";
			this.miMaskRename.Visible = false;
			this.miMaskRename.Click += new System.EventHandler(this.miMaskRename_Click);
			// 
			// lvPlayers
			// 
			this.lvPlayers.BackColor = System.Drawing.Color.White;
			this.lvPlayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.chPlayerRank,
									this.chPlayerName,
									this.chPlayerGames});
			this.lvPlayers.Dock = System.Windows.Forms.DockStyle.Left;
			this.lvPlayers.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvPlayers.ForeColor = System.Drawing.Color.Black;
			this.lvPlayers.FullRowSelect = true;
			this.lvPlayers.Location = new System.Drawing.Point(0, 0);
			this.lvPlayers.MultiSelect = false;
			this.lvPlayers.Name = "lvPlayers";
			this.lvPlayers.Size = new System.Drawing.Size(262, 131);
			this.lvPlayers.TabIndex = 6;
			this.lvPlayers.UseCompatibleStateImageBehavior = false;
			this.lvPlayers.View = System.Windows.Forms.View.Details;
			this.lvPlayers.SelectedIndexChanged += new System.EventHandler(this.lvPlayers_SelectedIndexChanged);
			this.lvPlayers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvPlayers_ColumnClick);
			// 
			// chPlayerRank
			// 
			this.chPlayerRank.Text = "#";
			this.chPlayerRank.Width = 19;
			// 
			// chPlayerName
			// 
			this.chPlayerName.Text = "Player";
			this.chPlayerName.Width = 118;
			// 
			// chPlayerGames
			// 
			this.chPlayerGames.Text = "Games";
			this.chPlayerGames.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.chPlayerGames.Width = 47;
			// 
			// lvMaps
			// 
			this.lvMaps.BackColor = System.Drawing.Color.White;
			this.lvMaps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.chMapRank,
									this.chMapName,
									this.chMapGames});
			this.lvMaps.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvMaps.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvMaps.ForeColor = System.Drawing.Color.Black;
			this.lvMaps.FullRowSelect = true;
			this.lvMaps.Location = new System.Drawing.Point(0, 0);
			this.lvMaps.MultiSelect = false;
			this.lvMaps.Name = "lvMaps";
			this.lvMaps.Size = new System.Drawing.Size(274, 131);
			this.lvMaps.TabIndex = 7;
			this.lvMaps.UseCompatibleStateImageBehavior = false;
			this.lvMaps.View = System.Windows.Forms.View.Details;
			this.lvMaps.SelectedIndexChanged += new System.EventHandler(this.lvMaps_SelectedIndexChanged);
			this.lvMaps.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvMaps_ColumnClick);
			// 
			// chMapRank
			// 
			this.chMapRank.Text = "#";
			this.chMapRank.Width = 20;
			// 
			// chMapName
			// 
			this.chMapName.Text = "Map";
			this.chMapName.Width = 138;
			// 
			// chMapGames
			// 
			this.chMapGames.Text = "Games";
			this.chMapGames.Width = 48;
			// 
			// btnClearSearch
			// 
			this.btnClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearSearch.Location = new System.Drawing.Point(639, 6);
			this.btnClearSearch.Name = "btnClearSearch";
			this.btnClearSearch.Size = new System.Drawing.Size(48, 18);
			this.btnClearSearch.TabIndex = 8;
			this.btnClearSearch.Text = "Clear";
			this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
			// 
			// niMain
			// 
			this.niMain.ContextMenu = this.cmNotify;
			this.niMain.Text = "Replay Manager";
			this.niMain.Visible = true;
			// 
			// cmNotify
			// 
			this.cmNotify.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
									this.miShow,
									this.menuItem3});
			// 
			// miShow
			// 
			this.miShow.Index = 0;
			this.miShow.Text = "Show";
			this.miShow.Click += new System.EventHandler(this.miShow_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "About";
			// 
			// sbMain
			// 
			this.sbMain.Location = new System.Drawing.Point(0, 492);
			this.sbMain.Name = "sbMain";
			this.sbMain.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
									this.sbpMessage,
									this.sbpNumReplays,
									this.sbpBlank});
			this.sbMain.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.sbMain.ShowPanels = true;
			this.sbMain.Size = new System.Drawing.Size(695, 22);
			this.sbMain.TabIndex = 9;
			// 
			// sbpMessage
			// 
			this.sbpMessage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.sbpMessage.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Raised;
			this.sbpMessage.Name = "sbpMessage";
			this.sbpMessage.Width = 601;
			// 
			// sbpNumReplays
			// 
			this.sbpNumReplays.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.sbpNumReplays.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
			this.sbpNumReplays.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Raised;
			this.sbpNumReplays.Name = "sbpNumReplays";
			this.sbpNumReplays.Text = "Replays: ";
			this.sbpNumReplays.Width = 58;
			// 
			// sbpBlank
			// 
			this.sbpBlank.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.sbpBlank.Name = "sbpBlank";
			this.sbpBlank.Width = 20;
			// 
			// splitStatusMain
			// 
			this.splitStatusMain.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitStatusMain.Enabled = false;
			this.splitStatusMain.Location = new System.Drawing.Point(0, 489);
			this.splitStatusMain.Name = "splitStatusMain";
			this.splitStatusMain.Size = new System.Drawing.Size(695, 3);
			this.splitStatusMain.TabIndex = 10;
			this.splitStatusMain.TabStop = false;
			// 
			// pnlMain
			// 
			this.pnlMain.Controls.Add(this.pnlViewMain);
			this.pnlMain.Controls.Add(this.splitter4);
			this.pnlMain.Controls.Add(this.pnlCategories);
			this.pnlMain.Controls.Add(this.splitter3);
			this.pnlMain.Controls.Add(this.pnlReplayDetail);
			this.pnlMain.Controls.Add(this.splitter2);
			this.pnlMain.Controls.Add(this.pnlTopToolbar);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 0);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(695, 464);
			this.pnlMain.TabIndex = 11;
			// 
			// pnlViewMain
			// 
			this.pnlViewMain.Controls.Add(this.pnlViewTop);
			this.pnlViewMain.Controls.Add(this.splitter5);
			this.pnlViewMain.Controls.Add(this.lvReplays);
			this.pnlViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlViewMain.Location = new System.Drawing.Point(156, 32);
			this.pnlViewMain.Name = "pnlViewMain";
			this.pnlViewMain.Size = new System.Drawing.Size(539, 293);
			this.pnlViewMain.TabIndex = 8;
			// 
			// pnlViewTop
			// 
			this.pnlViewTop.Controls.Add(this.pnlMainTopRight);
			this.pnlViewTop.Controls.Add(this.splitter6);
			this.pnlViewTop.Controls.Add(this.lvPlayers);
			this.pnlViewTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlViewTop.Location = new System.Drawing.Point(0, 0);
			this.pnlViewTop.Name = "pnlViewTop";
			this.pnlViewTop.Size = new System.Drawing.Size(539, 131);
			this.pnlViewTop.TabIndex = 7;
			// 
			// pnlMainTopRight
			// 
			this.pnlMainTopRight.Controls.Add(this.lvMaps);
			this.pnlMainTopRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMainTopRight.Location = new System.Drawing.Point(265, 0);
			this.pnlMainTopRight.Name = "pnlMainTopRight";
			this.pnlMainTopRight.Size = new System.Drawing.Size(274, 131);
			this.pnlMainTopRight.TabIndex = 8;
			// 
			// splitter6
			// 
			this.splitter6.Location = new System.Drawing.Point(262, 0);
			this.splitter6.Name = "splitter6";
			this.splitter6.Size = new System.Drawing.Size(3, 131);
			this.splitter6.TabIndex = 7;
			this.splitter6.TabStop = false;
			// 
			// splitter5
			// 
			this.splitter5.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter5.Location = new System.Drawing.Point(0, 131);
			this.splitter5.Name = "splitter5";
			this.splitter5.Size = new System.Drawing.Size(539, 3);
			this.splitter5.TabIndex = 6;
			this.splitter5.TabStop = false;
			// 
			// splitter4
			// 
			this.splitter4.Location = new System.Drawing.Point(153, 32);
			this.splitter4.Name = "splitter4";
			this.splitter4.Size = new System.Drawing.Size(3, 293);
			this.splitter4.TabIndex = 7;
			this.splitter4.TabStop = false;
			// 
			// pnlCategories
			// 
			this.pnlCategories.Controls.Add(this.tvCategories);
			this.pnlCategories.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlCategories.Location = new System.Drawing.Point(0, 32);
			this.pnlCategories.Name = "pnlCategories";
			this.pnlCategories.Size = new System.Drawing.Size(153, 293);
			this.pnlCategories.TabIndex = 6;
			// 
			// splitter3
			// 
			this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter3.Location = new System.Drawing.Point(0, 325);
			this.splitter3.Name = "splitter3";
			this.splitter3.Size = new System.Drawing.Size(695, 3);
			this.splitter3.TabIndex = 5;
			this.splitter3.TabStop = false;
			// 
			// pnlReplayDetail
			// 
			this.pnlReplayDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlReplayDetail.Location = new System.Drawing.Point(0, 328);
			this.pnlReplayDetail.Name = "pnlReplayDetail";
			this.pnlReplayDetail.Size = new System.Drawing.Size(695, 136);
			this.pnlReplayDetail.TabIndex = 4;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter2.Enabled = false;
			this.splitter2.Location = new System.Drawing.Point(0, 29);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(695, 3);
			this.splitter2.TabIndex = 3;
			this.splitter2.TabStop = false;
			// 
			// pnlTopToolbar
			// 
			this.pnlTopToolbar.Controls.Add(this.label1);
			this.pnlTopToolbar.Controls.Add(this.txtSearch);
			this.pnlTopToolbar.Controls.Add(this.btnClearSearch);
			this.pnlTopToolbar.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTopToolbar.Location = new System.Drawing.Point(0, 0);
			this.pnlTopToolbar.Name = "pnlTopToolbar";
			this.pnlTopToolbar.Size = new System.Drawing.Size(695, 29);
			this.pnlTopToolbar.TabIndex = 0;
			// 
			// tbMain
			// 
			this.tbMain.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.tbMain.ButtonSize = new System.Drawing.Size(16, 16);
			this.tbMain.DropDownArrows = true;
			this.tbMain.Location = new System.Drawing.Point(0, 0);
			this.tbMain.Name = "tbMain";
			this.tbMain.ShowToolTips = true;
			this.tbMain.Size = new System.Drawing.Size(695, 22);
			this.tbMain.TabIndex = 12;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 22);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(695, 3);
			this.splitter1.TabIndex = 13;
			this.splitter1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.pnlMain);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 25);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(695, 464);
			this.panel1.TabIndex = 14;
			// 
			// frmMain
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(695, 514);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.tbMain);
			this.Controls.Add(this.splitStatusMain);
			this.Controls.Add(this.sbMain);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmMain";
			this.Text = "Dawn of War Replay Manager v1.0 - by Shiver";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.cmsPCategories.ResumeLayout(false);
			this.cmsPReplays.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sbpMessage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpNumReplays)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpBlank)).EndInit();
			this.pnlMain.ResumeLayout(false);
			this.pnlViewMain.ResumeLayout(false);
			this.pnlViewTop.ResumeLayout(false);
			this.pnlMainTopRight.ResumeLayout(false);
			this.pnlCategories.ResumeLayout(false);
			this.pnlTopToolbar.ResumeLayout(false);
			this.pnlTopToolbar.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem miAvailableReplay;
		private System.Windows.Forms.ToolStripMenuItem miRenameReplay;
		private System.Windows.Forms.ToolStripMenuItem miDeleteReplay;
		private System.Windows.Forms.ToolStripMenuItem miDeleteCategory;
		private System.Windows.Forms.ToolStripMenuItem miRenameCategory;
		private System.Windows.Forms.ToolStripMenuItem miNewCategory;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem customRenameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem replayRenameToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ContextMenuStrip cmsPReplays;
		private System.Windows.Forms.ToolStripMenuItem makeDefaultToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ContextMenuStrip cmsPCategories;
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

		private void frmMain_Load(object sender, System.EventArgs e)
		{
		}
		
		/// <summary>
		/// Fetches the tree node using the category tag ID
		/// </summary>
		/// <param name="tag">Tag to search for</param>
		/// <param name="nodes">Node collection that is searched</param>
		/// <returns>A TreeNode object if successful or null</returns>
		private TreeNode GetNodeByTag(int tag, TreeNodeCollection nodes)
		{
			for (int rooti = 0; rooti < nodes.Count; rooti++)
			{
				if (nodes[rooti].Tag.ToString() == tag.ToString())
					return nodes[rooti];
				else if (nodes[rooti].GetNodeCount(true) > 0)
				{
					TreeNode node = GetNodeByTag(tag, nodes[rooti].Nodes);
					if (node != null)
						return node;
				}
			}
			return null;
		}

		/// <summary>
		/// Adds the categories to the main TreeView
		/// </summary>
		/// <param name="cats">A CategoryStore object to use as a source</param>
		private void addCategories(CategoryStore cats)
		{
			TreeNode node;
			foreach (CategoryStore.CategoryRow row in cats.Category)
			{
				node = new TreeNode();
				node.Tag = row.ID;
				node.Text = row.Name;

				if (row.ParentID != 0)
				{
					TreeNode pNode = GetNodeByTag(row.ParentID, tvCategories.Nodes);
					if (pNode != null)
						pNode.Nodes.Add(node);
				}
				else
				{						
					tvCategories.Nodes.Add(node);
				}
			}
			tvCategories.ExpandAll();
		}

		/// <summary>
		/// Apply a global filter according to the category that we are selected on
		/// </summary>
		/// <param name="CategoryID"></param>
		/// <returns>Returns the number of replays in the current category or -1 if none</returns>
		private int FilterCategories(int CategoryID)
		{
			if (CategoryID != 1)	// category ID 1 is reserved for "All Replays"
			{
				//fetch all the replayids for the category
				object[] replayids = Methods.GetReplaysByCategoryID(dvReplays, CategoryID);
			
				//kind of lame, but it generates the expression (1x long string) for all the replayids to match
				System.Text.StringBuilder s_Replayids = new System.Text.StringBuilder();;
				if (replayids.Length > 0)
				{
					foreach (object replayid in replayids)
					{
						if (s_Replayids.Length == 0)
							s_Replayids.Append("(ReplayID = " + replayid);
						else
							s_Replayids.Append(" OR ReplayID = " + replayid); 
					}
					s_Replayids.Append(")");		
		
					//apply the global filter to show only the found replay ids
					Methods.GlobalReplayFilter(dvReplays, dvPlayers, dvMaps, s_Replayids.ToString());
				}
				else
					return -1;	//no replays in that category
			}
			else
			{
				//showing all replays... remove global filters
				Methods.GlobalReplayFilter(dvReplays, dvPlayers, dvMaps, null);
			}
			return dvReplays.Table.Rows.Count;
		}

		private void tvCategories_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			selectedCategory = (int)e.Node.Tag;
			//reset the search options. could be slow to refilter after a new category select.
			txtSearch.Text = "";
			
			int result = FilterCategories((int)e.Node.Tag);
			if (result > 0)
			{
				//repopulate the list views with the right replays
				Methods.PopulateReplaysView(lvReplays, dvReplays, "", "Added DESC");
				Methods.PopulatePlayersView(lvPlayers, dvPlayers, "", "Games DESC");
				Methods.PopulateMapsView(lvMaps, dvMaps, "", "Games DESC");
			}
			else
			{
				//if nothing is found... clear the lists
				lvReplays.Items.Clear();
				lvPlayers.Items.Clear();
				lvMaps.Items.Clear();
			}
		}

		private void btnClearSearch_Click(object sender, System.EventArgs e)
		{
			txtSearch.Text = "";
		}

		private void txtSearch_TextChanged(object sender, System.EventArgs e)
		{
			string search_filter = "LIKE '" + txtSearch.Text + "%'";

			Methods.PopulateMapsView(lvMaps, dvMaps, "Map " + search_filter, "Games DESC");
			Methods.PopulatePlayersView(lvPlayers, dvPlayers, "Player " + search_filter, "Games DESC");
			Methods.PopulateReplaysView(lvReplays, dvReplays, "Name " + search_filter, "Added DESC");
		}

		private void lvReplays_DoubleClick(object sender, System.EventArgs e)
		{
			//show the last selected replay
			frmReplayView rv = new frmReplayView();
			string filename = Methods.GetReplayFilenameByReplayID(dvReplays, (int)lvReplays.SelectedItems[lvReplays.SelectedItems.Count - 1].Tag);
			ReplayReader reader = new ReplayReader(replayManager.ReplayManagerFilePath + @"\" + filename);
			rv.Replay = reader.Read();
			rv.ShowDialog();
		}

		private void lvPlayers_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lvPlayers.SelectedItems.Count > 0)
			{
				int iname = chPlayerName.Index;
				object[] replayids = Methods.GetReplaysByPlayerName(dvPlayers, lvPlayers.SelectedItems[0].SubItems[iname].Text);

				System.Text.StringBuilder s_Replayids = new System.Text.StringBuilder();;
				foreach (object replayid in replayids)
				{
					if (s_Replayids.Length == 0)
						s_Replayids.Append("ReplayID = " + replayid);
					else
						s_Replayids.Append(" OR ReplayID = " + replayid); 
				}
				Methods.PopulateReplaysView(lvReplays, dvReplays, s_Replayids.ToString(), "Added DESC");
			}
		}

		private void lvMaps_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lvMaps.SelectedItems.Count > 0)
			{
				int iname = chMapName.Index;
				object[] replayids = Methods.GetReplaysByMapName(dvMaps, lvMaps.SelectedItems[0].SubItems[iname].Text);

				System.Text.StringBuilder s_Replayids = new System.Text.StringBuilder();
				if (replayids.Length > 0)
				{
					foreach (object replayid in replayids)
					{
						if (s_Replayids.Length == 0)
							s_Replayids.Append("ReplayID = " + replayid);
						else
							s_Replayids.Append(" OR ReplayID = " + replayid); 
					}
				}
				Methods.PopulateReplaysView(lvReplays, dvReplays, s_Replayids.ToString(), "Added DESC");
			}
		}

		private void cmPCategory_Click(object sender, System.EventArgs e)
		{
			if (lvReplays.SelectedItems.Count > 0)
			{
				for (int index = 0; index < lvReplays.SelectedItems.Count; index++)
				{
					int itag = (int)lvReplays.SelectedItems[index].Tag;
				}
			}
		}

		private void lvReplays_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			ArrayList list = new ArrayList();
			foreach (ListViewItem item in lvReplays.SelectedItems)
			{
				list.Add(item.Tag);
			}
			
			if (list.Count > 0)
				lvReplays.DoDragDrop(list.ToArray(), DragDropEffects.Move);
		}

		private void tvCategories_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			//fetch the categoryid that the item was dragged to
			TreeNode node = tvCategories.GetNodeAt(
				tvCategories.PointToClient(new Point(e.X, e.Y)));
			int itag = (int)node.Tag;

			if (itag >= 0)
			{
				Methods.AlterReplayCategory((object[])e.Data.GetData(typeof(object[])), itag, storeReader);
				dvReplays = Methods.CreateReplayDataView(storeReader, storeReader.Replays);

				int result = FilterCategories(selectedCategory);
				if (result > 0)
				{
					//repopulate the listviews with the updated category information
					Methods.PopulateReplaysView(lvReplays, dvReplays, "", "Added DESC");
					Methods.PopulatePlayersView(lvPlayers, dvPlayers, "", "Games DESC");
					Methods.PopulateMapsView(lvMaps, dvMaps, "", "Games DESC");
				}
				else
				{
					//if no replays left... clear the list
					lvReplays.Items.Clear();
					lvPlayers.Items.Clear();
					lvMaps.Items.Clear();
				}
			}
		}

		private void tvCategories_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void lvPlayers_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			int cPlayerGames = chPlayerGames.Index;
			int cPlayerName = chPlayerName.Index;
			int cPlayerRank = chPlayerRank.Index;
			
			//get the column and order its currently sorting by
			string[] split = dvPlayers.Sort.Split(' ');

			string column = split[0];
			string order = "";
			if (split.Length >= 2)
				order = split[1];
			//rank column was clicked
			if (e.Column == cPlayerRank)
			{
				if (column.ToUpper() == "GAMES")
				{
					if (order.ToUpper() == "DESC")
						dvPlayers.Sort = "Games";
					else
						dvPlayers.Sort = "Games DESC";
				}
				else
				{
					dvPlayers.Sort = "Games DESC";
				}
				
				Methods.PopulatePlayersView(lvPlayers, dvPlayers, dvPlayers.RowFilter, dvPlayers.Sort);
			}
			else if (e.Column == cPlayerName)	//player name clicked
			{
				if (column.ToUpper() == "Player")
				{
					if (order.ToUpper() == "DESC")
						dvPlayers.Sort = "Player";
					else
						dvPlayers.Sort = "Player DESC";
				}
				else
				{
					if (order.ToUpper() == "DESC")
						dvPlayers.Sort = "Player";
					else
						dvPlayers.Sort = "Player DESC";
				}

				Methods.PopulatePlayersView(lvPlayers, dvPlayers, dvPlayers.RowFilter, dvPlayers.Sort);
			}
			else if (e.Column == cPlayerGames) //num games clicked
			{
				if (column.ToUpper() == "GAMES")
				{
					if (order.ToUpper() == "DESC")
						dvPlayers.Sort = "Games";
					else
						dvPlayers.Sort = "Games DESC";
				}
				else
				{
					if (order.ToUpper() == "DESC")
						dvPlayers.Sort = "Games";
					else
						dvPlayers.Sort = "Games DESC";
				}
				
				Methods.PopulatePlayersView(lvPlayers, dvPlayers, dvPlayers.RowFilter, dvPlayers.Sort);
			}
		}

		private void lvMaps_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			int cMapName = chMapName.Index;
			int cMapGames = chMapGames.Index;
			int cMapRank = chMapRank.Index;
			
			//get the column and order its currently sorting by
			string[] split = dvMaps.Sort.Split(' ');;

			string column = split[0];
			string order = "";
			if (split.Length >= 2)
				order = split[1];

			if (e.Column == cMapName)
			{
				if (column.ToUpper() == "MAP")
				{
					if (order.ToUpper() == "DESC")
						dvMaps.Sort = "Map";
					else
						dvMaps.Sort = "Map DESC";
				}
				else
				{
					if (order.ToUpper() == "DESC")
						dvMaps.Sort = "Map";
					else
						dvMaps.Sort = "Map DESC";
				}

				Methods.PopulateMapsView(lvMaps, dvMaps, dvMaps.RowFilter, dvMaps.Sort);
			}
			else if (e.Column == cMapGames)
			{
				if (column.ToUpper() == "GAMES")
				{
					if (order.ToUpper() == "DESC")
						dvMaps.Sort = "Games";
					else
						dvMaps.Sort = "Games DESC";
				}
				else
				{
					if (order.ToUpper() == "DESC")
						dvMaps.Sort = "Games";
					else
						dvMaps.Sort = "Games DESC";
				}

				Methods.PopulateMapsView(lvMaps, dvMaps, dvMaps.RowFilter, dvMaps.Sort);
			}
			else if (e.Column == cMapRank)
			{
				Methods.PopulateMapsView(lvMaps, dvMaps, dvMaps.RowFilter, dvMaps.Sort);
			}
		}

		private void lvReplays_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			int cReplayName = chReplayName.Index;
			int cReplayModified = chReplayDateModified.Index;
			int cReplayAdded = chReplayDateAdded.Index;
			int cReplayFilename = chReplayFile.Index;

			//get the column and order its currently sorting by
			string[] split = dvReplays.Sort.Split(' ');;

			string column = split[0];
			string order = "";
			if (split.Length >= 2)
				order = split[1];

			if (e.Column == cReplayName)
			{
				if (column.ToUpper() == "NAME")
				{
					if (order.ToUpper() == "DESC")
						dvReplays.Sort = "Name";
					else
						dvReplays.Sort = "Name DESC";
				}
				else
				{
					if (order.ToUpper() == "DESC")
						dvReplays.Sort = "Name";
					else
						dvReplays.Sort = "Name DESC";
				}

				Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort);
			}
			else if (e.Column == cReplayModified)
			{
				if (column.ToUpper() == "MODIFIED")
				{
					if (order.ToUpper() == "DESC")
						dvReplays.Sort = "Modified";
					else
						dvReplays.Sort = "Modified DESC";
				}
				else
				{
					if (order.ToUpper() == "DESC")
						dvReplays.Sort = "Modified";
					else
						dvReplays.Sort = "Modified DESC";
				}

				Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort);
			}
			else if (e.Column == cReplayAdded)
			{
				if (column.ToUpper() == "ADDED")
				{
					if (order.ToUpper() == "DESC")
						dvReplays.Sort = "Added";
					else
						dvReplays.Sort = "Added DESC";
				}
				else
				{
					if (order.ToUpper() == "DESC")
						dvReplays.Sort = "Added";
					else
						dvReplays.Sort = "Added DESC";
				}

				Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort);
			}
			else if (e.Column == cReplayFilename)
			{
				if (column.ToUpper() == "FILENAME")
				{
					if (order.ToUpper() == "DESC")
						dvReplays.Sort = "Filename";
					else
						dvReplays.Sort = "Filename DESC";
				}
				else
				{
					if (order.ToUpper() == "DESC")
						dvReplays.Sort = "Filename";
					else
						dvReplays.Sort = "Filename DESC";
				}

				Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort);
			}
		}

		private void miPCatRename_Click(object sender, System.EventArgs e)
		{
			/*
			tvCategories.SelectedNode.BeginEdit();
			*/
		}

		private void tvCategories_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			//check if we can edit this first...
			if (storeReader.IsPermanent((int)e.Node.Tag))
				tvCategories.SelectedNode.EndEdit(true);
			tvCategories.SelectedNode = e.Node;
		}

		private void tvCategories_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			if (!e.CancelEdit && e.Label != null)
			{
				int itag = (int)e.Node.Tag;
				if (!storeReader.IsPermanent(itag))
					Methods.RenameCategory(storeReader, itag, e.Label);
			}
		}

		private void miPNewCat_Click(object sender, System.EventArgs e)
		{
			/*
			frmInput input = new frmInput();
			input.Question = StoreReader.STORE_CATEGORYNEW_INPUT;
			input.Caption = "Create New Category...";
			DialogResult result = input.ShowDialog(this);

			if (result == DialogResult.OK)
			{
				Methods.CreateNewCategory(storeReader, input.Input, (int)tvCategories.SelectedNode.Tag);
				Methods.PopulateCategoriesView(tvCategories, storeReader);
			}
			*/
		}

		private void miPCatDelete_Click(object sender, System.EventArgs e)
		{
			/*
			//TODO: Replace this null checking with a right click select on a node...
			if (tvCategories.SelectedNode != null)
			{
				if (!storeReader.IsPermanent((int)tvCategories.SelectedNode.Tag))
				{	
					string msg = StoreReader.STORE_CATEGORYDELETE_CONFIRM.Replace("{0}", "'" + tvCategories.SelectedNode.Text + "'");
					DialogResult result = MessageBox.Show(this, msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);		

					if (result == DialogResult.Yes)
					{
						Methods.DeleteCategory(storeReader, (int)tvCategories.SelectedNode.Tag);
						Methods.PopulateCategoriesView(tvCategories, storeReader);
					}
				}
				else
				{
					string error = StoreReader.STORE_CATEGORYDELETE_PERM.Replace("{0}", "'" + tvCategories.SelectedNode.Text + "'");
					MessageBox.Show(this, error, "Delete failed!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			*/
		}

		private void cmPFileRename_Click(object sender, System.EventArgs e)
		{
			if (lvReplays.SelectedItems.Count == 1)
			{
				ReplayStore.ReplayRow row = storeReader.GetReplayByID((int)lvReplays.SelectedItems[0].Tag);

				frmInput input = new frmInput();
				string question = StoreReader.STORE_REPLAYRENAME_INPUT.Replace("{0}", row.Name);;
				input.Question = question;
				input.Caption = "Filename Rename...";
				input.Input = row.Filename;
				input.MaxLength = 255;
				DialogResult result = input.ShowDialog(this);

				if (result == DialogResult.OK)
				{
					if (replayManager.RenameReplayFile(row.Filename, input.Input))
					{
						Methods.RenameReplayFile(storeReader, row.ID, input.Input);
						dvReplays = Methods.CreateReplayDataView(storeReader, storeReader.Replays);
						Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort); 
					}
					else
					{
						string error = ReplayManager.MANAGER_REPLAYFILERENAME_FAIL.Replace("{0}", row.Filename);
						error = error.Replace("{1}", input.Input);

						MessageBox.Show(this, error, "Rename failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			else
				MessageBox.Show(this, StoreReader.STORE_REPLAYFILERENAME_SELECT, "Rename failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void cmPRename_Click(object sender, System.EventArgs e)
		{
			/*
			Replay replay = null;
			if (lvReplays.SelectedItems.Count == 1)
			{
				ReplayStore.ReplayRow row = storeReader.GetReplayByID((int)lvReplays.SelectedItems[0].Tag);

				frmInput input = new frmInput();
				string question = StoreReader.STORE_REPLAYNAMERENAME_INPUT.Replace("{0}", row.Name);;
				input.Question = question;
				input.Caption = "Replay Rename...";
				input.Input = row.Name;
				input.MaxLength = 255;
				DialogResult result = input.ShowDialog(this);

				if (result == DialogResult.OK)
				{
					ReplayReader reader = new ReplayReader(replayManager.ReplayManagerFilePath + @"\" + row.Filename);
					replay = reader.Read();
					if (replay != null && replayManager.RenameReplay(replay, input.Input))
					{
						Methods.RenameReplay(storeReader, row.ID, input.Input);
						dvReplays = Methods.CreateReplayDataView(storeReader, storeReader.Replays);
						Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort);
					}
					else
					{
						string error = ReplayManager.MANAGER_REPLAYFILERENAME_FAIL.Replace("{0}", row.Filename);
						error = error.Replace("{1}", input.Input);

						MessageBox.Show(this, error, "Rename failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			else
				MessageBox.Show(this, StoreReader.STORE_REPLAYFILERENAME_SELECT, "Rename failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			*/	
		}

		private void cmPAvailable_Click(object sender, System.EventArgs e)
		{
			/*
			log.Write(LogType.Info, 5, "Toggling Available for " + lvReplays.SelectedIndices.Count.ToString() + " selected replay(s)");
			try
			{
				foreach (int index in lvReplays.SelectedIndices)
				{
					log.Write(LogType.Info, 5, "Getting ReplayRow for ReplayID=" + lvReplays.Items[index].Tag.ToString());
					ReplayStore.ReplayRow row = storeReader.GetReplayByID((int)lvReplays.Items[index].Tag);

					if (!replayManager.MakeAvailable(replayManager.ReplayManagerFilePath + @"\" + row.Filename))
					{
						string error = ReplayManager.MANAGER_REPLAYAVAIL_FAIL.Replace("{0}", row.Filename);
						MessageBox.Show(this, error, "Make available failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					
					Methods.htAvailable = replayManager.GetAvailable();
					if (!Methods.htAvailable.ContainsKey(row.Filename))
						lvReplays.Items[index].BackColor = System.Drawing.Color.Empty;
					else
						lvReplays.Items[index].BackColor = System.Drawing.Color.PowderBlue;
				}
			}
			catch(Exception x)
			{
				log.Write(LogType.Error, 1, "cmPAvailable_Click(): " + x.StackTrace);
			}
			*/
		}

		private void miShow_Click(object sender, System.EventArgs e)
		{
			this.Show();
			this.ShowInTaskbar = true;

		}

		private void cmPReplayView_Click(object sender, System.EventArgs e)
		{
			lvReplays_DoubleClick(lvReplays, null);
		}

		private void lvReplays_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					lvReplays_DoubleClick(lvReplays, null);
					break;
				case Keys.Delete:
					//Methods.DeleteReplay(lvReplays, dvReplays, int id);
					break;
				default:
					break;
			}			
		}

		private void miMaskRename_Click(object sender, System.EventArgs e)
		{
			frmMaskRename mask = new frmMaskRename();
			mask.ShowDialog();
		}

		private void lvReplays_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//TODO: need to check or uncheck availabe
		}

		private void cmPDelete_Click(object sender, System.EventArgs e)
		{
			/*
			ArrayList list = new ArrayList();
			foreach (ListViewItem item in lvReplays.SelectedItems)
			{
				list.Add(item.Tag);
			}

			if (list.Count > 0)
			{
				object[] filenames = Methods.DeleteReplay(storeReader, list.ToArray());
				if (filenames.Length > 0)
				{
					//delete files in both the Replays folder and Playback
					replayManager.DeleteReplays(filenames);
					//if we were able to save the store etc, reload the views
					dvReplays = Methods.CreateReplayDataView(storeReader, storeReader.Replays);
					Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort);
					dvMaps = Methods.CreateMapDataView(storeReader, storeReader.Replays);
					Methods.PopulateMapsView(lvMaps, dvMaps, dvMaps.RowFilter, dvMaps.Sort);
					dvPlayers = Methods.CreatePlayerDataView(storeReader, storeReader.Replays);
					Methods.PopulatePlayersView(lvPlayers, dvPlayers, dvPlayers.RowFilter, dvPlayers.Sort);
				}
				else
				{
					//failed
				}
			}
			*/
		}
		
		void miNewCategoryClick(object sender, System.EventArgs e)
		{
			frmInput input = new frmInput();
			input.Question = StoreReader.STORE_CATEGORYNEW_INPUT;
			input.Caption = "Create New Category...";
			DialogResult result = input.ShowDialog(this);

			if (result == DialogResult.OK)
			{
				Methods.CreateNewCategory(storeReader, input.Input, (int)tvCategories.SelectedNode.Tag);
				Methods.PopulateCategoriesView(tvCategories, storeReader);
			}
		}
		
		void miRenameCategoryClick(object sender, System.EventArgs e)
		{
			tvCategories.SelectedNode.BeginEdit();
		}
		
		void miDeleteCategoryClick(object sender, System.EventArgs e)
		{
			//TODO: Replace this null checking with a right click select on a node...
			if (tvCategories.SelectedNode != null)
			{
				if (!storeReader.IsPermanent((int)tvCategories.SelectedNode.Tag))
				{	
					string msg = StoreReader.STORE_CATEGORYDELETE_CONFIRM.Replace("{0}", "'" + tvCategories.SelectedNode.Text + "'");
					DialogResult result = MessageBox.Show(this, msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);		

					if (result == DialogResult.Yes)
					{
						Methods.DeleteCategory(storeReader, (int)tvCategories.SelectedNode.Tag);
						Methods.PopulateCategoriesView(tvCategories, storeReader);
					}
				}
				else
				{
					string error = StoreReader.STORE_CATEGORYDELETE_PERM.Replace("{0}", "'" + tvCategories.SelectedNode.Text + "'");
					MessageBox.Show(this, error, "Delete failed!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}
		
		void miAvailableReplayClick(object sender, System.EventArgs e)
		{
			log.Write(LogType.Info, 5, "Toggling Available for " + lvReplays.SelectedIndices.Count.ToString() + " selected replay(s)");
			try
			{
				foreach (int index in lvReplays.SelectedIndices)
				{
					log.Write(LogType.Info, 5, "Getting ReplayRow for ReplayID=" + lvReplays.Items[index].Tag.ToString());
					ReplayStore.ReplayRow row = storeReader.GetReplayByID((int)lvReplays.Items[index].Tag);

					if (!replayManager.MakeAvailable(replayManager.ReplayManagerFilePath + @"\" + row.Filename))
					{
						string error = ReplayManager.MANAGER_REPLAYAVAIL_FAIL.Replace("{0}", row.Filename);
						MessageBox.Show(this, error, "Make available failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					
					Methods.htAvailable = replayManager.GetAvailable();
					if (!Methods.htAvailable.ContainsKey(row.Filename))
						lvReplays.Items[index].BackColor = System.Drawing.Color.Empty;
					else
						lvReplays.Items[index].BackColor = System.Drawing.Color.PowderBlue;
				}
			}
			catch(Exception x)
			{
				log.Write(LogType.Error, 1, "cmPAvailable_Click(): " + x.StackTrace);
			}
		}
		
		void miDeleteReplayClick(object sender, System.EventArgs e)
		{
			ArrayList list = new ArrayList();
			foreach (ListViewItem item in lvReplays.SelectedItems)
			{
				list.Add(item.Tag);
			}

			if (list.Count > 0)
			{
				object[] filenames = Methods.DeleteReplay(storeReader, list.ToArray());
				if (filenames.Length > 0)
				{
					//delete files in both the Replays folder and Playback
					replayManager.DeleteReplays(filenames);
					//if we were able to save the store etc, reload the views
					dvReplays = Methods.CreateReplayDataView(storeReader, storeReader.Replays);
					Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort);
					dvMaps = Methods.CreateMapDataView(storeReader, storeReader.Replays);
					Methods.PopulateMapsView(lvMaps, dvMaps, dvMaps.RowFilter, dvMaps.Sort);
					dvPlayers = Methods.CreatePlayerDataView(storeReader, storeReader.Replays);
					Methods.PopulatePlayersView(lvPlayers, dvPlayers, dvPlayers.RowFilter, dvPlayers.Sort);
				}
				else
				{
					//failed
				}
			}
		}
		
		void miRenameReplayClick(object sender, System.EventArgs e)
		{
			Replay replay = null;
			if (lvReplays.SelectedItems.Count == 1)
			{
				ReplayStore.ReplayRow row = storeReader.GetReplayByID((int)lvReplays.SelectedItems[0].Tag);

				frmInput input = new frmInput();
				string question = StoreReader.STORE_REPLAYNAMERENAME_INPUT.Replace("{0}", row.Name);;
				input.Question = question;
				input.Caption = "Replay Rename...";
				input.Input = row.Name;
				input.MaxLength = 255;
				DialogResult result = input.ShowDialog(this);

				if (result == DialogResult.OK)
				{
					ReplayReader reader = new ReplayReader(replayManager.ReplayManagerFilePath + @"\" + row.Filename);
					replay = reader.Read();
					if (replay != null && replayManager.RenameReplay(replay, input.Input))
					{
						Methods.RenameReplay(storeReader, row.ID, input.Input);
						dvReplays = Methods.CreateReplayDataView(storeReader, storeReader.Replays);
						Methods.PopulateReplaysView(lvReplays, dvReplays, dvReplays.RowFilter, dvReplays.Sort);
					}
					else
					{
						string error = ReplayManager.MANAGER_REPLAYFILERENAME_FAIL.Replace("{0}", row.Filename);
						error = error.Replace("{1}", input.Input);

						MessageBox.Show(this, error, "Rename failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			else
				MessageBox.Show(this, StoreReader.STORE_REPLAYFILERENAME_SELECT, "Rename failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
