using System.Collections.Generic;

namespace AIGames.HeadsUpOmaha.Arena
{
	public class BotComparer : IComparer<Bot>
	{
		public BotComparer (BotCompareType compareType)
		{
			this.CompareType = compareType;
		}
		public BotCompareType CompareType { get; protected set; }

		public int Compare(Bot x, Bot y)
		{
			switch (this.CompareType)
			{
				case BotCompareType.Percentage: return y.Score.CompareTo(x.Score);
				case BotCompareType.Elo:
				default: return y.Rating.CompareTo(x.Rating);
			}
		}
	}
}
