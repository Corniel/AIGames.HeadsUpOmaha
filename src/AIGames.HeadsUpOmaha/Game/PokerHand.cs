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
					straight5432Ace = o[0].Height == 14 & o[1].Height - o[4].Height == 3;
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
			if (table == null || table.Count() < 3 || table.Count() > 5 || table.Any(c => c.IsEmpty()))
			{
				throw new ArgumentException("The table should contain [3,5] cards.", "table");
			}
			if (hand == null || (hand.Count() != 2 && hand.Count() != 4) || hand.Any(c => c.IsEmpty()))
			{
				throw new ArgumentException("The hand should contain 2 or 4 cards.", "hand");
			}
			var best = PokerHand.Empty;

			var all = new List<Card>(hand);
			all.AddRange(table);

			var tMax = Bits.Flag[all.Count];

			var permutations = hand.Count() == 4 ? HeadsUpOmahaPermutations : HeadsUptOmahaOpponentPermutations;

			// Select 2 card from the first 4 (or 2), and 3 from the rest.
			foreach (var permutation in permutations)
			{
				// If whe have a table with less then 5 cards, stop earlier.
				if (permutation > tMax) { break; }

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

		/// <summary>Gets the winning chance of a hand.</summary>
		public static double GetWinningChanceHeadsUpOmaha(IEnumerable<Card> table, IEnumerable<Card> hand)
		{
			if (table == null || table.Count() < 3 || table.Count() > 5 || table.Any(c => c.IsEmpty()))
			{
				throw new ArgumentException("The table should contain [3,5] cards.", "table");
			}
			if (hand == null || hand.Count() != 4 || hand.Any(c => c.IsEmpty()))
			{
				throw new ArgumentException("The hand should contain 4 cards.", "hand");
			}

			var total = 0;
			var score = 0;

			var ownHand = CreateFromHeadsUpOmaha(table, hand);

			var other = Cards.Deck.ToList();
			foreach (var card in hand) { other.Remove(card); }
			foreach (var card in table) { other.Remove(card); }
			var tMax = Bits.Flag[other.Count];

			var permutations = TakeTwopponentCardPermutations;

			// Select 2 card from the options.
			foreach (var permutation in permutations)
			{
				// If whe have a table with less then 5 cards, stop earlier.
				if (permutation > tMax) { break; }

				var set = new List<Card>();

				for (var i = 0; i < other.Count; i++)
				{
					if ((permutation & Bits.Flag[i]) != 0)
					{
						set.Add(other[i]);
					}
				}

				var test = CreateFromHeadsUpOmaha(table, set);
				var compare = PokerHandComparer.Instance.Compare(test, ownHand);
				if (compare == 1)
				{
					score += 2;
				}
				else if (compare == 0)
				{
					score++;
				}
				total += 2;
			}
			return 1.0 - ((double)score / (double)total);
		}

		private static readonly ulong[] HeadsUpOmahaPermutations = new ulong[] { 115, 117, 118, 121, 122, 124, 179, 181, 182, 185, 186, 188, 211, 213, 214, 217, 218, 220, 227, 229, 230, 233, 234, 236, 307, 309, 310, 313, 314, 316, 339, 341, 342, 345, 346, 348, 355, 357, 358, 361, 362, 364, 403, 405, 406, 409, 410, 412, 419, 421, 422, 425, 426, 428, 451, 453, 454, 457, 458, 460 };
		private static readonly ulong[] HeadsUptOmahaOpponentPermutations = new ulong[] { 31, 47, 55, 59, 79, 87, 91, 103, 107, 115 };
		private static readonly ulong[] TakeTwopponentCardPermutations = new ulong[] { 3, 5, 6, 9, 10, 12, 17, 18, 20, 24, 33, 34, 36, 40, 48, 65, 66, 68, 72, 80, 96, 129, 130, 132, 136, 144, 160, 192, 257, 258, 260, 264, 272, 288, 320, 384, 513, 514, 516, 520, 528, 544, 576, 640, 768, 1025, 1026, 1028, 1032, 1040, 1056, 1088, 1152, 1280, 1536, 2049, 2050, 2052, 2056, 2064, 2080, 2112, 2176, 2304, 2560, 3072, 4097, 4098, 4100, 4104, 4112, 4128, 4160, 4224, 4352, 4608, 5120, 6144, 8193, 8194, 8196, 8200, 8208, 8224, 8256, 8320, 8448, 8704, 9216, 10240, 12288, 16385, 16386, 16388, 16392, 16400, 16416, 16448, 16512, 16640, 16896, 17408, 18432, 20480, 24576, 32769, 32770, 32772, 32776, 32784, 32800, 32832, 32896, 33024, 33280, 33792, 34816, 36864, 40960, 49152, 65537, 65538, 65540, 65544, 65552, 65568, 65600, 65664, 65792, 66048, 66560, 67584, 69632, 73728, 81920, 98304, 131073, 131074, 131076, 131080, 131088, 131104, 131136, 131200, 131328, 131584, 132096, 133120, 135168, 139264, 147456, 163840, 196608, 262145, 262146, 262148, 262152, 262160, 262176, 262208, 262272, 262400, 262656, 263168, 264192, 266240, 270336, 278528, 294912, 327680, 393216, 524289, 524290, 524292, 524296, 524304, 524320, 524352, 524416, 524544, 524800, 525312, 526336, 528384, 532480, 540672, 557056, 589824, 655360, 786432, 1048577, 1048578, 1048580, 1048584, 1048592, 1048608, 1048640, 1048704, 1048832, 1049088, 1049600, 1050624, 1052672, 1056768, 1064960, 1081344, 1114112, 1179648, 1310720, 1572864, 2097153, 2097154, 2097156, 2097160, 2097168, 2097184, 2097216, 2097280, 2097408, 2097664, 2098176, 2099200, 2101248, 2105344, 2113536, 2129920, 2162688, 2228224, 2359296, 2621440, 3145728, 4194305, 4194306, 4194308, 4194312, 4194320, 4194336, 4194368, 4194432, 4194560, 4194816, 4195328, 4196352, 4198400, 4202496, 4210688, 4227072, 4259840, 4325376, 4456448, 4718592, 5242880, 6291456, 8388609, 8388610, 8388612, 8388616, 8388624, 8388640, 8388672, 8388736, 8388864, 8389120, 8389632, 8390656, 8392704, 8396800, 8404992, 8421376, 8454144, 8519680, 8650752, 8912896, 9437184, 10485760, 12582912, 16777217, 16777218, 16777220, 16777224, 16777232, 16777248, 16777280, 16777344, 16777472, 16777728, 16778240, 16779264, 16781312, 16785408, 16793600, 16809984, 16842752, 16908288, 17039360, 17301504, 17825792, 18874368, 20971520, 25165824, 33554433, 33554434, 33554436, 33554440, 33554448, 33554464, 33554496, 33554560, 33554688, 33554944, 33555456, 33556480, 33558528, 33562624, 33570816, 33587200, 33619968, 33685504, 33816576, 34078720, 34603008, 35651584, 37748736, 41943040, 50331648, 67108865, 67108866, 67108868, 67108872, 67108880, 67108896, 67108928, 67108992, 67109120, 67109376, 67109888, 67110912, 67112960, 67117056, 67125248, 67141632, 67174400, 67239936, 67371008, 67633152, 68157440, 69206016, 71303168, 75497472, 83886080, 100663296, 134217729, 134217730, 134217732, 134217736, 134217744, 134217760, 134217792, 134217856, 134217984, 134218240, 134218752, 134219776, 134221824, 134225920, 134234112, 134250496, 134283264, 134348800, 134479872, 134742016, 135266304, 136314880, 138412032, 142606336, 150994944, 167772160, 201326592, 268435457, 268435458, 268435460, 268435464, 268435472, 268435488, 268435520, 268435584, 268435712, 268435968, 268436480, 268437504, 268439552, 268443648, 268451840, 268468224, 268500992, 268566528, 268697600, 268959744, 269484032, 270532608, 272629760, 276824064, 285212672, 301989888, 335544320, 402653184, 536870913, 536870914, 536870916, 536870920, 536870928, 536870944, 536870976, 536871040, 536871168, 536871424, 536871936, 536872960, 536875008, 536879104, 536887296, 536903680, 536936448, 537001984, 537133056, 537395200, 537919488, 538968064, 541065216, 545259520, 553648128, 570425344, 603979776, 671088640, 805306368, 1073741825, 1073741826, 1073741828, 1073741832, 1073741840, 1073741856, 1073741888, 1073741952, 1073742080, 1073742336, 1073742848, 1073743872, 1073745920, 1073750016, 1073758208, 1073774592, 1073807360, 1073872896, 1074003968, 1074266112, 1074790400, 1075838976, 1077936128, 1082130432, 1090519040, 1107296256, 1140850688, 1207959552, 1342177280, 1610612736, 2147483649, 2147483650, 2147483652, 2147483656, 2147483664, 2147483680, 2147483712, 2147483776, 2147483904, 2147484160, 2147484672, 2147485696, 2147487744, 2147491840, 2147500032, 2147516416, 2147549184, 2147614720, 2147745792, 2148007936, 2148532224, 2149580800, 2151677952, 2155872256, 2164260864, 2181038080, 2214592512, 2281701376, 2415919104, 2684354560, 3221225472, 4294967297, 4294967298, 4294967300, 4294967304, 4294967312, 4294967328, 4294967360, 4294967424, 4294967552, 4294967808, 4294968320, 4294969344, 4294971392, 4294975488, 4294983680, 4295000064, 4295032832, 4295098368, 4295229440, 4295491584, 4296015872, 4297064448, 4299161600, 4303355904, 4311744512, 4328521728, 4362076160, 4429185024, 4563402752, 4831838208, 5368709120, 6442450944, 8589934593, 8589934594, 8589934596, 8589934600, 8589934608, 8589934624, 8589934656, 8589934720, 8589934848, 8589935104, 8589935616, 8589936640, 8589938688, 8589942784, 8589950976, 8589967360, 8590000128, 8590065664, 8590196736, 8590458880, 8590983168, 8592031744, 8594128896, 8598323200, 8606711808, 8623489024, 8657043456, 8724152320, 8858370048, 9126805504, 9663676416, 10737418240, 12884901888, 17179869185, 17179869186, 17179869188, 17179869192, 17179869200, 17179869216, 17179869248, 17179869312, 17179869440, 17179869696, 17179870208, 17179871232, 17179873280, 17179877376, 17179885568, 17179901952, 17179934720, 17180000256, 17180131328, 17180393472, 17180917760, 17181966336, 17184063488, 17188257792, 17196646400, 17213423616, 17246978048, 17314086912, 17448304640, 17716740096, 18253611008, 19327352832, 21474836480, 25769803776, 34359738369, 34359738370, 34359738372, 34359738376, 34359738384, 34359738400, 34359738432, 34359738496, 34359738624, 34359738880, 34359739392, 34359740416, 34359742464, 34359746560, 34359754752, 34359771136, 34359803904, 34359869440, 34360000512, 34360262656, 34360786944, 34361835520, 34363932672, 34368126976, 34376515584, 34393292800, 34426847232, 34493956096, 34628173824, 34896609280, 35433480192, 36507222016, 38654705664, 42949672960, 51539607552, 68719476737, 68719476738, 68719476740, 68719476744, 68719476752, 68719476768, 68719476800, 68719476864, 68719476992, 68719477248, 68719477760, 68719478784, 68719480832, 68719484928, 68719493120, 68719509504, 68719542272, 68719607808, 68719738880, 68720001024, 68720525312, 68721573888, 68723671040, 68727865344, 68736253952, 68753031168, 68786585600, 68853694464, 68987912192, 69256347648, 69793218560, 70866960384, 73014444032, 77309411328, 85899345920, 103079215104, 137438953473, 137438953474, 137438953476, 137438953480, 137438953488, 137438953504, 137438953536, 137438953600, 137438953728, 137438953984, 137438954496, 137438955520, 137438957568, 137438961664, 137438969856, 137438986240, 137439019008, 137439084544, 137439215616, 137439477760, 137440002048, 137441050624, 137443147776, 137447342080, 137455730688, 137472507904, 137506062336, 137573171200, 137707388928, 137975824384, 138512695296, 139586437120, 141733920768, 146028888064, 154618822656, 171798691840, 206158430208, 274877906945, 274877906946, 274877906948, 274877906952, 274877906960, 274877906976, 274877907008, 274877907072, 274877907200, 274877907456, 274877907968, 274877908992, 274877911040, 274877915136, 274877923328, 274877939712, 274877972480, 274878038016, 274878169088, 274878431232, 274878955520, 274880004096, 274882101248, 274886295552, 274894684160, 274911461376, 274945015808, 275012124672, 275146342400, 275414777856, 275951648768, 277025390592, 279172874240, 283467841536, 292057776128, 309237645312, 343597383680, 412316860416, 549755813889, 549755813890, 549755813892, 549755813896, 549755813904, 549755813920, 549755813952, 549755814016, 549755814144, 549755814400, 549755814912, 549755815936, 549755817984, 549755822080, 549755830272, 549755846656, 549755879424, 549755944960, 549756076032, 549756338176, 549756862464, 549757911040, 549760008192, 549764202496, 549772591104, 549789368320, 549822922752, 549890031616, 550024249344, 550292684800, 550829555712, 551903297536, 554050781184, 558345748480, 566935683072, 584115552256, 618475290624, 687194767360, 824633720832, 1099511627777, 1099511627778, 1099511627780, 1099511627784, 1099511627792, 1099511627808, 1099511627840, 1099511627904, 1099511628032, 1099511628288, 1099511628800, 1099511629824, 1099511631872, 1099511635968, 1099511644160, 1099511660544, 1099511693312, 1099511758848, 1099511889920, 1099512152064, 1099512676352, 1099513724928, 1099515822080, 1099520016384, 1099528404992, 1099545182208, 1099578736640, 1099645845504, 1099780063232, 1100048498688, 1100585369600, 1101659111424, 1103806595072, 1108101562368, 1116691496960, 1133871366144, 1168231104512, 1236950581248, 1374389534720, 1649267441664, 2199023255553, 2199023255554, 2199023255556, 2199023255560, 2199023255568, 2199023255584, 2199023255616, 2199023255680, 2199023255808, 2199023256064, 2199023256576, 2199023257600, 2199023259648, 2199023263744, 2199023271936, 2199023288320, 2199023321088, 2199023386624, 2199023517696, 2199023779840, 2199024304128, 2199025352704, 2199027449856, 2199031644160, 2199040032768, 2199056809984, 2199090364416, 2199157473280, 2199291691008, 2199560126464, 2200096997376, 2201170739200, 2203318222848, 2207613190144, 2216203124736, 2233382993920, 2267742732288, 2336462209024, 2473901162496, 2748779069440, 3298534883328, 4398046511105, 4398046511106, 4398046511108, 4398046511112, 4398046511120, 4398046511136, 4398046511168, 4398046511232, 4398046511360, 4398046511616, 4398046512128, 4398046513152, 4398046515200, 4398046519296, 4398046527488, 4398046543872, 4398046576640, 4398046642176, 4398046773248, 4398047035392, 4398047559680, 4398048608256, 4398050705408, 4398054899712, 4398063288320, 4398080065536, 4398113619968, 4398180728832, 4398314946560, 4398583382016, 4399120252928, 4400193994752, 4402341478400, 4406636445696, 4415226380288, 4432406249472, 4466765987840, 4535485464576, 4672924418048, 4947802324992, 5497558138880, 6597069766656, 8796093022209, 8796093022210, 8796093022212, 8796093022216, 8796093022224, 8796093022240, 8796093022272, 8796093022336, 8796093022464, 8796093022720, 8796093023232, 8796093024256, 8796093026304, 8796093030400, 8796093038592, 8796093054976, 8796093087744, 8796093153280, 8796093284352, 8796093546496, 8796094070784, 8796095119360, 8796097216512, 8796101410816, 8796109799424, 8796126576640, 8796160131072, 8796227239936, 8796361457664, 8796629893120, 8797166764032, 8798240505856, 8800387989504, 8804682956800, 8813272891392, 8830452760576, 8864812498944, 8933531975680, 9070970929152, 9345848836096, 9895604649984, 10995116277760, 13194139533312, 17592186044417, 17592186044418, 17592186044420, 17592186044424, 17592186044432, 17592186044448, 17592186044480, 17592186044544, 17592186044672, 17592186044928, 17592186045440, 17592186046464, 17592186048512, 17592186052608, 17592186060800, 17592186077184, 17592186109952, 17592186175488, 17592186306560, 17592186568704, 17592187092992, 17592188141568, 17592190238720, 17592194433024, 17592202821632, 17592219598848, 17592253153280, 17592320262144, 17592454479872, 17592722915328, 17593259786240, 17594333528064, 17596481011712, 17600775979008, 17609365913600, 17626545782784, 17660905521152, 17729624997888, 17867063951360, 18141941858304, 18691697672192, 19791209299968, 21990232555520, 26388279066624 };
	}
}
