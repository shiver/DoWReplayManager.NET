using System;
using System.IO;
using System.Security.Cryptography;
//using ICSharpCode.SharpZipLib.Zip;
//using ICSharpCode.SharpZipLib.Checksums;
using System.Configuration;
using System.Collections;
using System.Windows.Forms;

using DowReplayManager.NET.Types;

namespace DowReplayManager.NET.Handlers
{
	public class ReplayHash
	{
		public string Filename;
		public byte[] HashCode;
	}
	
	public class ReplayManager
	{
		public const string MANAGER_REPLAYFILERENAME_FAIL = "Unable to rename the replay file\n'{0}' to\n'{1}'.";
		public const string MANAGER_REPLAYAVAIL_FAIL = "Unable to make the replay file '{0}' available.";

		#region Properties
		public enum ArchiveType
		{
			ZipCompression,
			FileStore
		}

		private ArchiveType archive;
		public ArchiveType Archive
		{
			get
			{
				return archive;
			}
			set
			{
				archive = value;
			}
		}

		public string DoWPlaybackFolder;
		public string ReplayManagerFile;
		public string ReplayManagerFilePath;
		public Logging log = null;

		private int compressionlevel;
		public int CompressionLevel
		{
			get
			{
				return compressionlevel;
			}
			set
			{
				compressionlevel = value;
			}
		}

		private DowReplayManager.NET.Readers.StoreReader storeReader;
		public DowReplayManager.NET.Readers.StoreReader StoreReader
		{
			get
			{
				return storeReader;
			}
			set
			{
				storeReader = value;
			}
		}
		#endregion

		public ReplayManager()
		{
		}

		public ReplayManager(string path, string filename)
		{
			DoWPlaybackFolder = ConfigurationSettings.AppSettings["DoWPlaybackFolder"].ToString();
			path = path.Remove(path.LastIndexOf(@"\"), path.Length - path.LastIndexOf(@"\"));

			ReplayManagerFilePath = path;
			ReplayManagerFile = filename;


			//this should always exist... if it doesn't create it.
			if (!Directory.Exists(path + @"\Replays"))
			{
				Directory.CreateDirectory(path + @"\Replays");	
			}
			
			ReplayManagerFilePath = ReplayManagerFilePath + @"\Replays";
			ReplayManagerFile = ReplayManagerFilePath + @"\Replays.zip";
		}

		/// <summary>
		/// Adds the supplied file to the existing or newly created ReplayManager archive
		/// </summary>
		/// <param name="filenames">Arrary of fullpath filenames of the replays to add</param>
		public void AddReplays(object[] filenames)
		{
			StoreReader.ReplayManagerPath = ReplayManagerFilePath;
			if (Archive == ArchiveType.FileStore)
			{
				MigrateReplays(filenames);
				StoreReader.AddReplays(filenames);
				StoreReader.SaveReplays("rec.dat");
				CullReplays(filenames);
			}
			else
			{
				/*
				object[] inflated = null;
				StoreReader.ReplayManagerPath = ReplayManagerFilePath;

				if (File.Exists(ReplayManagerFile))
				{
					//take what is in already and uncompress them
					ZipInputStream zfReader = new ZipInputStream(File.OpenRead(ReplayManagerFile));
					inflated = InflateReplays(zfReader);
					zfReader.Close();
				}
			
				//stick them back into a temp .zip file
				ZipOutputStream zfWriter = new ZipOutputStream(File.Create(ReplayManagerFilePath + @"\temp.zip"));
				zfWriter.SetLevel(0);
			
				if (inflated != null)
					DeflateReplays(inflated, zfWriter);
			
				//migrate new replays here
				MigrateReplays(filenames);
				//add the new
				DeflateReplays(filenames, zfWriter);

				zfWriter.Finish();
				zfWriter.Close();
			
				StoreReader.AddReplays(filenames);
				StoreReader.SaveReplays("rec.dat");
				CullReplays(filenames);

				File.Delete(ReplayManagerFile);
				File.Move(ReplayManagerFilePath + @"\temp.zip", ReplayManagerFile);*/
			}
		}

		private void MigrateReplays(object[] filenames)
		{
			foreach (string file in filenames)
			{
				//copy the replay here... ask if we need to overwrite
				if (!File.Exists(ReplayManagerFilePath + @"\" + Path.GetFileName(file)))
				{
					File.Copy(file, ReplayManagerFilePath + @"\" + Path.GetFileName(file), false);
				}
				else
				{
					DialogResult result = MessageBox.Show(null,
						"The replay " + Path.GetFileName(file) + " already exists. Do you wish to overwrite?", 
						"Overwrite existsing...", 
						MessageBoxButtons.YesNoCancel, 
						MessageBoxIcon.Question);

					if (result == DialogResult.Yes)
					{
						File.Copy(file, ReplayManagerFilePath + @"\" + Path.GetFileName(file), true);
					}
				}
			}
		}

		private void CullReplays(object[] filenames)
		{
			foreach (string file in filenames)
			{
				File.Delete(file);
				/*if (Archive != ArchiveType.FileStore)
					File.Delete(ReplayManagerFilePath + @"\" + Path.GetFileName(file));*/
			}
		}

		/// <summary>
		/// Poll the DoW Playback folder for new replays
		/// </summary>
		/// <param name="resursive"></param>
		/// <returns>Returns an array of objects holding the filename and hashcode of each replay</returns>
		public object[] PollPlaybackFolder(bool resursive)
		{
			string[] files = null;
			try
			{
				files = Directory.GetFiles(DoWPlaybackFolder, "*.rec");
			}
			catch
			{
				MessageBox.Show(null, "It appears as if your DoWPlaybackFolder setting is incorrect. Please change it in the DoWRM.exe.Config file.", "Playback folder incorrect...", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
				
			System.Collections.ArrayList list = new System.Collections.ArrayList();
	
			if (files != null)
			{
				foreach (string file in files)
				{
					FileStream fs = File.OpenRead(file);
					byte[] data = new byte[fs.Length];
					fs.Read(data, 0, data.Length);
					fs.Close();

					MD5 md5 = new MD5CryptoServiceProvider();
					byte[] hash = md5.ComputeHash(data);
			
					ReplayHash replayHash = new ReplayHash();
					replayHash.Filename = file;
					replayHash.HashCode = hash;
					list.Add(replayHash);
				}
			}

			return list.ToArray();
		}

		public bool RenameReplayFile(string oldFilename, string newFilename)
		{
			oldFilename = ReplayManagerFilePath + @"\" + oldFilename;
			newFilename = ReplayManagerFilePath + @"\" + newFilename;
			try
			{
				File.Move(oldFilename, newFilename);
				return true;
			}
			catch(Exception x)
			{
				return false;
			}
		}

		/// <summary>
		/// Renames the replays' internal name
		/// </summary>
		/// <param name="newName">New name to use for the replay</param>
		public bool RenameReplay(Replay replay, string newName)
		{
			try
			{
				//load the replay data
				FileStream fsreader = File.OpenRead(replay.Filename);
				byte[] data = new byte[fsreader.Length];
				fsreader.Read(data, 0, data.Length);
				fsreader.Close();

				//create a buffer for the adjusted file
				int new_replaysize = replay.FileSize + ((newName.Length - replay.Name.Length) * 2);
				byte[] buffer = new byte[new_replaysize];
				System.IO.MemoryStream writer = new MemoryStream(buffer);
 
				//calculate the new db & foldinfo len
				int database_len = replay.DATABASE + ((newName.Length - replay.Name.Length) * 2);
				int foldinfo_len = replay.FOLDINFO + ((newName.Length - replay.Name.Length) * 2);

				//write everything into the new buffer up to the foldinfo len
				writer.Write(data, 0, replay.FOLDINFOPOS);
				byte[] foldinfo_byte = BitConverter.GetBytes(foldinfo_len);
				writer.Write(foldinfo_byte, 0, foldinfo_byte.Length);

				writer.Write(data, replay.FOLDINFOPOS + 4, (replay.DATABASEPOS - replay.FOLDINFOPOS - 4));
				byte[] database_byte = BitConverter.GetBytes(database_len);
				writer.Write(database_byte, 0, database_byte.Length);

				writer.Write(data, replay.DATABASEPOS + 4, (replay.REPLAYLENPOS - replay.DATABASEPOS - 4));
				byte[] replay_byte = BitConverter.GetBytes(newName.Length);
				writer.Write(replay_byte, 0, replay_byte.Length);

				//get the new name in bytes
				System.Text.UnicodeEncoding encoder = new System.Text.UnicodeEncoding();
				byte[] byte_name = encoder.GetBytes(newName);
				writer.Write(byte_name, 0, byte_name.Length);

				writer.Write(data, (replay.REPLAYLENPOS + 4 + (replay.Name.Length * 2)), 
					(replay.FileSize - ((replay.REPLAYLENPOS + 4) + replay.Name.Length * 2)));

				writer.Close();

				BinaryWriter binwriter = new BinaryWriter(File.Create(replay.Filename));
				binwriter.Write(buffer);
				binwriter.Close();

				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool MakeAvailable(string filename)
		{
			try
			{
				//if it exists... get rid of it
				log.Write(LogType.Info, 5, "Checking if '" + filename + "' exists in " + DoWPlaybackFolder);
				if (File.Exists(DoWPlaybackFolder + @"\" + Path.GetFileName(filename)))
				{
					log.Write(LogType.Info, 5, "Removing '" + Path.GetFileName(filename) + "' from DoW Playback folder");
					File.Delete(DoWPlaybackFolder + @"\" + Path.GetFileName(filename));
				}
				//if it doesnt put it in the playback folder
				else
				{
					log.Write(LogType.Info, 5, "Making '" + Path.GetFileName(filename) + "' Available in DoW Playback folder");
					File.Copy(filename, DoWPlaybackFolder + @"\" + Path.GetFileName(filename));
				}
				return true;
			}
			catch(Exception x)
			{
				log.Write(LogType.Error, 1, "MakeAvailable() " + x.StackTrace);
				return false;
			}
		}

		public Hashtable GetAvailable()
		{
			try
			{
				Hashtable hash = new Hashtable();
				DirectoryInfo dir = new DirectoryInfo(DoWPlaybackFolder);
				FileInfo[] files = dir.GetFiles("*.rec");

				foreach (FileInfo file in files)
				{
					hash.Add(file.Name, null);
				}

				return hash;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Permanently remove replay files from both the DoW Playback folder and the Replays store
		/// </summary>
		/// <param name="filenames"></param>
		/// <returns>True if totally successful</returns>
		public bool DeleteReplays(object[] filenames)
		{
			try
			{
				foreach (string file in filenames)
				{
					//delete the file if its in the Playback Folder
					if (File.Exists(DoWPlaybackFolder + @"\" + file))
						File.Delete(DoWPlaybackFolder + @"\" + file);

					//delete the file in the store
					if (File.Exists(ReplayManagerFilePath + @"\" + file))
						File.Delete(ReplayManagerFilePath + @"\" + file);
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/*private void DeflateReplays(object[] filenames, ZipOutputStream zfWriter)
		{
			foreach (string file in filenames)
			{
				FileStream fs = File.OpenRead(ReplayManagerFilePath + @"\" + Path.GetFileName(file));
				
				byte[] data = new byte[fs.Length];
				fs.Read(data, 0, data.Length);
				fs.Close();
				ZipEntry entry = new ZipEntry(Path.GetFileName(file));
	
				Crc32 crc = new Crc32();
				crc.Reset();
				crc.Update(data);
				entry.Crc = crc.Value;
				zfWriter.PutNextEntry(entry);
				zfWriter.Write(data, 0, data.Length);

				File.Delete(file);
			}
		}*/

		/*private object[] InflateReplays(ZipInputStream zfReader)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			ZipEntry entry = null;

			int size = 2048;
			byte[] data = null;
			FileStream file = null;

			while ((entry = zfReader.GetNextEntry()) != null)
			{
				list.Add(ReplayManagerFilePath + @"\" + entry.Name);
				file = File.Create(ReplayManagerFilePath + @"\" + entry.Name);

				data = new byte[2048];
				while (true)
				{
					size = zfReader.Read(data, 0, data.Length);
					if (size > 0)
						file.Write(data, 0, size);
					else
						break;
				}
				file.Close();
			}

			return list.ToArray();
		}*/

		/*public bool RenameReplayFile(string oldFilename, string newFilename)
		{
			if (File.Exists(ReplayManagerFile))
			{
				oldFilename = ReplayManagerFilePath + @"\" + oldFilename;
				newFilename = ReplayManagerFilePath + @"\" + newFilename;
				object[] inflated = null;
				//take what is in already and uncompress them
				ZipInputStream zfReader = new ZipInputStream(File.OpenRead(ReplayManagerFile));
				inflated = InflateReplays(zfReader);
				zfReader.Close();

				//stick them back into a temp .zip file
				ZipOutputStream zfWriter = new ZipOutputStream(File.Create(ReplayManagerFilePath + @"\temp.zip"));
				zfWriter.SetLevel(0);
			
				//do the renaming
				File.Move(oldFilename, newFilename);
				for (int index = 0; index < inflated.Length; index++)
				{
					if ((string)inflated[index] == oldFilename)
					{
						inflated[index] = newFilename;
						break;
					}
				}

				if (inflated != null)
					DeflateReplays(inflated, zfWriter);

				zfWriter.Finish();
				zfWriter.Close();

				CullReplays(inflated);

				File.Delete(ReplayManagerFile);
				File.Move(ReplayManagerFilePath + @"\temp.zip", ReplayManagerFile);
				
				return true;
			}
			return false;
		}*/

		/*public bool GetReplay(string filename, string location)
		{
			ZipInputStream zipfile = new ZipInputStream(File.OpenRead(ReplayManagerFile));

			ZipEntry entry = null;
			bool found = false;
			
			while ((entry = zipfile.GetNextEntry()) != null)
			{												
				if (Path.GetFileName(filename) == entry.Name)
				{
					found = true;
					break;
				}
			}
			
			if (found == true || entry != null)
			{
				FileStream file = File.Create(location);

				int size = 2048;
				byte[] data = new byte[2048];
				while (true)
				{
					size = zipfile.Read(data, 0, data.Length);
					if (size > 0)
						file.Write(data, 0, size);
					else
						break;

				}
				file.Close();
			}

			zipfile.Close();
			return true;
		}*/
	}
}
