using System;
using System.Collections;
using System.IO;
using DowReplayManager.NET.Types;
using DowReplayManager.NET.Handlers;

namespace DowReplayManager.NET.Readers
{
	/// <summary>
	/// Summary description for StoreReader.
	/// </summary>
	public class StoreReader
	{
		public const string STORE_REPLAYSAVE_FAIL = "Changes to the replay store failed to save.";
		public const string STORE_CATEGORYSAVE_FAIL = "Changes to the categories failed to save.";
		public const string STORE_CATEGORYDELETE_CONFIRM = "Are you sure you want to delete the category named:\n {0}?";
		public const string STORE_CATEGORYDELETE_PERM = "Category {0} is permanent and can not be deleted.";
		public const string STORE_CATEGORYNEW_INPUT = "Please enter the name of the new category (no special characters please)...";
		public const string STORE_REPLAYRENAME_INPUT = "Please enter the new filename for the replay:\n {0}";
		public const string STORE_REPLAYFILERENAME_SELECT = "Renames may only be performed on one replay at a time. Please select one.";
		public const string STORE_REPLAYNAMERENAME_INPUT = "Please enter the new replay name for:\n {0}";

		public Logging log = null;
		#region Properties
		private ReplayStore recs;
		public ReplayStore Replays
		{
			get
			{
				return recs;
			}
		}

		private CategoryStore cats;
		public CategoryStore Categories
		{
			get
			{
				return cats;
			}
		}

		private string replaysfilename;
		public string ReplaysFilename
		{
			get
			{
				return replaysfilename;
			}
			set
			{
				replaysfilename = value;
			}
		}

		private string categoriesfilename;
		public string CategoriesFilename
		{
			get
			{
				return categoriesfilename;
			}
			set
			{
				categoriesfilename = value;
			}
		}

		private string replaymanagerpath;
		public string ReplayManagerPath
		{
			get
			{
				return replaymanagerpath;
			}
			set
			{
				replaymanagerpath = value;
			}
		}
		#endregion

		

		public StoreReader()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="catFilename">Full path to the category store file</param>
		/// <param name="recFilename">Full path to the replay store file</param>
		public StoreReader(string catFilename, string recFilename)
		{
			CategoriesFilename = catFilename;
			ReplaysFilename = recFilename;
		}

		/// <summary>
		/// Attempts to read both the Categories and Replays store
		/// </summary>
		public bool Read()
		{
			try
			{
				cats = new CategoryStore();
				cats.ReadXml(CategoriesFilename);

				recs = new ReplayStore();
				recs.ReadXml(ReplaysFilename);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public void ReadCategories()
		{
			cats = new CategoryStore();
			cats.ReadXml(CategoriesFilename);
		}

		public void ReadReplays()
		{
			recs = new ReplayStore();
			recs.ReadXml(ReplaysFilename);
		}

		public bool SaveReplays(string filename)
		{
			try
			{
				recs.WriteXml(filename);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool SaveCategories(string filename)
		{
			try
			{
				cats.WriteXml(filename);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Creates a hashtable to hold the hash code of each replay
		/// </summary>
		/// <returns></returns>
		private ArrayList GetHashCodes()
		{
			ArrayList hash = new ArrayList();
			foreach (ReplayStore.ReplayRow row in recs.Replay)
			{
				hash.Add(row.HashCode);
			}

			return hash;
		}

		/// <summary>
		/// Adds the supplied replays to the store
		/// </summary>
		/// <param name="filenames">Array of FullPath strings to the replay files</param>
		public void AddReplays(object[] filenames)
		{
			foreach (string filename in filenames)
			{
				string file = ReplayManagerPath + @"\" + Path.GetFileName(filename);

				FileInfo fileinfo = new FileInfo(file);
				ReplayReader reader = new ReplayReader(file);
				Replay replay = reader.Read();

				if (replay != null && replay.IsValid)
				{
					//add the replay
					ReplayStore.ReplayRow row = recs.Replay.AddReplayRow(
						0, replay.Name, replay.HashCode, fileinfo.Name, DateTime.Now, fileinfo.LastWriteTime);
					//add the map
					recs.Map.AddMapRow(replay.Map, row.ID);

					//add the players
					for (int index = 0; index < replay.Players.NumPlayers; index++)
					{
						recs.Player.AddPlayerRow(replay.Players[index].Name, row.ID);
					}
				}
			}
		}


		/// <summary>
		/// Checks if the supplied replay hash codes exist
		/// </summary>
		/// <param name="hashes"></param>
		/// <returns>Array of replays that need to be added to the store</returns>
		public object[] HasHashes(object[] hashes)
		{
			ArrayList list = new ArrayList();
			bool found = false;
			
			foreach (ReplayHash hash in hashes)
			{
				found = false;
				foreach (ReplayStore.ReplayRow row in recs.Replay)
				{
					if (CompareHash(row.HashCode, hash.HashCode))
					{
						found = true;
						break;
					}
				}
				//if we dont find it in the store, add it to the list
				if (found == false)
					list.Add(hash.Filename);
			}

			//return the list of files that need to be added
			return list.ToArray();
		}

		/// <summary>
		/// Compares one byte[] to another
		/// </summary>
		/// <param name="hash1"></param>
		/// <param name="hash2"></param>
		/// <returns>True if they match</returns>
		private bool CompareHash(byte[] hash1, byte[] hash2)
		{
			bool match = false;
			for (int i = 0; i < hash1.Length; i++)
			{
				if (hash1[i] != hash2[i])
				{
					match = false;
					break;
				}

				if (i == hash1.Length - 1)
					return true;
			}
			return match;
		}

		/// <summary>
		/// Finds the category by specifying its name
		/// </summary>
		/// <param name="category">Name of the category (case sensitive)</param>
		/// <returns>Index of the found category or -1</returns>
		public int HasCategory(string category)
		{
			IEnumerator catEnum = cats.Category.GetEnumerator();

			while (catEnum.MoveNext() || catEnum.Current != null)
			{
				CategoryStore.CategoryRow row = (CategoryStore.CategoryRow)catEnum.Current;
				if (row.Name == category)
				{
					return row.ID;
				}
			}
			return  -1;
		}

		public bool HasPlayer(string playername)
		{
			foreach (ReplayStore.PlayerRow row in recs.Player)
			{
				if (row.Name == playername)
					return true;
			}
			return false;
		}

		public object PlayerReplays(string playername)
		{
			object obj;
			try
			{
				obj = recs.Player.Compute("count(Name)", "Name = '" + playername + "'");
				return obj;
			}
			catch(Exception e)
			{
				string a = e.Message;
			}
			return null;
		}

		public object MapReplays(string mapname)
		{
			object obj;
			try
			{
				obj = recs.Map.Compute("count(Name)", "Name = '" + mapname + "'");
				return obj;
			}
			catch(Exception e)
			{
				string a = e.Message;
			}
			return null;
		}

		/// <summary>
		/// Checks if the selected category is permanent or not
		/// </summary>
		/// <returns></returns>
		public bool IsPermanent(int id)
		{
			foreach (CategoryStore.CategoryRow row in cats.Category)
			{
				if (row.ID == id)
					if (row.AllowDelete == true)
						return false;
					else 
						return true;
			}
			return false;
		}

		public ReplayStore.ReplayRow GetReplayByID(int id)
		{
			foreach (ReplayStore.ReplayRow row in recs.Replay)
			{
				if (row.ID == id)
					return row;
			}
			
			return null;
		}
	}
}
