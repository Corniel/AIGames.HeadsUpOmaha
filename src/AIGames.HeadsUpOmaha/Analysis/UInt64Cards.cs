using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.Analysis
{
	/// <summary>Represents an unsorted deck of cards as UInt64.</summary>
	[DebuggerDisplay("{DebugToString()}")]
	public partial struct UInt64Cards
	{
		/// <summary>Gets masks for (5 3).</summary>
		public static readonly byte[] Mask5Over3 = { 7,11,13,14,19,21,22,25,26,28 };

		/// <summary>Gets masks for (4 2).</summary>
		public static readonly byte[] Mask4Over2 = { 3,5,6,9,10,12 };

		/// <summary>Gets all cards.</summary>
		public static readonly UInt64Cards All = new UInt64Cards(Bits.Flag[52] - 1);

		private static readonly int[] SuitShift = new int[] { 0, 13, 26, 39 };

		/// <summary>Constructor.</summary>
		public UInt64Cards(ulong m) { mask = m; }

		private ulong mask;

		/// <summary>Returns true if the cards are all one suit.</summary>
		public bool IsFlush
		{
			get
			{
				return
					Bits.Count(CardMask(mask, CardSuit.Diamonds)) == 5 ||
					Bits.Count(CardMask(mask, CardSuit.Clubs)) == 5 ||
					Bits.Count(CardMask(mask, CardSuit.Hearts)) == 5 ||
					Bits.Count(CardMask(mask, CardSuit.Spades)) == 5;
			}
		}

		/// <summary>Gets the number of cards.</summary>
		public int Count { get { return Bits.Count(mask); } }

		/// <summary>Remove selection from hand.</summary>
		public UInt64Cards Invert()
		{
			return new UInt64Cards(~mask);
		}

		/// <summary>Remove selection from hand.</summary>
		public UInt64Cards Remove(UInt64Cards selection)
		{
			return new UInt64Cards(mask ^ selection.mask);
		}
		/// <summary>Remove selection from hand.</summary>
		public UInt64Cards Combine(UInt64Cards selection)
		{
			return new UInt64Cards(mask | selection.mask);
		}

		/// <summary>Remove selection from hand.</summary>
		public UInt64Cards Combine(UInt64Cards s0, UInt64Cards s1)
		{
			return new UInt64Cards(mask | s0.mask | s1.mask);
		}

		/// <summary>Gets the score of a the cards.</summary>
		public UInt32PokerHand GetScore()
		{
#if DEBUG
			if (this.Count != 5) { throw new InvalidOperationException("The number of cards is not 5."); }
#endif
			uint rankX, mask1, mask2, mask3, mask4;

			var sd = CardMask(mask, CardSuit.Diamonds);
			var sc = CardMask(mask, CardSuit.Clubs);
			var sh = CardMask(mask, CardSuit.Hearts);
			var ss = CardMask(mask, CardSuit.Spades);

			var mask5 = sc | sd | sh | ss;
			var mask5Count = Bits.Count(mask5);

			switch (mask5Count)
			{
				// FourOfAKind or FullHouse
				case 2:
					mask4 = sh & sd & sc & ss;
					if (mask4 != 0)
					{
						rankX = GetSingleValue(mask4) | (mask5 ^ mask4);
						return UInt32PokerHand.Create(PokerHandType.FourOfAKind, rankX);
					}
					else
					{
						mask3 = ((sc & sd) | (sh & ss)) & ((sc & sh) | (sd & ss));
						rankX = GetSingleValue(mask3) | (mask5 ^ mask3);
						return UInt32PokerHand.Create(PokerHandType.FullHouse, rankX);
					}
				
				// Three of a kind, or two pair.
				case 3:
					mask2 = mask5 ^ (sc ^ sd ^ sh ^ ss);
					if (mask2 != 0)
					{
						rankX = mask2 << 4;
						mask1 = mask5 ^ mask2;
						rankX |= GetSingleValue(mask1) >> 13;
						return UInt32PokerHand.Create(PokerHandType.TwoPair, rankX);
					}
					else
					{
						mask3 = ((sc & sd) | (sh & ss)) & ((sc & sh) | (sd & ss));
						rankX = GetSingleValue(mask3) | (mask5 ^ mask3);
						return UInt32PokerHand.Create(PokerHandType.ThreeOfAKind, rankX);
					}
				// One pair.
				case 4:
					mask2 = mask5 ^ (sc ^ sd ^ sh ^ ss);
					rankX = GetSingleValue(mask2) | (mask5 ^ mask2);
					return UInt32PokerHand.Create(PokerHandType.OnePair, rankX);
				// Straight and or flush, or high card.
				case 5:
				default:
					rankX = GetStraightValue(mask5);
					if(rankX != 0)
					{
						return UInt32PokerHand.Create(IsFlush ? PokerHandType.StraightFlush : PokerHandType.Straight, rankX);
					}
					if (IsFlush)
					{
						return UInt32PokerHand.Create(PokerHandType.Flush, mask5);
					}
					return UInt32PokerHand.Create(PokerHandType.HighCard, mask5);
			}
		}
		/// <summary>Gets the single value of a mask shifted 13 places.</summary>
		internal uint GetSingleValue(uint m)
		{
			switch (m)
			{
				case 0x0001: return 02 << 13;
				case 0x0002: return 03 << 13;
				case 0x0004: return 04 << 13;
				case 0x0008: return 05 << 13;
				case 0x0010: return 06 << 13;
				case 0x0020: return 07 << 13;
				case 0x0040: return 08 << 13;
				case 0x0080: return 09 << 13;
				case 0x0100: return 10 << 13;
				case 0x0200: return 11 << 13;
				case 0x0400: return 12 << 13;
				case 0x0800: return 13 << 13;
				case 0x1000: return 14 << 13;
				default: return 0;
			}
		}
		internal uint GetStraightValue(uint mask)
		{
			switch (mask)
			{
				case 0x001f: return 05;
				case 0x003e: return 06;
				case 0x007c: return 07;
				case 0x00f8: return 08;
				case 0x01f0: return 09;
				case 0x03e0: return 10;
				case 0x07c0: return 11;
				case 0x0f80: return 12;
				case 0x1f00: return 13;
				// 4321A
				case 0x100f: return 04;
				
				default: return 0;
			}
		}

		/// <summary>Gets a sub set of hands of the deck.</summary>
		public UInt64Cards[] GetHandSubsets()
		{
			var subsets = new UInt64Cards[6];
			var indexes = GetMaskIndexes(4);

			for (var i = 0; i < 6; i++)
			{
				ulong s = 0;
				for (var p = 0; p < 5; p++)
				{
					if ((Bits.Flag[p] & Mask4Over2[i]) != 0)
					{
						s |= indexes[p];
					}
				}
				subsets[i] = new UInt64Cards(s);
			}
			return subsets;
		}

		/// <summary>Gets a sub set of tables of the deck.</summary>
		public UInt64Cards[] GetTableSubsets()
		{
			var subsets = new UInt64Cards[10];
			var indexes = GetMaskIndexes(5);

			for (var i = 0; i < 10; i++)
			{
				ulong s = 0;
				for (var p = 0; p < 6; p++)
				{
					if ((Bits.Flag[p] & Mask5Over3[i]) != 0)
					{
						s |= indexes[p];
					}
				}
				subsets[i] = new UInt64Cards(s);
			}
			return subsets;
		}

		/// <summary>Converts the UInt64Cards to cards.</summary>
		public Cards ToCards()
		{
			var cards = new Cards();
			for (int i = 0; i < 52; i++)
			{
				if ((Bits.Flag[i] & mask) != 0)
				{
					cards.Add(Card.FromIndex(i));
				}
			}
			return cards;
		}

		/// <summary>Represents a card as a string.</summary>
		public override string ToString()
		{
			return ToCards().ToString();
		}

		/// <summary>Represents a card as a debug string.</summary>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		private string DebugToString()
		{
			if (Bits.Count(mask) == 0) { return "{empty}"; }
			return ToCards().ToString("f");
		}

		private static uint CardMask(ulong cards, CardSuit suit)
		{
			return (uint)((cards >> SuitShift[(int)suit]) & Bits.Mask13);
		}

		/// <summary>Creates UInt64Cards from a set of cards.</summary>
		public static UInt64Cards Create(IEnumerable<Card> cards)
		{
			ulong m = 0;

			foreach (var c in cards)
			{
				m |= Bits.Flag[c.Index];
			};

			return new UInt64Cards(m);
		}

		/// <summary>Parses a set cards and returns UInt64Cards.</summary>
		public static UInt64Cards Parse(string str)
		{
			var cards = Cards.Parse(str);
			return Create(cards);
		}

		/// <summary>Returns a random hand with the specified number of cards and constrained
		/// to not contain any of the passed dead cards.
		/// </summary>
		/// <param name="dead">Mask for the cards that must not be returned.</param>
		/// <param name="ncards">The number of cards to return in this hand.</param>
		/// <param name="rnd">An instance of the Random class.</param>
		/// <returns>A randomly chosen hand containing the number of cards requested.</returns>
		public static UInt64Cards GetRandom(UInt64Cards dead, int ncards, MT19937Generator rnd)
		{
			// Return a random hand.
			ulong mask = 0UL, card;

			for (int i = 0; i < ncards; i++)
			{
				do
				{
					card = Bits.Flag[rnd.Next(52)];
				} 
				while (((dead.mask | mask) & card) != 0);
				
				mask |= card;
			}

			return new UInt64Cards(mask);
		}

		/// <summary>Creates sub hands.</summary>
		public static UInt64Cards[] CreateSubHands(Cards hand)
		{
			var supHands = new UInt64Cards[6];

			for (int i = 0; i < 6; i++)
			{
				var subset = new List<Card>();
				for (var p = 0; p < 4; p++)
				{
					if ((Bits.Flag[p] & Mask4Over2[i]) != 0)
					{
						subset.Add(hand[p]);
					}
				}
				supHands[i] = UInt64Cards.Create(subset);
			}
			return supHands;
		}

		private ulong[] GetMaskIndexes(int bitCount)
		{
			var indexes = new ulong[bitCount];
			var p = 0;
			for (byte i = 0; i < 52 & p < bitCount; i++)
			{
				if ((mask & Bits.Flag[i]) != 0)
				{
					indexes[p++] = Bits.Flag[i];
				}
			}
			return indexes;
		}
	}
}
