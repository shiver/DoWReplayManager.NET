using System;
using System.Drawing;

namespace DowReplayManager.NET.Types
{
	/// <summary>
	/// Summary description for Player.
	/// </summary>
	public class Player
	{
		private string name;
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

		private int team;
		public int Team
		{
			get
			{
				return team;
			}
			set
			{
				team = value;
			}
		}

		private string race;
		public string Race
		{
			get
			{
				return race;
			}
			set
			{
				race = value;
				convertRace();
			}
		}

		private string army;
		public string Army
		{
			get
			{
				return army;
			}
			set
			{
				army = value;
			}
		}

		private System.Drawing.Color[] armycolours;
		public System.Drawing.Color[] ArmyColours
		{
			get
			{
				return armycolours;
			}
			set
			{
				armycolours = value;
			}
		}

		private string badgename;
		public string BadgeName
		{
			get
			{
				return badgename;
			}
			set
			{
				badgename = value;
			}
		}

		private Bitmap badge;
		public Bitmap Badge
		{
			get
			{
				return badge;
			}
			set
			{
				badge = value;
			}
		}

		private string bannername;
		public string BannerName
		{
			get
			{
				return bannername;
			}
			set
			{
				bannername = value;
			}
		}

		private Bitmap banner;
		public Bitmap Banner
		{
			get
			{
				return banner;
			}
			set
			{
				banner = value;
			}
		}

		private void convertRace()
		{
			switch (race.ToUpper())
			{
				case "CHAOS_MARINE_RACE":
					race = "Chaos";
					break;
				case "SPACE_MARINE_RACE":
					race = "Space Marines";
					break;
				case "ELDAR_RACE":
					race = "Eldar";
					break;
				case "ORK_RACE":
					race = "Ork";
					break;
				case "GUARD_RACE":
					race = "Imperial Guard";
					break;
			}
		}

		public Player()
		{
		}
	}
}
