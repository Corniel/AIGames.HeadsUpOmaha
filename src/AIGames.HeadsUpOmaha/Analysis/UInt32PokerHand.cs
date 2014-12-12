using AIGames.HeadsUpOmaha.Game;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AIGames.HeadsUpOmaha.Analysis
{
	/// <summary>Represents the pokerhand.</summary>
	/// <remarks>
	/// A pokerhand is a set of five cards with some score.
	/// </remarks>
	[DebuggerDisplay("{DebuggerDisplay}")]
	public struct UInt32PokerHand : IComparable, IComparable<UInt32PokerHand>
	{
		private uint mask;

		/// <summary>Gets the score type.</summary>
		public PokerHandType ScoreType { get { return (PokerHandType)(mask >> 28); } }

		/// <summary>Compares the pokerhand to the object.</summary>
		/// <remarks>
		/// This should only be used to compare with anouther pokerhand, the cast is
		/// done because of the speed.
		/// </remarks>
		public int CompareTo(object obj)
		{
			return CompareTo((UInt32PokerHand)obj);
		}

		/// <summary>Compares this pokerhand to the other.</summary>
		public int CompareTo(UInt32PokerHand other)
		{
			return mask.CompareTo(other.mask);
		}

		[ExcludeFromCodeCoverage, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				var tp = ScoreType;
				var p2 = tp == PokerHandType.TwoPair;
				var high = (mask >> (p2 ? 0 : 13)) & Bits.Mask13;

				var s = mask ^ ((uint)tp << 28);

				var cards = new UInt64Cards((mask << (p2 ? 4 : 0)) & Bits.Mask13).ToCards().OrderByDescending(c => c).ToList();

				var low = String.Join(",", cards.Select(c => c.Height));

				switch (tp)
				{
					case PokerHandType.FourOfAKind:
					case PokerHandType.OnePair:
					case PokerHandType.ThreeOfAKind:
					case PokerHandType.FullHouse:
						return string.Format("{0}: {1} [{2}], {3}", tp, high, low, s);

					case PokerHandType.TwoPair:
						return string.Format("{0}: {2} [{1}], {3}", tp, high, low, s);

					case PokerHandType.Straight:
					case PokerHandType.StraightFlush:
						return string.Format("{0}: {1}, {2}", tp, high, s);

					case PokerHandType.Flush:
					case PokerHandType.HighCard:
					default:
						return string.Format("{0}: {1}, {2}", tp, low, s);
				}
			}
		}

		/// <summary>Compares two hands.</summary>
		public static int Compare(UInt64Cards[] l, UInt64Cards[] r, UInt64Cards[] tbl)
		{
			var lBest = default(UInt32PokerHand);
			var rBest = default(UInt32PokerHand);

			for (int h = 0; h < 6; h++)
			{
				for (int t = 0; t < 10; t++)
				{
					var lTest = tbl[t].Combine(l[h]).GetScore();
					var rTest = tbl[t].Combine(r[h]).GetScore();

					if (lTest.CompareTo(lBest) > 0)
					{
						lBest = lTest;
					}

					if (rTest.CompareTo(rBest) > 0)
					{
						rBest = rTest;
					}
				}
			}
			return lBest.CompareTo(rBest);
		}

		/// <summary>Creates a pokerhand.</summary>
		public static UInt32PokerHand Create(PokerHandType tp, uint mask)
		{
			var hand = new UInt32PokerHand();
			hand.mask = ((uint)tp << 28) | mask;
			return hand;
		}
	}
}
