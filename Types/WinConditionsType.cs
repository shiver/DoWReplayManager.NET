using System;

namespace DowReplayManager.NET.Types
{
	/// <summary>
	/// Summary description for WinConditionsType.
	/// </summary>
	public class WinConditionsType
	{
		public const int AnnihilateValue = -157708158;
		private bool annihilate = false;
		public bool Annihilate
		{
			get
			{
				return annihilate;
			}
			set
			{
				annihilate = value;
			}
		}

		public const int AssassinateValue = -1158102879;
		private bool assassinate = false;
		public bool Assassinate
		{
			get
			{
				return assassinate;
			}
			set
			{
				assassinate = value;
			}
		}

		public const int ControlAreaValue = 735076042;
		private bool controlarea = false;
		public bool ControlArea
		{
			get
			{
				return controlarea;
			}
			set
			{
				controlarea = value;
			}
		}

		public const int DestroyHQValue = 863969525;
		private bool destroyhq = false;
		public bool DestroyHQ
		{
			get
			{
				return destroyhq;
			}
			set
			{
				destroyhq = value;
			}
		}

		public const int EconomicVictoryValue = -779857721;
		private bool economicvictory = false;
		public bool EconomicVictory
		{
			get
			{
				return economicvictory;
			}
			set
			{
				economicvictory = value;
			}
		}

		private bool gametimer = false;
		
		public const int TakeAndHoldValue = 1959084950;
		private bool takeandhold = false;
		public bool TakeAndHold
		{
			get
			{
				return takeandhold;
			}
			set
			{
				takeandhold = value;
			}
		}

		public const int SuddenDeathValue = -1826760460;
		private bool suddendeath = false;
		public bool SuddenDeath
		{
			get
			{
				return suddendeath;
			}
			set
			{
				suddendeath = value;
			}
		}
		
		public WinConditionsType()
		{
		}
	}
}
