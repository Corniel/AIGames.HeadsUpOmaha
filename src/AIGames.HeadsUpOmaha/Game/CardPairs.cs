using System.Collections.Generic;
using System.Linq;

namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>Represents pairs based on a set of cards.</summary>
	public struct CardPairs
	{
		/// <summary>Represents an empty set of card pairs.</summary>
		public static readonly CardPairs Empty = default(CardPairs);

		private int[] lookup;

		/// <summary>Gets the number of cards with the specified height.</summary>
		/// <param name="height"></param>
		public int this[int height] { get { return lookup[height]; } }

		/// <summary>Gets the highest number of pairs.</summary>
		public int Max { get { return lookup.Max(); } }

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
