using System;
using System.Collections;

namespace DowReplayManager.NET.Types
{
	/// <summary>
	/// Summary description for ChatType.
	/// </summary>
	public class ChatType
	{
		public class ChatMessage
		{
			public string Sender;
			public string Text;
			public int Time;
		}

		private ArrayList messages;
		public ChatMessage this[int i]
		{
			get
			{
				return (ChatMessage)messages[i];
			}
			set
			{
				messages[i] = value;
			}
		}

		public int Length
		{
			get
			{
				return messages.Count;
			}
		}


		public void AddMessage(string sender, string message, int ticks)
		{
			ChatMessage msg = new ChatMessage();
			msg.Sender = sender;
			msg.Text = message;
			msg.Time = ticks;
			
			messages.Add(msg);
		}

		public ChatType()
		{
			messages = new ArrayList();
		}
	}
}
