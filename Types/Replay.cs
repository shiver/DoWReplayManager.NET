using System;

namespace DowReplayManager.NET.Types
{
	/// <summary>
	/// Replay file object which holds all the loaded data.
	/// </summary>
	public class Replay
	{
		#region Properties
		private string name;
		/// <summary>
		/// Internal replay name
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		private string filename;
		/// <summary>
		/// Raw replay filename
		/// </summary>
		public string Filename
		{
			get
			{
				return filename;
			}
			set
			{
				filename = value;
			}
		}

		/*private int rating;
		public int Rating
		{
			get
			{
				return rating;
			}

		}*/

		private DateTime date;
		/// <summary>
		/// Date .rec was last modified
		/// </summary>
		public DateTime Date
		{
			get
			{
				return date;
			}
			set
			{
				date = value;
			}
		}

		private int version;
		/// <summary>
		/// DoW version required (approximation)
		/// </summary>
		public int Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		private PlayerCollection players;
		/// <summary>
		/// Collection of the Player objects
		/// </summary>
		public PlayerCollection Players
		{
			get
			{
				return players;
			}
			set
			{
				players = value;
			}
		}

		private int numteams;
		public int NumTeams
		{
			get
			{
				return numteams;
			}
			set
			{
				numteams = value;
			}
		}

		private TimeSpan duration;
		/// <summary>
		/// Length of the replay
		/// </summary>
		public TimeSpan Duration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
			}
		}

		private string map;
		/// <summary>
		/// Map played
		/// </summary>
		public string Map
		{
			get
			{
				return map;
			}
			set
			{
				map = value;
			}
		}

		private int mapsize;
		/// <summary>
		/// Map block size
		/// </summary>
		public int MapSize
		{
			get
			{
				return mapsize;
			}
			set
			{
				mapsize = value;
			}
		}

		/*private string comments;
		public string Comments
		{
			get
			{
				return comments;
			}
			set
			{
				comments = value;
			}
		}*/

		private GameOptionsType gameopts;
		public GameOptionsType GameOptions
		{
			get
			{
				return gameopts;
			}
			set
			{
				gameopts = value;
			}
		}

		private WinConditionsType wincons;
		public WinConditionsType WinConditions
		{
			get
			{
				return wincons;
			}
			set
			{
				wincons = value;
			}
		}

		private ChatType chat;
		public ChatType Chat
		{
			get
			{
				return chat;
			}
			set
			{
				chat = value;
			}
		}

		private byte[] hashcode;
		public byte[] HashCode
		{
			get
			{
				return hashcode;
			}
			set
			{
				hashcode = value;
			}
		}

		private bool isvalid;
		public bool IsValid
		{
			get
			{
				return isvalid;
			}
			set
			{
				isvalid = value;
			}
		}

		private int filesize;
		public int FileSize
		{
			get
			{
				return filesize;
			}
			set
			{
				filesize = value;
			}
		}

		private int foldinfopos;
		public int FOLDINFOPOS
		{
			get
			{
				return foldinfopos;
			}
			set
			{
				foldinfopos = value;
			}
		}

		private int foldinfo;
		public int FOLDINFO
		{
			get
			{
				return foldinfo;
			}
			set
			{
				foldinfo = value;
			}
		}

		private int database;
		public int DATABASE
		{
			get
			{
				return database;
			}
			set
			{
				database = value;
			}
		}

		private int databasepos;
		public int DATABASEPOS
		{
			get
			{
				return databasepos;
			}
			set
			{
				databasepos = value;
			}
		}

		private int replaylenpos;
		public int REPLAYLENPOS
		{
			get
			{
				return replaylenpos;
			}
			set
			{
				replaylenpos = value;
			}
		}
		#endregion

	}
}
