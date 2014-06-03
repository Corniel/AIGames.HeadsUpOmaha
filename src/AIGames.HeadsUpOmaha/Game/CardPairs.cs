using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>Represents pairs based on a set of cards.</summary>
	[DebuggerDisplay("{DebugToString()}")]
	public struct CardPairs
	{
		private int[] lookup;

		/// <summary>Gets the number of cards with the specified height.</summary>
		/// <param name="height"></param>
		public int this[int height] { get { return lookup[height]; } }

		/// <summary>Get the best card.</summary>
		public int Best { get { return GetBest(0); } }
		/// <summary>Gets the second best card.</summary>
		public int Second { get { return GetBest(1); } }
		
		/// <summary>Get the nth best card.</summary>
		public int GetBest(int pos)
		{
			var p = 0;
			for (var height = 14; height >= 0; height--)
			{
				p += lookup[height];
				if (p > pos) { return height; }
			}
			return 0;
		}

		/// <summary>Gets the highest number of pairs.</summary>
		public int Max { get { return lookup.Max(); } }

		/// <summary>Represents a card as a debug string.</summary>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		private string DebugToString()
		{
			if (lookup == null || Max == 0) { return "Pairs: {empty}"; }
			return String.Format(
				"Pairs: Max: {0}, Best: {1}, Second: {2}",
				this.Max,
				Numbers[this.Best],
				Numbers[this.Second]
			);
		}
		private const string Numbers = " ?23456789TJQKA";

		/// <summary>Creates card pairs based on set of cards.</summary>
		/// <param name="cards">
		/// The cards to determine the pair for.
		/// </param>
		public static CardPairs Create(IEnumerable<Card> cards)
		{
			var pairs = new CardPairs()
			{
				// [2] = 2, [14] = Ace
				lookup =  new int[15]
			};

			foreach (var c in cards)
			{
				pairs.lookup[c.Height]++;
			}

			return pairs;
		}
	}
}
