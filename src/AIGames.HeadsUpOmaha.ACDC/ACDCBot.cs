using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using System.Collections.Generic;
using Troschuetz.Random.Generators;
using System.Linq;

namespace AIGames.HeadsUpOmaha.ACDC
{
	public class ACDCBot : IBot
	{
		public static void Main(string[] args) { ConsolePlatform.Run(new ACDCBot()); }

		public ACDCBot()
		{
			this.Rnd = new MT19937Generator();
			this.Opponent = new OpponentBot();

			//try
			//{
			//	this.Data = ActionData.Load("data.xml");
			//}
			//catch
			//{
			this.Data = new ActionData();
			//}
		}

		/// <summary>Gets the opponent bot.</summary>
		public OpponentBot Opponent { get; protected set; }

		/// <summary>The randomizer.</summary>
		public MT19937Generator Rnd { get; set; }

		///// <summary>Gets the action data.</summary>
		public ActionData Data { get; protected set; }

		/// <summary>The action of the the bot.</summary>
		/// <param name="state">
		/// The current state.
		/// </param>
		public GameAction Action(GameState state)
		{
			// Only play doable small blinds.
			if (state.IsPreFlop)
			{
				return PreFlopAction(state);
			}

			var pWin = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd, 1000);
			var pLos = 1.0 - pWin;

			var options = new Dictionary<GameAction, double>();

			if (state.NoAmountToCall)
			{
				options[GameAction.Check] = pWin * GetP(state.Own.Stack + state.Pot, state) + pLos * GetP(state.Own.Stack, state);
				if (state.CanRaise)
				{
					for (var raise = state.MinimumRaise; raise <= state.MinimumRaise; raise += 20)
					{
						// asuming that ther is no folding.
						var stackWin = state.Own.Stack + state.Pot + raise;
						var stackLos = state.Own.Stack - raise;
						options[GameAction.Raise(raise)] = pWin * GetP(stackWin, state) + pLos * GetP(stackLos, state);
					}
				}
			}
			else
			{
				options[GameAction.Fold] = GetP(state.Own.Stack, state);
				options[GameAction.Call] = pWin * GetP(state.Own.Stack + state.Pot, state) + pLos * GetP(state.Own.Stack - state.AmountToCall, state);
			}
			return options.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
		}

		public GameAction PreFlopAction(GameState state)
		{
			var pWin = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd, 500);
			var pLos = 1.0 - pWin;

			var options = new Dictionary<GameAction, double>();

			if (state.NoAmountToCall)
			{
				return GameAction.Check;
				//options[GameAction.Check] = pWin * GetP(state.Own.Stack + state.Pot, state) + pLos * GetP(state.Own.Stack, state);
			}
			options[GameAction.Fold] = GetP(state.Own.Stack, state);
			options[GameAction.Call] = pWin * GetP(state.Own.Stack + state.Pot, state) + pLos * GetP(state.Own.Stack - state.AmountToCall, state);

			return options.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
		}

		/// <summary>The reaction of the opponent.</summary>
		/// <param name="state">
		/// The current state.
		/// </param>
		/// <param name="reaction">
		/// The action.
		/// </param>
		public void Reaction(GameState state, GameAction reaction)
		{
			this.Data.Add(state, reaction);
		}

		/// <summary>The state when a result (win, loss, draw) made.</summary>
		public void Result(GameState state)
		{
			this.Data.Update(state, this.Rnd);
			//this.Data.Save("data.xml");
		}

		/// <summary>The state when a final result (first, second) was made.</summary>
		public void FinalResult(GameState state) { }

		/// <summary>Gets a guessed winning change base, based on the stack.</summary>
		/// <remarks>
		/// As guess the 3 times big blind is choosen as -2 SD.
		/// </remarks>
		public static double GetP(int stack, GameState state)
		{
			if (stack < state.BigBlind) { return 0; }
			if (stack > state.Chips - state.BigBlind) { return 1; }
			double avg = state.Chips / 2.0;
			double min2sd = 3 * state.BigBlind;

			var a = (min2sd - avg) / 2.0;

			var x = (avg - stack) / a;

			return Gauss.GetZ(x);
		}
	}
}
