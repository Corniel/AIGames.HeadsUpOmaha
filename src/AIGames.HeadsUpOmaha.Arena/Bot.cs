﻿using System;
using System.Diagnostics;

namespace AIGames.HeadsUpOmaha.Arena
{
	[Serializable]
	[DebuggerDisplay("{DebugToString()}")]
	public class Bot: IComparable, IComparable<Bot>
	{
		public Bot()
		{
            Rating = 1400.0;
			K = 32.0;
		}
		public BotInfo Info { get; set; }
		public int Wins { get; set; }
		public int Draws { get; set; }
		public int Losses { get; set; }
        public Elo Rating { get; set; }
		public double K { get; set; }

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public string DebugToString()
		{
			return String.Format("Bot: {0}, Version: {1}{2}, Elo: {3:0000}, K: {4:0.0}, W: {5}, D: {6}, L: {7}",
				Info.Name,
				Info.Version,
				Info.Inactive ? ", Inactive" : "",
                Rating,
				K,
				Wins,
				Draws,
				Losses);
		}

		public int CompareTo(object obj)
		{
			return CompareTo(obj as Bot);
		}

		public int CompareTo(Bot other)
		{
			return -this.Rating.CompareTo(other.Rating);
		}
	}
}
