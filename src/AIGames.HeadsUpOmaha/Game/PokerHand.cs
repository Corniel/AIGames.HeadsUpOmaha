using AIGames.HeadsUpOmaha;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>Represents a poker hand.</summary>
	[Serializable, DebuggerDisplay("{DebugToString()}")]
	public struct PokerHand : ISerializable, IXmlSerializable
	{
		/// <summary>The number of possible hands.</summary>
		public const int HandTotal = 2598960;

		/// <summary>The number of possible hands of a certain type.</summary>
		public static readonly Dictionary<PokerHandType, int> HandSubTotal = new Dictionary<PokerHandType, int>()
		{
			{ PokerHandType.StraightFlush, 40 },
			{ PokerHandType.FourOfAKind, 624 },
			{ PokerHandType.FullHouse,  3744 },
			{ PokerHandType.Flush, 5108 },
			{ PokerHandType.Straight, 10200 },
			{ PokerHandType.ThreeOfAKind, 54912 },
			{ PokerHandType.TwoPair, 123552 },
			{ PokerHandType.OnePair, 1098240 },
			{ PokerHandType.HighCard, 1302540 },
		};

		/// <summary>Represents a empty hand.</summary>
		public static readonly PokerHand Empty = default(PokerHand);

		#region properties

		private PokerHandType tp;
		private Card c0;
		private Card c1;
		private Card c2;
		private Card c3;
		private Card c4;

		/// <summary>Gets the score type.</summary>
		public PokerHandType ScoreType { get { return tp; } }

		/// <summary>Gets the card based on its index.</summary>
		public Card this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return c0;
					case 1: return c1;
					case 2: return c2;
					case 3: return c3;
					case 4: return c4;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		#endregion

		#region (XML) (De)serialization

		/// <summary>Initializes a new instance of  poker hand based on the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		private PokerHand(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			tp = (PokerHandType)info.GetInt32("tp");
			c0 = (Card)info.GetValue("c0", typeof(Card));
			c1 = (Card)info.GetValue("c1", typeof(Card));
			c2 = (Card)info.GetValue("c2", typeof(Card));
			c3 = (Card)info.GetValue("c3", typeof(Card));
			c4 = (Card)info.GetValue("c4", typeof(Card));
		}

		/// <summary>Adds the underlying propererty of  poker hand to the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			info.AddValue("tp", tp);
			info.AddValue("c0", c0);
			info.AddValue("c1", c1);
			info.AddValue("c2", c2);
			info.AddValue("c3", c3);
			info.AddValue("c4", c4);
		}

		/// <summary>Gets the xml schema to (de) xml serialize a poker hand.</summary>
		/// <remarks>
		/// Returns null as no schema is required.
		/// </remarks>
		XmlSchema IXmlSerializable.GetSchema() { return null; }

		/// <summary>Reads the  poker hand from an xml writer.</summary>
		/// <remarks>
		/// Uses the string parse set function of card.
		/// </remarks>
		/// <param name="reader">An xml reader.</param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			var s = reader.ReadElementString();
			var val = CreateFrom5(Cards.Parse(s));
			tp = val.tp;
			c0 = val.c0;
			c1 = val.c1;
			c2 = val.c2;
			c3 = val.c3;
			c4 = val.c4;
		}

		/// <summary>Writes the poker hand to an xml writer.</summary>
		/// <remarks>
		/// Uses the string representation of poker hand.
		/// </remarks>
		/// <param name="writer">An xml writer.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString());
		}

		#endregion

		/// <summary>Returs true if poker hand represents the empty value, otherwise false.</summary>
		public bool IsEmpty() { return Object.Equals(this, Empty); }

		#region Tostring

		/// <summary>Represents a card as a string.</summary>
		public override string ToString()
		{
			if (IsEmpty()) { return string.Empty; }
			return string.Format("[{0},{1},{2},{3},{4}]", c0, c1, c2, c3, c4);
		}

		/// <summary>Represents a card as a debug string.</summary>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		private string DebugToString()
		{
			if (IsEmpty()) { return "{empty}"; }

			return string.Format("[{0:f},{1:f},{2:f},{3:f},{4:f}] {5}",
				c0, c1, c2, c3, c4, tp);
		}

		#endregion

		#region Equality

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">
		/// Another object to compare to.
		/// </param>
		/// <returns>
		/// true if obj and this card are the same type and represent the same value;
		/// otherwise, false.
		/// </returns>
		public override bool Equals(object obj) { return base.Equals(obj); }

		/// <summary>Returns the hash code for this card.</summary>
		public override int GetHashCode()
		{
			return
				tp.GetHashCode() ^
				(c0.GetHashCode() << 03) ^
				(c1.GetHashCode() << 09) ^
				(c2.GetHashCode() << 14) ^
				(c3.GetHashCode() << 20) ^
				(c4.GetHashCode() << 26);
		}

		/// <summary>Returns true if left equals right, otherwise false.</summary>
		public static bool operator ==(PokerHand l, PokerHand r) { return l.Equals(r); }

		/// <summary>Returns false if left equals right, otherwise true.</summary>
		public static bool operator !=(PokerHand l, PokerHand r) { return !(l == r); }

		#endregion

		/// <summary>Creates a poker hand score from 5 cards.</summary>
		/// <param name="cards">The 5 cards to get a poker hand from.</param>
		/// <see cref="http://en.wikipedia.org/wiki/List_of_poker_hands"/>
		public static PokerHand CreateFrom5(IEnumerable<Card> cards)
		{
			if (cards == null || cards.Count() != 5 || cards.Any(c => c.IsEmpty()))
			{
				throw new ArgumentException("Five cards are required.", "card");
			}

			var straight5432Ace = false;

			var score = PokerHandType.HighCard;

			var pairs = CardPairs.Create(cards);
			var sets = pairs.Max;

			var o = cards
				.OrderByDescending(c => c)
				.OrderByDescending(c => pairs[c.Height])
				.ToList();

			switch (sets)
			{
				case 4: score = PokerHandType.FourOfAKind; break;
				case 3: score = o[3].Height == o[4].Height ? PokerHandType.FullHouse : PokerHandType.ThreeOfAKind; break;
				case 2: score = o[2].Height == o[3].Height ? PokerHandType.TwoPair : PokerHandType.OnePair; break;
				default:
					var isFlush = o.All(c => o[0].Suit == c.Suit);

					// Ace, 5,4,3,2
					straight5432Ace = o[0].Height == 14 && o[1].Height == 5;
					var straight = straight5432Ace || (o[0].Height - o[4].Height == 4);

					if (straight)
					{
						score = isFlush ? PokerHandType.StraightFlush : PokerHandType.Straight;
					}
					else if (isFlush)
					{
						score = PokerHandType.Flush;
					}
					break;
			}

			return new PokerHand()
			{
				tp = score,
				c0 = o[straight5432Ace ? 1 : 0],
				c1 = o[straight5432Ace ? 2 : 1],
				c2 = o[straight5432Ace ? 3 : 2],
				c3 = o[straight5432Ace ? 4 : 3],
				c4 = o[straight5432Ace ? 0 : 4],
			};
		}

		/// <summary>Creates the best possible hand based on the best 3 cards from
		/// the table and the best 2 from the hand.
		/// </summary>
		public static PokerHand CreateFromHeadsUpOmaha(IEnumerable<Card> table, IEnumerable<Card> hand)
		{
			if (table == null || table.Count() != 5 || table.Any(c => c.IsEmpty()))
			{
				throw new ArgumentException("The table should contain 5 cards.", "table");
			}
			if (hand == null || hand.Count() != 4 || hand.Any(c => c.IsEmpty()))
			{
				throw new ArgumentException("The hand should contain 4 cards.", "hand");
			}
			var best = PokerHand.Empty;

			var all = new List<Card>(hand);
			all.AddRange(table);

			// Select 2 card from the first 4 (or 2), and 3 from the rest.
			foreach (var permutation in HeadsUpOmahaPermutations)
			{
				var set = new List<Card>();

				for (var i = 0; i < all.Count; i++)
				{
					if ((permutation & Bits.Flag[i]) != 0)
					{
						set.Add(all[i]);
					}
				}
				var test = CreateFrom5(set);

				if (PokerHandComparer.Instance.Compare(test, best) > 0)
				{
					best = test;
				}
			}
			return best;
		}

		private static readonly ulong[] HeadsUpOmahaPermutations = new ulong[] { 115, 117, 118, 121, 122, 124, 179, 181, 182, 185, 186, 188, 211, 213, 214, 217, 218, 220, 227, 229, 230, 233, 234, 236, 307, 309, 310, 313, 314, 316, 339, 341, 342, 345, 346, 348, 355, 357, 358, 361, 362, 364, 403, 405, 406, 409, 410, 412, 419, 421, 422, 425, 426, 428, 451, 453, 454, 457, 458, 460 };
	}
}
