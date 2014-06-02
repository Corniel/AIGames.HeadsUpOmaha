using System.Collections.Generic;

namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>Compares two poker hands.</summary>
	public class PokerHandComparer : IEqualityComparer<PokerHand>, IComparer<PokerHand>
	{
		/// <summary>The singleton instance.</summary>
		public static readonly PokerHandComparer Instance = new PokerHandComparer();

		/// <summary>Constructor.</summary>
		/// <remarks>
		/// Use the singleton.
		/// </remarks>
		private PokerHandComparer() { }

		/// <summary>Compares two hands.</summary>
		public int Compare(PokerHand x, PokerHand y)
		{
			var compare = x.ScoreType.CompareTo(y.ScoreType);
			for (int i = 0; i < 5 && compare == 0; i++)
			{
				compare = x[i].Height.CompareTo(y[i].Height);
			}
			return compare;
		}
		/// <summary>Returns true if two hands are equaly strong, otherwise false.</summary>
		public bool Equals(PokerHand x, PokerHand y)
		{
			return Compare(x, y) == 0;
		}

		/// <summary>Gets a hash of a hand.</summary>
		public int GetHashCode(PokerHand obj)
		{
			return
				obj.ScoreType.GetHashCode() ^
					(obj[0].Height << 03) ^
					(obj[1].Height << 09) ^
					(obj[2].Height << 14) ^
					(obj[3].Height << 20) ^
					(obj[4].Height << 26);
		}
	}
}
