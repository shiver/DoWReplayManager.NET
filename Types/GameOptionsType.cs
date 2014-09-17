using System;

namespace DowReplayManager.NET.Types
{
	/// <summary>
	/// Summary description for GameOptionsType.
	/// </summary>
	public class GameOptionsType
	{
		public const string AIDifficultyName = "FDIA";
		public enum AIDifficultyType
		{
			Easy,
			Standard,
			Hard,
			Harder,
			Insane
		}

		private AIDifficultyType aidifficulty;
		public AIDifficultyType AIDifficulty
		{
			get
			{
				return aidifficulty;
			}
			set
			{
				aidifficulty = value;
			}
		}

		public const string StartingResourcesName = "TSSR";
		public enum StartingResourcesType
		{
			Standard,
			QuickStart
		}

		private StartingResourcesType startingresources;
		public StartingResourcesType StartingResources
		{
			get
			{
				return startingresources;
			}
			set
			{
				startingresources = value;
			}
		}


		public const string LockTeamsName = "MTKL";
		public enum LockTeamsType
		{
			Yes,
			No
		}

		private LockTeamsType lockteams;
		public LockTeamsType LockTeams
		{
			get
			{
				return lockteams;
			}
			set
			{
				lockteams = value;
			}
		}

		public const string CheatsEnabledName = "AEHC";
		public enum CheatsEnabledType
		{
			Yes,
			No
		}

		private CheatsEnabledType cheatsenabled;
		public CheatsEnabledType CheatsEnabled
		{
			get
			{
				return cheatsenabled;
			}
			set
			{
				cheatsenabled = value;
			}
		}

		public const string StartingLocationName = "COLS";
		public enum StartingLocationType
		{
			Random,
			Fixed
		}

		private StartingLocationType startinglocation;
		public StartingLocationType StartingLocation
		{
			get
			{
				return startinglocation;
			}
			set
			{
				startinglocation = value;
			}
		}

		public const string GameSpeedName = "DPSG";
		public enum GameSpeedType
		{
			VerySlow,
			Slow,
			Normal,
			Fast
		}

		private GameSpeedType gamespeed;
		public GameSpeedType GameSpeed
		{
			get
			{
				return gamespeed;
			}
			set
			{
				gamespeed = value;
			}
		}

		public const string ResourceSharingName = "HSSR";
		public enum ResourceSharingType
		{
			Yes,
			No
		}

		private ResourceSharingType resourcesharing;
		public ResourceSharingType ResourceSharing
		{
			get
			{
				return resourcesharing;
			}
			set
			{
				resourcesharing = value;
			}
		}
		

		public const string ResourceRateName = "TRSR";
		public enum ResourceRateType
		{
			Low,
			Standard,
			High
		}

		private ResourceRateType resourcerate;
		public ResourceRateType ResourceRate
		{
			get
			{
				return resourcerate;
			}
			set
			{
				resourcerate = value;
			}
		}

		public GameOptionsType()
		{
		}
	}
}
