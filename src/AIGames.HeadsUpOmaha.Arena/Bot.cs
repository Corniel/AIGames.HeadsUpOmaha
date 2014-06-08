using System;
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

		public string FullName
		{
			get { return string.Format("{0} {1}", this.Info.Name, this.Info.Version); }
		}

		public BotInfo Info { get; set; }
		public int Wins { get; set; }
		public int Draws { get; set; }
		public int Losses { get; set; }
		public int Games { get { return this.Wins + this.Draws + this.Losses; } }
		public double Score 
		{
			get
			{
				if (this.Games == 0) { return double.NaN; }

				return ((double)this.Wins + this.Draws * .5) / (double)this.Games;
			}
		}
        public Elo Rating { get; set; }
		public double K { get; set; }

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public string DebugToString()
		{
			return String.Format("Bot: {0}, Version: {1}{2}, Elo: {3:0000}, K: {4:0.0}, W: {5}, D: {6}, L: {7} ({8:0.0%})",
				Info.Name,
				Info.Version,
				Info.Inactive ? ", Inactive" : "",
                Rating,
				K,
				Wins,
				Draws,
				Losses,
				Score);
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
