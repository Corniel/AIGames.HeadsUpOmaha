using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AIGames.HeadsUpOmaha.Platform
{
	/// <summary>Represents an instruction.</summary>
	[Serializable]
	public struct Instruction : ISerializable, IXmlSerializable
	{
		/// <summary>Represents an empty instruction.</summary>
		public static readonly Instruction Empty = default(Instruction);

		/// <summary>Instruction 'Engine says: "player1 finished 1"'.</summary>
		public static readonly Instruction Player1Finished1 = Parse("Engine says: \"player1 finished 1\"");
		/// <summary>Instruction 'Engine says: "player2 finished 1"'.</summary>
		public static readonly Instruction Player2Finished1 = Parse("Engine says: \"player2 finished 1\"");
		/// <summary>Instruction 'Engine says: "player1 finished 2"'.</summary>
		public static readonly Instruction Player1Finished2 = Parse("Engine says: \"player1 finished 2\"");
		/// <summary>Instruction 'Engine says: "player2 finished 2"'.</summary>
		public static readonly Instruction Player2Finished2 = Parse("Engine says: \"player2 finished 2\"");

		#region (XML) (De)serialization

		/// <summary>Initializes a new instance of card based on the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		private Instruction(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			token0 = info.GetString("token0");
			token1 = info.GetString("token1");
			token2 = info.GetValue("token2", typeof(object));
		}

		/// <summary>Adds the underlying propererty of card to the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			info.AddValue("token0", token0);
			info.AddValue("token1", token1);
			info.AddValue("token2", token2);
		}

		/// <summary>Gets the xml schema to (de) xml serialize a card.</summary>
		/// <remarks>
		/// Returns null as no schema is required.
		/// </remarks>
		XmlSchema IXmlSerializable.GetSchema() { return null; }

		/// <summary>Reads the card from an xml writer.</summary>
		/// <remarks>
		/// Uses the string parse function of card.
		/// </remarks>
		/// <param name="reader">An xml reader.</param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			var s = reader.ReadElementString();
			var val = Parse(s);
			token0 = val.token0;
			token1 = val.token1;
			token2 = val.token2;
		}

		/// <summary>Writes the card to an xml writer.</summary>
		/// <remarks>
		/// Uses the string representation of card.
		/// </remarks>
		/// <param name="writer">An xml writer.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString());
		}

		#endregion

		#region Properties

		/// <summary>Gets the token type.</summary>
		/// <remarks>
		/// The instruction type is token0.
		/// </remarks>
		public InstructionType InstructionType
		{
			get
			{
				switch (token0)
				{
					case null: return Platform.InstructionType.None;
					case "Action": return Platform.InstructionType.Action;
					case "Match": return Platform.InstructionType.Match;
					case "Output": return Platform.InstructionType.Output;
					case "Settings": return Platform.InstructionType.Settings;
					default:
						return token0.StartsWith("player") ? Platform.InstructionType.Player : Platform.InstructionType.Output;
				}
			}
		}

		/// <summary>Gets the action.</summary>
		/// <remarks>
		/// The action is token1.
		/// </remarks>
		public String Action { get { return token1; } }

		/// <summary>Gets the player type for a player action.</summary>
		/// <remarks>
		/// The value is token2.
		/// </remarks>
		public PlayerType Player { get { return (PlayerType)Enum.Parse(typeof(PlayerType), token0); } }

		/// <summary>The value of the instruction as object.</summary>
		/// <remarks>
		/// The value is token2.
		/// </remarks>
		public Object Value { get { return token2; } }
		
		/// <summary>The value of the instruction as Int32.</summary>
		/// <remarks>
		/// The value is token2.
		/// </remarks>
		public Int32 Int32Value { get { return (Int32)token2; } }
		
		/// <summary>The value of the instruction as cards.</summary>
		/// <remarks>
		/// The value is token2.
		/// </remarks>
		public Cards CardsValue { get { return (Cards)token2; } }

		/// <summary>The value of the instruction as player type.</summary>
		/// <remarks>
		/// The value is token2.
		/// </remarks>
		public PlayerType PlayerTypeValue { get { return (PlayerType)token2; } }

		/// <summary>Gets the final result according to the instruction.</summary>
		/// <remarks>
		/// If the instruction is of the type "Player[12] finished [12]", the result
		/// is returned, otherwise NoResult.
		/// </remarks>
		public RoundResult FinalResult
		{
			get
			{
				if (this == Player1Finished1 || this == Player2Finished2) { return RoundResult.Player1Wins; }
				if (this == Player1Finished2 || this == Player2Finished1) { return RoundResult.Player2Wins; }
				return RoundResult.NoResult;
			}
		}

		private string token0;
		private string token1;
		private object token2;

		#endregion

		/// <summary>Returs true if instruction represent the empty value, otherwise false.</summary>
		public bool IsEmpty() { return token0 == default(String) && token1 == default(String) && token2 == default(Object); }

		/// <summary>Returns true if the object equals the instruction, otherwise false.</summary>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		/// <summary>Returns the hash code for the instruction.</summary>
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
		/// <summary>Returns true if both instruction are equal, otherwise false.</summary>
		public static bool operator ==(Instruction l, Instruction r) { return l.Equals(r); }
		/// <summary>Returns true if both instruction are not equal, otherwise false.</summary>
		public static bool operator !=(Instruction l, Instruction r) { return !(l == r); }

		/// <summary>Reprents the instruction a string.</summary>
		public override string ToString()
		{
			if (IsEmpty()) { return String.Empty; }
			return token0 + ' ' + token1 + ' ' + token2.ToString();
		}

		#region Factory methods

		/// <summary>Parses a line representing an instruction.</summary>
		public static Instruction Parse(string line)
		{
			if (String.IsNullOrEmpty(line)) { return Instruction.Empty; }
			if (line.StartsWith("Output from your bot: "))
			{
				return new Instruction()
				{
					token0 = "Output",
					token1 = "from your bot:",
					token2 = line.Substring(22)
				};
			}
			if (line.StartsWith("Engine says: "))
			{
				return new Instruction()
				{
					token0 = "Engine",
					token1 = "says:",
					token2 = line.Substring(13)
				};
			}
			var splited = line.Split(' ');
			if (splited.Length != 3) { throw new ArgumentException("The line should contain 3 tokens.", "line"); }

			if (!ValidToken0.Contains(splited[0])) 
			{
				throw new ArgumentException("The first token is not valid.", "line");
			}

			var instruction = new Instruction() { token0 = splited[0], token1 = splited[1], token2 = splited[2] };

			if (!ValidToken1[instruction.InstructionType].Contains(splited[1]))
			{
				throw new ArgumentException("The second token is not valid.", "line");
			}
			var tokenType = InstructionValueType.Int32;

			TokenTypes.TryGetValue(splited[1], out tokenType);

			object val = null;
			var token2 = splited[2];
			switch (tokenType)
			{
				case InstructionValueType.PlayerType: val = ParsePlayerType(token2); break;
				case InstructionValueType.Cards: val = ParseCards(token2); break;
				case InstructionValueType.Zero: val = ParseZero(token2); break;
				case InstructionValueType.Int32:
				default: val = ParseInt32(token2); break;
			}
			instruction.token2 = val;

			return instruction;
		}

		/// <summary>Creates an instruction.</summary>
		public static Instruction Create(PlayerType player, string action, object value)
		{
			var line = String.Format("{0} {1} {2}", player, action, value);
			return Instruction.Parse(line);
		}

		/// <summary>Creates an instruction.</summary>
		public static Instruction Create(InstructionType type, string action, object value)
		{
			var line = String.Format("{0} {1} {2}", type,  action, value);
			return Instruction.Parse(line);
		}

		/// <summary>Creates a player1|2 finished 1|2 instruction.</summary>
		/// <remarks>
		/// This is logged as engines says instruction unfortunatly.
		/// </remarks>
		public static Instruction CreateFinished(PlayerType player, int rank)
		{
			return new Instruction()
			{
				token0 = "Engine",
				token1 = "says:",
				token2 = string.Format("{0} finished {1}", '"', player, rank)
			};
		}

		private static int ParseInt32(string token2)
		{
			int number;

			if (Int32.TryParse(token2, out number) && number >= 0)
			{
				return number;
			}
			else
			{
				throw new ArgumentException("The thrid token is not a valid number.", "line");
			}
		}
		private static int ParseZero(string token2)
		{
			if (token2 != "0")
			{
				throw new ArgumentException("The thrid token should be zero.", "line");
			}
			else
			{
				return 0;
			}
		}
		private static Cards ParseCards(string token2)
		{
			try
			{
				return Cards.Parse(token2);
				
			}
			catch (Exception x)
			{
				throw new ArgumentException("The thrid token should be a set of cards", "line", x);
			}
		}
		private static PlayerType ParsePlayerType(string token2)
		{
			PlayerType player;
			if (Enum.TryParse<PlayerType>(token2, out player))
			{
				return player;
			}
			else
			{
				throw new ArgumentException("The thrid token should be player1 or player2.", "line");
			}
		}
	
		private static readonly string[] ValidToken0 = new string[] { "player1", "player2", "Action", "Match", "Settings" };

		private static readonly Dictionary<InstructionType, string[]> ValidToken1 = new Dictionary<InstructionType, string[]>()
		{
			{ InstructionType.None, new string[]{ } },
			{ InstructionType.Output, new string[]{ } },

			{ InstructionType.Action, new string[]{ "player1", "player2" } },
			{ InstructionType.Match, new string[]{ "round", "smallBlind", "bigBlind", "maxWinPot", "amountToCall", "onButton", "table" } },
			{ InstructionType.Player, new string[]{ "stack", "post", "hand", "call", "check", "raise", "wins", "fold" } },
			{ InstructionType.Settings, new string[]{ "timeBank", "timePerMove", "handsPerLevel", "startingStack", "yourBot" } },
		};

		private static readonly Dictionary<string, InstructionValueType> TokenTypes = new Dictionary<string, InstructionValueType>()
		{
			{ "table", InstructionValueType.Cards },
			{ "hand", InstructionValueType.Cards },
			{ "onButton", InstructionValueType.PlayerType },
			{ "yourBot", InstructionValueType.PlayerType },
			{ "check", InstructionValueType.Zero },
			{ "fold", InstructionValueType.Zero },
		};

		#endregion
	}
}
