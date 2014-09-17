using System;
using System.Collections;
using System.Windows.Forms;
using System.Data;

using DowReplayManager.NET.Types;
using DowReplayManager.NET.Readers;
using DowReplayManager.NET.Handlers;

using System.Diagnostics;

namespace DowReplayManager.NET.Code
{
	/// <summary>
	/// Summary description for frmMainCode.
	/// </summary>
	public class Methods
	{
		public static string globalfilter = null;
		public static Hashtable htAvailable = null;

		public Methods()
		{
		}

		/// <summary>
		/// Creates a DataView of the Player details which can be sorted, filtered and adjusted
		/// </summary>
		/// <param name="storeReader"></param>
		/// <param name="recs"></param>
		/// <returns>DataView of all Players sorted by Games DESC</returns>
		public static DataView CreatePlayerDataView(StoreReader storeReader, ReplayStore recs)
		{
			DataTable table = new DataTable("Players");
			table.Columns.Add("Player", typeof(string));
			table.Columns.Add("Games", typeof(int));
			table.Columns.Add("ReplayID", typeof(int));

			Debug.WriteLine(table);
			DataRow dvRow = null;
			

			//create a hash table to store the unique player names
			foreach (ReplayStore.PlayerRow row in recs.Player)
			{				
				object reps = storeReader.PlayerReplays(row.Name);
				if (reps != System.DBNull.Value)
				{
					dvRow = table.NewRow();
					dvRow["Player"] = row.Name;
					dvRow["Games"] = (int)reps;
					dvRow["ReplayID"] = row.ReplayID;
					table.Rows.Add(dvRow);
				}
			}

			DataView dvPlayers = new DataView(table, "", "Games DESC", DataViewRowState.CurrentRows);
			dvPlayers.Table.CaseSensitive = false;
			return dvPlayers;			
		}

		/// <summary>
		/// Adds Players contained in the supplied DataView to the ListView
		/// </summary>
		/// <param name="lvPlayers"></param>
		/// <param name="dvPlayers"></param>
		/// <param name="filter"></param>
		/// <param name="sort"></param>
		public static void PopulatePlayersView(ListView lvPlayers, DataView dvPlayers, string filter, string sort)
		{
			lvPlayers.BeginUpdate();
			lvPlayers.Items.Clear();
			
			if (globalfilter != null && filter.Length > 0)
			{
				filter = globalfilter + " AND " + filter;
			}
			else if (globalfilter != null)
			{
				filter = globalfilter;
			}

			FormatFilter(ref filter);
			dvPlayers.RowFilter = filter;

			dvPlayers.Sort = sort;
			dvPlayers.RowStateFilter = DataViewRowState.CurrentRows;

			//use a hash table to make sure there are only unique entries
			Hashtable hash = new Hashtable();
			int index = 1;
			foreach (DataRowView dvr in dvPlayers)
			{
				if (!hash.ContainsKey(dvr["Player"]))
				{
					ListViewItem item = 
						new ListViewItem(new string[] {index.ToString(), 
														  dvr["Player"].ToString(), 
														  dvr["Games"].ToString()});

					lvPlayers.Items.Add(item);
					hash.Add(dvr["Player"], null);
					index++;
				}
			}

			lvPlayers.EndUpdate();
		}

		/// <summary>
		/// Creates a DataView of the Player details which can be sorted, filtered and adjusted
		/// </summary>
		/// <param name="storeReader"></param>
		/// <param name="recs"></param>
		/// <returns>DataView of all Maps sorted by Games DESC</returns>
		public static DataView CreateMapDataView(StoreReader storeReader, ReplayStore recs)
		{
			DataTable table = new DataTable("Maps");
			table.Columns.Add("Map", typeof(string));
			table.Columns.Add("Games", typeof(int));
			table.Columns.Add("ReplayID", typeof(int));

			DataRow dvRow;

			foreach (ReplayStore.MapRow row in recs.Map)
			{	
				object reps = storeReader.MapReplays(row.Name);
				if (reps != System.DBNull.Value)
				{
					dvRow = table.NewRow();
					dvRow["Map"] = row.Name;
					dvRow["Games"] = (int)reps;
					dvRow["ReplayID"] = row.ReplayID;
					table.Rows.Add(dvRow);
				}
			}

			DataView dvMaps = new DataView(table, "", "Games DESC", DataViewRowState.CurrentRows);
			dvMaps.Table.CaseSensitive = false;
			return dvMaps;			
		}

		/// <summary>
		/// Adds Players contained in the supplied DataView to the ListView
		/// </summary>
		/// <param name="lvMaps"></param>
		/// <param name="dvMaps"></param>
		/// <param name="filter"></param>
		/// <param name="sort"></param>
		public static void PopulateMapsView(ListView lvMaps, DataView dvMaps, string filter, string sort)
		{
			lvMaps.BeginUpdate();
			lvMaps.Items.Clear();

			if (globalfilter != null && filter.Length > 0)
			{
				filter = globalfilter + " AND " + filter;
			}
			else if (globalfilter != null)
			{
				filter = globalfilter;
			}

			FormatFilter(ref filter);
			dvMaps.RowFilter = filter;

			dvMaps.Sort = sort;
			dvMaps.RowStateFilter = DataViewRowState.CurrentRows;

			//use a hash table to make sure there are only unique entries
			Hashtable hash = new Hashtable();
			int index = 1;
			foreach (DataRowView dvr in dvMaps)
			{
				if (!hash.ContainsKey(dvr["Map"]))
				{
					ListViewItem item = new ListViewItem(
						new string[] {	index.ToString(), 
										 dvr["Map"].ToString(), 
										 dvr["Games"].ToString()});
					lvMaps.Items.Add(item);
					hash.Add(dvr["Map"], null);
					index++;
				}
			}

			lvMaps.EndUpdate();
		}

		public static DataView CreateReplayDataView(StoreReader storeReader, ReplayStore recs)
		{
			DataTable table = new DataTable("Replays");
			table.Columns.Add("ReplayID", typeof(int));
			table.Columns.Add("Name", typeof(string));
			table.Columns.Add("Modified", typeof(DateTime));
			table.Columns.Add("Added", typeof(DateTime));
			table.Columns.Add("Filename", typeof(string));
			table.Columns.Add("CategoryID", typeof(int));

			DataRow dvRow;

			foreach (ReplayStore.ReplayRow row in recs.Replay)
			{
				dvRow = table.NewRow();
				dvRow["ReplayID"] = row.ID;
				dvRow["Name"] = row.Name;
				dvRow["Modified"] = row.DateModified;
				dvRow["Added"] = row.DateAdded;
				dvRow["Filename"] = row.Filename;
				dvRow["CategoryID"] = row.CategoryID;
				table.Rows.Add(dvRow);
			}

			DataView dvReplays = new DataView(table, "", "Added DESC", DataViewRowState.CurrentRows);
			dvReplays.Table.CaseSensitive = false;
			return dvReplays;			
		}

		public static void PopulateReplaysView(ListView lvReplays, DataView dvReplays, string filter, string sort)
		{
			lvReplays.BeginUpdate();
			lvReplays.Items.Clear();

			if (globalfilter != null && filter.Length > 0)
			{
				filter = globalfilter + " AND " + filter;
			}
			else if (globalfilter != null)
			{
				filter = globalfilter;
			}

			FormatFilter(ref filter);
			dvReplays.RowFilter = filter;

			dvReplays.Sort = sort;
			dvReplays.RowStateFilter = DataViewRowState.CurrentRows;

			int index = 1;
			foreach (DataRowView dvr in dvReplays)
			{
				ListViewItem item = new ListViewItem(
					new string[] {
					dvr["Name"].ToString(), dvr["Modified"].ToString(),
					dvr["Added"].ToString(), dvr["Filename"].ToString()});

				if (htAvailable != null && htAvailable.ContainsKey(dvr["Filename"].ToString()))
					item.BackColor = System.Drawing.Color.PowderBlue;
				item.Tag = dvr["ReplayID"];
				lvReplays.Items.Add(item);
				index++;
			}

			lvReplays.EndUpdate();
		}

		/// <summary>
		/// Get a list of ReplayIDs using the supplied player name
		/// </summary>
		/// <param name="dvPlayers"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static object[] GetReplaysByPlayerName(DataView dvPlayers, string name)
		{
			string old_filter = dvPlayers.RowFilter;
			dvPlayers.RowFilter = "Player = '" + name + "'";

			System.Collections.ArrayList list = new ArrayList();
			
			foreach (DataRowView dvr in dvPlayers)
			{
				list.Add(dvr["ReplayID"]);
			}

			dvPlayers.RowFilter = old_filter;
			return list.ToArray();
		}

		public static object[] GetReplaysByMapName(DataView dvMaps, string name)
		{
			string old_filter = dvMaps.RowFilter;
			dvMaps.RowFilter = "Map = '" + name + "'";

			System.Collections.ArrayList list = new ArrayList();
			
			foreach (DataRowView dvr in dvMaps)
			{
				list.Add(dvr["ReplayID"]);
			}

			dvMaps.RowFilter = old_filter;
			return list.ToArray();
		}

		public static object[] GetReplaysByCategoryID(DataView dvReplays, int id)
		{
			string old_filter = dvReplays.RowFilter;
 
			if (id != 1)
				dvReplays.RowFilter = "CategoryID = " + id;

			System.Collections.ArrayList list = new ArrayList();

			foreach (DataRowView dvr in dvReplays)
			{
				list.Add(dvr["ReplayID"]);
			}
			
			dvReplays.RowFilter = old_filter;
			return list.ToArray();
		}

		public static string GetReplayFilenameByReplayID(DataView dvReplays, int id)
		{
			string old_filter = dvReplays.RowFilter;

			dvReplays.RowFilter = "ReplayID = " + id;

			string filename = null;
			foreach (DataRowView dvr in dvReplays)
			{
				filename = (string)dvr["Filename"];
			}

			dvReplays.RowFilter = old_filter;
			return filename;
		}

		public static void GlobalReplayFilter(DataView dvReplays, DataView dvPlayers, DataView dvMaps, string filter)
		{
			globalfilter = filter;
		}

		/// <summary>
		/// Changes the supplied Replays' Category ID to the NewCategoryID
		/// </summary>
		/// <param name="ReplayIDs"></param>
		/// <param name="NewCategoryID"></param>
		/// <param name="storeReader"></param>
		public static void AlterReplayCategory(object[] ReplayIDs, int NewCategoryID, StoreReader storeReader)
		{
			System.Text.StringBuilder s_Replayids = new System.Text.StringBuilder();
			
			if (ReplayIDs.Length > 0)
			{
				foreach (object replayid in ReplayIDs)
				{
					if (s_Replayids.Length == 0)
						s_Replayids.Append("ID = " + replayid);
					else
						s_Replayids.Append(" OR ID = " + replayid); 
				}
			
				
				DataRow[] drRows = storeReader.Replays.Tables["Replay"].Select(s_Replayids.ToString());
				foreach (ReplayStore.ReplayRow row in drRows)
				{
					row.CategoryID = NewCategoryID;
				}

				storeReader.Replays.Merge(drRows);

				//save changes
				if (!storeReader.SaveReplays("rec.dat"))
					MessageBox.Show(null, StoreReader.STORE_REPLAYSAVE_FAIL, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Fetches the category tree node using the category tag ID
		/// </summary>
		/// <param name="tag">Tag to search for</param>
		/// <param name="nodes">Node collection that is searched</param>
		/// <returns>A TreeNode object if successful or null</returns>
		private static TreeNode GetNodeByTag(int tag, TreeNodeCollection nodes)
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

		public static void PopulateCategoriesView(TreeView tvCategories, StoreReader storeReader)
		{
			tvCategories.Nodes.Clear();

			TreeNode node;
			foreach (CategoryStore.CategoryRow row in storeReader.Categories.Category)
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

		public static void CreateNewCategory(StoreReader storeReader, string NewCategoryName, int ParentID)
		{
			CategoryStore.CategoryRow row = storeReader.Categories.Category.NewCategoryRow();
			row["Name"] = NewCategoryName;
			row["AllowDelete"] = true;
			row["ParentID"] = ParentID;
			row["HasChildren"] = false;

			int id = (int)row["ID"];
			storeReader.Categories.Category.AddCategoryRow(row);

			if (!storeReader.SaveCategories("cat.dat"))
				MessageBox.Show(null, StoreReader.STORE_CATEGORYSAVE_FAIL, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static void RenameCategory(StoreReader storeReader, int CategoryID, string NewCategoryName)
		{
			DataRow[] drRows = storeReader.Categories.Category.Select("ID = " + CategoryID);
			foreach (CategoryStore.CategoryRow row in drRows)
			{
				row.Name = NewCategoryName;
			}
	
			storeReader.Categories.Merge(drRows);
			
			if (!storeReader.SaveCategories("cat.dat"))
				MessageBox.Show(null, StoreReader.STORE_CATEGORYSAVE_FAIL, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);		
		}

		public static void DeleteCategory(StoreReader storeReader, int CategoryID)
		{
			DataRow[] drRows = storeReader.Categories.Category.Select("ID = " + CategoryID);
			foreach (CategoryStore.CategoryRow row in drRows)
			{
				storeReader.Categories.Category.RemoveCategoryRow(row);
			}
			
			if (!storeReader.SaveCategories("cat.dat"))
				MessageBox.Show(null, StoreReader.STORE_CATEGORYSAVE_FAIL, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static void RenameReplayFile(StoreReader storeReader, int ReplayID, string NewFileName)
		{
			DataRow[] drRows = storeReader.Replays.Replay.Select("ID = " + ReplayID);
			foreach (ReplayStore.ReplayRow row in drRows)
			{
				row.Filename = NewFileName;
			}

			storeReader.Replays.Merge(drRows);

			if (!storeReader.SaveReplays("rec.dat"))
				MessageBox.Show(null, StoreReader.STORE_REPLAYSAVE_FAIL, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static void RenameReplay(StoreReader storeReader, int ReplayID, string NewName)
		{
			DataRow[] drRows = storeReader.Replays.Replay.Select("ID = " + ReplayID);
			foreach (ReplayStore.ReplayRow row in drRows)
			{
				row.Name = NewName;
			}

			storeReader.Replays.Merge(drRows);

			if (!storeReader.SaveReplays("rec.dat"))
				MessageBox.Show(null, StoreReader.STORE_REPLAYSAVE_FAIL, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static object[] DeleteReplay(StoreReader storeReader, object[] ReplayIDs)
		{
			DialogResult result = MessageBox.Show(null, 
				"Deleted replays will be totally removed from your system! Are you sure?",
				"Delete replay(s)...",
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

			//store all the replay filenames that we're gonna delete in this Array
			ArrayList list = new ArrayList();

			if (result == DialogResult.Yes)
			{
				System.Text.StringBuilder s_Replayids = new System.Text.StringBuilder();
			
				if (ReplayIDs.Length > 0)
				{
					foreach (object replayid in ReplayIDs)
					{
						if (s_Replayids.Length == 0)
							s_Replayids.Append("ID = " + replayid);
						else
							s_Replayids.Append(" OR ID = " + replayid); 
					}
				}

				//first remove the replay from the store
				DataRow[] drReplay = storeReader.Replays.Replay.Select(s_Replayids.ToString());
				foreach (ReplayStore.ReplayRow row in drReplay)
				{
					list.Add(row.Filename);
					storeReader.Replays.Replay.RemoveReplayRow(row);
				}
				
				//remove from the map store
				DataRow[] drMap = storeReader.Replays.Map.Select(s_Replayids.ToString().Replace("ID", "ReplayID"));
				foreach (ReplayStore.MapRow row in drMap)
				{
					storeReader.Replays.Map.RemoveMapRow(row);
				}
					//remove from the player store
				DataRow[] drPlayers = storeReader.Replays.Player.Select(s_Replayids.ToString().Replace("ID", "ReplayID"));
				foreach (ReplayStore.PlayerRow row in drPlayers)
				{
					storeReader.Replays.Player.RemovePlayerRow(row);
				}
				
				//attempt to save the changes to the store
				if (!storeReader.SaveReplays("rec.dat"))
					MessageBox.Show(null, StoreReader.STORE_REPLAYSAVE_FAIL, "Save failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					return list.ToArray();
			}
			return list.ToArray();
		}

		/// <summary>
		/// Formats the filter correctly for use with special characters
		/// </summary>
		/// <param name="filter"></param>
		public static void FormatFilter(ref string filter)
		{
			if (filter != null && filter.Length > 0)
			{
				if (filter.IndexOf("[") > 0)
					filter = filter.Replace(@"[", @"[[]");

				if (filter.IndexOf("]", filter.IndexOf("]") + 1) > 0)
						filter = filter.Replace(@"]", @"[]]");

				
			}
	    }
	}
}
