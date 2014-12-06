using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Game;
using System;
using System.Diagnostics;
using System.Xml.Serialization;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.ACDC
{
	[Serializable, DebuggerDisplay("{DebugToString()}")]
	public class ActionDataRecord
	{
		public ActionDataRecord()
		{
			this.WinningChance = double.NaN;
		}
		/// <summary>Represents the round.</summary>
		[XmlAttribute("rnd")]
		public int Round { get; set; }
		/// <summary>Represents the sub round (equals the size of the table).</summary>
		public int SubRound { get { return this.Table == null ? 0 : this.Table.Count; } }

		/// <summary>Represent the bet round (equals the nth action on a subround).</summary>
		[XmlAttribute("sub")]
		public int BetRound { get; set; }

		[XmlAttribute("blind")]
		public int SmallBlind { get; set; }
		public int BigBlind { get { return this.SmallBlind * 2; } }

		/// <summary>Returns true, if the opponent has the on button, otherwise false.</summary>
		[XmlAttribute("but")]
		public bool IsOnButton { get; set; }

		public GameAction Action { get; set;}
		public Cards HandOwn { get; set; }
		public Cards HandOpp { get; set; }
		public Cards Table { get; set; }

		[XmlAttribute("own")]
		public int StackOwn { get; set; }
		[XmlAttribute("opp")]
		public int StackOpp { get; set; }

		[XmlAttribute("max")]
		public int MaxRaise { get; set; }
		[XmlAttribute("add")]
		public int PotAdd { get; set; }
		[XmlAttribute("pot")]
		public int Pot { get; set; }
		[XmlAttribute("call")]
		public int AmountToCall { get; set; }

		/// <summary>The winning your chance.</summary>
		[XmlAttribute("p")]
		public double WinningChance { get; set; }

		/// <summary>Returns true if not winning chance is available.</summary>
		public bool NoWinningChance { get { return double.IsNaN(this.WinningChance); } }

		public RoundResult Result { get; set; }

		/// <summary>Update a record.</summary>
		public void Update(GameState state, MT19937Generator rnd)
		{
			this.HandOpp = state.Opp.Hand.Copy();

			// if a hand was revealed, calculate the winning change.
			if (this.HandOpp.Count > 0)
			{
				this.WinningChance = 1.0 - PokerHandEvaluator.Calculate(this.HandOpp, this.Table, rnd, 200);
			}
		}

		private string DebugToString()
		{
			return string.Format(
				"[{0:00}] {1}, Blind: {2}, Pot: {3}(+{4}), Stack:{5}-{6}, Tbl: {7:f}, Hand: {8:f}, P: {9:0.0%}{10}",
				this.Round, 
				this.Action,
				this.BigBlind,
				this.Pot,
				this.PotAdd,
				this.StackOpp,
				this.StackOwn,
				this.Table,
				this.HandOpp,
				this.WinningChance,
				this.Result == RoundResult.NoResult ? "" : ", Result: " + this.Result.ToString()
				);
		}

		public static ActionDataRecord Create(GameState state, GameAction reaction)
		{
			var record = new ActionDataRecord()
			{
				Round = state.Round,
				IsOnButton = state.OnButton != state.YourBot,
				SmallBlind = state.SmallBlind,
				Action = reaction,
				HandOwn = state.Own.Hand.Copy(),
				Table = state.Table.Copy(),
				StackOwn = state.Own.Stack,
				StackOpp = state.Opp.Stack,
				Pot = state.Own.Pot + state.Opp.Pot,
				PotAdd = reaction.Amount + state.AmountToCall,
				MaxRaise = state.MaxinumRaise,
				AmountToCall = state.AmountToCall,
			};
			return record;
		}

		public static ActionDataRecord Create(GameState state)
		{
			var record = Create(state, default(GameAction));
			record.HandOpp = state.Opp.Hand.Copy();
			record.Result = state.Result;

			switch (record.Result)
			{
				case RoundResult.Player1Wins: record.WinningChance = state.YourBot == PlayerType.player1 ? 1.0 : 0.0; break;
				case RoundResult.Player2Wins: record.WinningChance = state.YourBot == PlayerType.player2 ? 1.0 : 0.0; break;
				case RoundResult.Draw: record.WinningChance = 0.5; break;
				case RoundResult.NoResult: break;
				default:break;
			}
			return record;
		}
	}
}
