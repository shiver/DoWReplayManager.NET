using System;
using System.IO;

namespace DowReplayManager.NET.Handlers
{

	public enum LogType
	{
		Info,
		Error
	}
	/// <summary>
	/// Summary description for Logging.
	/// </summary>
	public class Logging
	{
		StreamWriter writer = null;

		private string filename = null;
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

		private int loglevel = 0;
		public int LogLevel
		{
			get
			{
				return loglevel;
			}
			set
			{
				loglevel = value;
			}
		}

		public Logging()
		{
		}
		
		public Logging(string filename)
		{
			this.filename = filename;
		}

		public bool Open()
		{
			try
			{
				if (!File.Exists(filename))
					writer = new StreamWriter(File.Create(filename));
				else
					writer = new StreamWriter(File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
				
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Write(LogType Type, int LogLevel, string Text)
		{
			try
			{
				if (LogLevel <= loglevel)
				{
					writer.WriteLine("--" + Type + " " + DateTime.Now.ToString("hh:mm:ss dd/MM/yyyy") + " : " + Text);
					writer.Flush();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Close()
		{
			writer.Close();
		}
	}
}
