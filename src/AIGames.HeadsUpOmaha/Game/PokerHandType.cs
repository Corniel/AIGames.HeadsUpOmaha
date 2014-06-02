namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>The type of a poker hand.</summary>
	public enum PokerHandType
	{
		/// <summary>High card.</summary>
		HighCard = 0,
		/// <summary>One pair.</summary>
		OnePair = 1,
		/// <summary>Two pair(s).</summary>
		TwoPair = 2,
		/// <summary>Three of a kind.</summary>
		ThreeOfAKind = 3,
		/// <summary>Straight.</summary>
		Straight = 4,
		/// <summary>Flush.</summary>
		Flush = 5,
		/// <summary>Full house.</summary>
		FullHouse = 6,
		/// <summary>Four of a kind.</summary>
		FourOfAKind = 7,
		/// <summary>Straight flush.</summary>
		StraightFlush = 8,
	}
}
