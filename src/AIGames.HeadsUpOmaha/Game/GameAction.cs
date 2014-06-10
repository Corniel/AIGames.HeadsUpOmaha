using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>Reperesents an action.</summary>
	[Serializable, DebuggerDisplay("{DebugToString()}")]
	public struct GameAction : ISerializable, IXmlSerializable
	{
		/// <summary>Represents a check.</summary>
		public static readonly GameAction Check = new GameAction() { m_Value = (int)GameActionType.check };
		/// <summary>Represents a call.</summary>
		public static readonly GameAction Call = new GameAction() { m_Value = (int)GameActionType.call };
		/// <summary>Represents a fold.</summary>
		public static readonly GameAction Fold = new GameAction() { m_Value = (int)GameActionType.fold };

		/// <summary>Creates a raise.</summary>
		public static GameAction Raise(int amount)
		{
			if (amount < 1) { throw new ArgumentOutOfRangeException("The amount should be at least 1.", "amount"); }
			return new GameAction() { m_Value = (amount << 2) + (int)GameActionType.raise };
		}

		#region (XML) (De)serialization

		/// <summary>Initializes a new instance of action based on the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		private GameAction(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			m_Value = info.GetByte("Value");
		}

		/// <summary>Adds the underlying propererty of action to the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			info.AddValue("Value", m_Value);
		}

		/// <summary>Gets the xml schema to (de) xml serialize an action.</summary>
		/// <remarks>
		/// Returns null as no schema is required.
		/// </remarks>
		XmlSchema IXmlSerializable.GetSchema() { return null; }

		/// <summary>Reads the action from an xml writer.</summary>
		/// <remarks>
		/// Uses the string parse function of action.
		/// </remarks>
		/// <param name="reader">An xml reader.</param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			var s = reader.ReadElementString();
			var val = Parse(s);
			m_Value = val.m_Value;
		}

		/// <summary>Writes the action to an xml writer.</summary>
		/// <remarks>
		/// Uses the string representation of action.
		/// </remarks>
		/// <param name="writer">An xml writer.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString());
		}

		#endregion

		#region Properties

		/// <summary>Represents the internal value.</summary>
		private int m_Value;

		/// <summary>Gets the height of the action.</summary>
		public int Amount { get { return m_Value >> 2; } }

		/// <summary>Gets the suit of the action.</summary>
		public GameActionType ActionType { get { return (GameActionType)(m_Value & 3); } }

		#endregion

		#region Tostring

		/// <summary>Represents an action as a string.</summary>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.ActionType, this.Amount);
		}

		/// <summary>Represents an action as a debug string.</summary>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		private string DebugToString()
		{
			return String.Format("{0}{1}",
				this.ActionType,
				this.Amount > 0 ? " " + this.Amount.ToString() : "");
		}

		#endregion

		#region Equality

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">
		/// Another object to compare to.
		/// </param>
		/// <returns>
		/// true if obj and this action are the same type and represent the same value;
		/// otherwise, false.
		/// </returns>
		public override bool Equals(object obj) { return base.Equals(obj); }

		/// <summary>Returns the hash code for this action.</summary>
		public override int GetHashCode() { return (int)m_Value; }

		/// <summary>Returns true if left equals right, otherwise false.</summary>
		public static bool operator ==(GameAction l, GameAction r) { return l.Equals(r); }
		/// <summary>Returns false if left equals right, otherwise true.</summary>
		public static bool operator !=(GameAction l, GameAction r) { return !(l == r); }

		#endregion

		#region Factory methods
		
		/// <summary>Parses an action.</summary>
		/// <param name="str">
		/// The string repersenting an action.
		/// </param>
		public static GameAction Parse(string str)
		{
			GameAction action;
			if (TryParse(str, out action))
			{
				return action;
			}
			throw new ArgumentException("The input does not represent a valid action.", "str");
		}

		/// <summary>Tries to parse an action.</summary>
		/// <param name="str">
		/// The string repersenting an action.
		/// </param>
		/// <param name="action">
		/// The actual action.
		/// </param>
		/// <returns>
		/// True, if the parsing succeeded, otherwise false.
		/// </returns>
		/// <remarks>
		/// A action is always represented by two characters:
		/// The first represents the action height and can be any number 2-9.
		/// T, J, Q, K, A are 10, Jack, Queen, King and Ace respectively.
		/// 
		/// The second character represents the suit and can be d, c, h or s.
		/// For Diamonds, Clubs, Hearts and Spades respectively.
		/// </remarks>
		public static bool TryParse(string str, out GameAction action)
		{
			action = GameAction.Check;

			if (String.IsNullOrEmpty(str)) { return true; }


			var splitted = str.Split(' ');

			GameActionType tp;

			if (splitted.Length < 3 && Enum.TryParse<GameActionType>(splitted[0], out tp))
			{
				if(tp != GameActionType.raise && (splitted.Length == 1 || splitted[1] == "0"))
				{
					action = new GameAction() { m_Value = (int)tp };
					return true;
				}
				int amount;
				if (Int32.TryParse(splitted[1], out amount))
				{
					action = Raise(amount);
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
