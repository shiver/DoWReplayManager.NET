using System;
using System.Collections;

namespace DowReplayManager.NET.Types
{
	/// <summary>
	/// Summary description for PlayerCollection.
	/// </summary>
	public class PlayerCollection
	{
		protected ArrayList players;
		
		private int numplayers;
		public int NumPlayers
		{
			get
			{
				return numplayers;
			}
		}

		public Player this[int i]
		{
			get
			{
				return (Player) players[i];
			}
			set
			{
				players[i] = value;
			}
		}

		public void Add(Player player)
		{
			players.Add(player);
			numplayers++;
		}

		public PlayerCollection()
		{
			players = new ArrayList();
		}
	}
}
