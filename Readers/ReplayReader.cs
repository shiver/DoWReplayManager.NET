using System;
using System.IO;
using System.Security.Cryptography;

using System.Diagnostics;

using DowReplayManager.NET.Types;

namespace DowReplayManager.NET.Readers
{
	/// <summary>
	/// Summary description for ReplayReader.
	/// </summary>
	public class ReplayReader
	{
		private FileStream fsReader;
		private Replay replay;
		private string filename;

		#region Properties

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
		#endregion

		#region Constructor
		public ReplayReader()
		{
			replay = new Replay();
		}

		public ReplayReader(string Filename)
		{
			filename = Filename;
			replay = new Replay();
			replay.Filename = filename;
		}

		#endregion

		protected bool openReplay()
		{
			try
			{
				//make sure there is a filename specified
				if (filename.Length == 0)
					return false;

				fsReader = new FileStream(filename, FileMode.Open, FileAccess.Read);
				return true;
			}
			catch
			{
			}
			return false;
		}

		/// <summary>
		/// Populates the last modified date of the .rec file
		/// </summary>
		private void fetchDate()
		{
			replay.Date = File.GetLastWriteTime(filename);
		}

		private TimeSpan CalculateDuration(int ticks)
		{
			ticks /= 8;
			int secs = ticks % 60;
			int mins = (ticks - secs) / 60;
			return new TimeSpan(0, mins, secs);
		}


		/// <summary>
		/// Processes v4 of the .rec file (patch 1.4 / 1.41)
		/// </summary>
		private void readv4(BinaryReader reader)
		{
			int index = 0;
			//skip
			reader.BaseStream.Position += 85;

			replay.Duration = CalculateDuration(reader.ReadInt32());
			
			reader.BaseStream.Position += 36;
			//FOLDINFO tag is in here
			replay.FOLDINFOPOS = (int)reader.BaseStream.Position;
			replay.FOLDINFO = reader.ReadInt32();

			//skip
			reader.BaseStream.Position += 61;
			
			//will use the number of players a bit later
			int numplayers = reader.ReadInt32();
			replay.MapSize = reader.ReadInt32();

			//len of map module
			int mapmodulelen = reader.ReadInt32();
			reader.BaseStream.Position += mapmodulelen;

			//internal map name eg: $1 0 0 3 (dont need this now)
			int internalmaplen = reader.ReadInt32();
			reader.BaseStream.Position += internalmaplen * 2;

			//map name
			replay.Map = new String(reader.ReadChars(reader.ReadInt32()));
			replay.Map = replay.Map.Replace("_", " ");
			replay.Map = replay.Map.Remove(0, replay.Map.LastIndexOf(@"\") + 4);

			//skip
			if (replay.Version > 1)
				reader.BaseStream.Position += 16;
			else
			{
				//FOLDMODI and DATADMOD tags
				reader.BaseStream.Position += 33; //skip
				int foldmodilen = reader.ReadInt32();
				reader.BaseStream.Position += foldmodilen + 4;
			}

			reader.BaseStream.Position += 12;

			replay.DATABASEPOS = (int)reader.BaseStream.Position;
			replay.DATABASE = reader.ReadInt32();
			//DATABASE tag is just before here
			reader.BaseStream.Position += 16;
			
			//game options
			replay.GameOptions = new GameOptionsType();
			int numgameopts = reader.ReadInt32();
			for (index = 0; index < numgameopts; index++)
			{
				int optvalue = reader.ReadInt32();
				string option = new String(reader.ReadChars(4));

				switch (option)
				{
					case GameOptionsType.AIDifficultyName:	//AI Difficulty
						replay.GameOptions.AIDifficulty = (GameOptionsType.AIDifficultyType)optvalue;
						break;
					case GameOptionsType.StartingResourcesName:	//Starting Resources
						replay.GameOptions.StartingResources = (GameOptionsType.StartingResourcesType)optvalue;
						break;
					case GameOptionsType.LockTeamsName:		//Lock Teams
						replay.GameOptions.LockTeams = (GameOptionsType.LockTeamsType)optvalue;
						break;
					case GameOptionsType.CheatsEnabledName:	//Cheats enabled
						replay.GameOptions.CheatsEnabled = (GameOptionsType.CheatsEnabledType)optvalue;
						break;
					case GameOptionsType.StartingLocationName:	//Starting Location
						replay.GameOptions.StartingLocation = (GameOptionsType.StartingLocationType)optvalue;
						break;
					case GameOptionsType.GameSpeedName:		 //Game Speed
						replay.GameOptions.GameSpeed = (GameOptionsType.GameSpeedType)optvalue;
						break;
					case GameOptionsType.ResourceSharingName: //Resource Sharing
						replay.GameOptions.ResourceSharing = (GameOptionsType.ResourceSharingType)optvalue;
						break;
					case GameOptionsType.ResourceRateName:	//Resource Rate
						replay.GameOptions.ResourceRate = (GameOptionsType.ResourceRateType)optvalue;
						break;
					default:
						break;
				}
			}

			//skip 1 byte
			reader.BaseStream.Position++;

			//internal replay name
			replay.REPLAYLENPOS = (int)reader.BaseStream.Position;
			int replaylen = reader.ReadInt32();
			System.Text.UnicodeEncoding unicode = new System.Text.UnicodeEncoding();
			replay.Name = unicode.GetString(reader.ReadBytes(replaylen * 2));
			
			//skip
			reader.BaseStream.Position += 4;

			//win conditions
			replay.WinConditions = new WinConditionsType();
			int numwinconditions = reader.ReadInt32();
			for(index = 0; index < numwinconditions; index++)
			{
				int win_condition = reader.ReadInt32();
				
				switch(win_condition)
				{
					case WinConditionsType.AnnihilateValue:	//Annihilate
						replay.WinConditions.Annihilate = true;
						break;
					case WinConditionsType.AssassinateValue://Assassinate
						replay.WinConditions.Assassinate = true;
						break;
					case WinConditionsType.ControlAreaValue://Control Area
						replay.WinConditions.ControlArea = true;
						break;
					case WinConditionsType.DestroyHQValue:	//Destroy HQ
						replay.WinConditions.DestroyHQ = true;
						break;
					case WinConditionsType.EconomicVictoryValue:	//Economic Victory
						replay.WinConditions.EconomicVictory = true;
						break;
					case WinConditionsType.TakeAndHoldValue:		//Take and Hold
						replay.WinConditions.TakeAndHold = true;
						break;
					case WinConditionsType.SuddenDeathValue:		//Sudden Death
						replay.WinConditions.SuddenDeath = true;
						break;
					default:
						break;
				}
			}

			//Players
			replay.Players = new PlayerCollection();
			for (index = 0; index < numplayers; index++)
			{			
				//skip
				reader.BaseStream.Position += 12;

				//player len
				int playerlen = reader.ReadInt32();
				if (playerlen != 44)	//this is not really needed now.... handled by the observer skip
				{
					replay.Players.Add(new Player());				

					//skip has DATAINFO tag
					reader.BaseStream.Position += 12;

					//skip
					reader.BaseStream.Position += 12;

					//current players name
					int playernamelen = reader.ReadInt32();
					replay.Players[index].Name = unicode.GetString(reader.ReadBytes(playernamelen * 2));

					//skip
					reader.BaseStream.Position += 4;
				
					//players team number
					replay.Players[index].Team = reader.ReadInt32() + 1; //+1 for the 0 base
					if (replay.NumTeams < replay.Players[index].Team)
						replay.NumTeams = replay.Players[index].Team;

					int playerracelen = reader.ReadInt32();
					replay.Players[index].Race = new string(reader.ReadChars(playerracelen));

					//new addition in v1.41 for skirmish check
					reader.BaseStream.Position += 4;
					if (replay.Version >= 4)
						reader.BaseStream.Position += reader.ReadInt32() + 4;
				
					//FOLDTCUC skip
					reader.BaseStream.Position += 32;

					int datalcinlen = reader.ReadInt32();
					reader.BaseStream.Position += datalcinlen + 4;
				
					reader.BaseStream.Position += 20;
					int armynamelen = reader.ReadInt32();
					replay.Players[index].Army = unicode.GetString(reader.ReadBytes(armynamelen * 2));

					replay.Players[index].ArmyColours = new System.Drawing.Color[5];
					for (int i = 0; i < 5; i++)
					{
						byte[] rawcolours = reader.ReadBytes(4);
						replay.Players[index].ArmyColours[i] = 
							System.Drawing.Color.FromArgb(rawcolours[3], rawcolours[2], rawcolours[1], rawcolours[0]);
					}

					for (int i = 0; i < 2; i++)	//badge and banner images
					{
						string tagname = new String(reader.ReadChars(8));

						if (tagname == "FOLDTCBD" || tagname == "FOLDTCBN")
						{
							//skip
							reader.BaseStream.Position += 28;

							int imagenamelen = reader.ReadInt32();
							if (tagname == "FOLDTCBD")
							{
								replay.Players[index].BadgeName = new string(reader.ReadChars(imagenamelen));
							}
							else
							{
								replay.Players[index].BannerName = new string(reader.ReadChars(imagenamelen));
							}

							//skip
							reader.BaseStream.Position += 24;

							//get the size of the image we're about to read
							int xsize = reader.ReadInt32();
							int ysize = reader.ReadInt32();

							//skip
							reader.BaseStream.Position += 24;
						
							if (tagname == "FOLDTCBD")
								replay.Players[index].Badge = new System.Drawing.Bitmap(xsize, ysize);
							else
								replay.Players[index].Banner = new System.Drawing.Bitmap(xsize, ysize);

							for (int y = 0; y < ysize; y++)
							{
								for (int x = 0; x < xsize; x++)
								{
									byte[] rawcolor = reader.ReadBytes(4);
									if (tagname == "FOLDTCBD")
										replay.Players[index].Badge.SetPixel(x, y, 
											System.Drawing.Color.FromArgb(rawcolor[3], rawcolor[2], rawcolor[1], rawcolor[0]));
									else
										replay.Players[index].Banner.SetPixel(x, y, 
											System.Drawing.Color.FromArgb(rawcolor[3], rawcolor[2], rawcolor[1], rawcolor[0]));
								}
							}

							if (tagname == "FOLDTCBD")
								replay.Players[index].Badge.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
							else
								replay.Players[index].Banner.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
						}
						else
						{
							reader.BaseStream.Position -= 8;
						}
					}
				}
				else
				{
					reader.BaseStream.Position += playerlen + 4;
				}
			}

			//convert from zero based index
			//replay.NumTeams++;

			//just skip over the observers for the time being
			string tag = new String(reader.ReadChars(8));
			while (tag == "FOLDGPLY")
			{
				reader.BaseStream.Position += 4;
				int observerlen = reader.ReadInt32();
				reader.BaseStream.Position += observerlen + 4;
				
				tag = new String(reader.ReadChars(8));
				if (tag != "FOLDGPLY")
					reader.BaseStream.Position -= 8;
			}
			//process the chat
			
			replay.Chat = new ChatType();
			int ticks = 0;
			while (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				int type = reader.ReadInt32();
				int len = reader.ReadInt32();

				if (len == 0)	//nothing left... get out
					break;

				switch (type)
				{
					case 1:
						int chattype = reader.ReadInt32();
						if (chattype == 1)
						{
							reader.BaseStream.Position += 5;
							int senderlen = reader.ReadInt32();
							string sender = unicode.GetString(reader.ReadBytes(senderlen*2));
							reader.BaseStream.Position += 12;
							int msg_len = reader.ReadInt32();		//Message Length
							byte[] msg_bytes = reader.ReadBytes(msg_len*2);

							string msg = unicode.GetString(msg_bytes);
							reader.BaseStream.Position = reader.BaseStream.Position;

							replay.Chat.AddMessage(sender, msg, ticks);
						}
						break;
					default:
						//skip
						reader.BaseStream.Position += 2;
						ticks = reader.ReadInt32();
						//skip - what we're already read
						reader.BaseStream.Position += len - 6;
						break;
				}
			}
		}

		/// <summary>
		/// Reads a the specified .rec file
		/// </summary>
		/// <returns>A Replay object if successful and null if not</returns>
		public Replay Read()
		{
			//create the stream
			if (openReplay())
			{
				replay.FileSize = (int)fsReader.Length;
				fetchDate();
				BinaryReader reader = new BinaryReader(fsReader, System.Text.Encoding.ASCII);

				replay.Version = reader.ReadInt32();
				
				switch (replay.Version)
				{
					case 1:
					case 2:
					case 4:	
						try
						{
							readv4(reader);
							replay.IsValid = true;
						}
						catch
						{
							replay.IsValid = false;
							Debug.WriteLine("v" + replay.Version + ": " + fsReader.Name + " failed @ " + reader.BaseStream.Position.ToString());
						}
						break;
					default:
						Debug.WriteLine("v" + replay.Version + ": " + fsReader.Name + " failed. not supported ");
						replay.IsValid = false;
						break;
				}

				replay.HashCode = GetHashCode(fsReader);

				reader.Close();
				fsReader.Close();
				return replay;
			}
			return null;
		}

		private byte[] GetHashCode(FileStream reader)
		{
			byte[] data = new byte[fsReader.Length];
			fsReader.Position = 0;
			fsReader.Read(data, 0, (int)fsReader.Length);
			System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(data);
			data = null;
			return result;
		}
	}
}
