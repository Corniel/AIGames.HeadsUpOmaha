using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.ACDC
{
	public class GameSimulation
	{
		private static readonly int[] subrounds = { 0, 3, 4, 5 };

		public double GetScore(GameState state, MT19937Generator rnd, List<int> subs)
		{
			if (!state.StartNewRound(rnd))
			{
				if (state.Player1.Stack < state.BigBlind)
				{
					return 0.0;
				}
				return 1.0;
			}

			var table = state.Table;
			state.Table = null;

			var player = state.OnButton;

			var action = GameAction.Check;

			for (int subround = 0; subround < subrounds.Length; subround++)
			{
				state.Table = Cards.Create(table.Take(subrounds[subround]));
				var cur = player;

				while (action != GameAction.Fold)
				{
					action = Action(state.Personalize(cur), rnd);
					if (action == GameAction.Fold) { break; }
					state.Update(cur, action);
					if (state[cur].Stack < 0)
					{
					}
					cur = cur.Other();

					if (cur == player && action.ActionType != GameActionType.raise) { break; }
				}
			}
			state.ApplyRoundResult(RoundResult.NoResult);
			subs.Add(state.Player1.Stack);
			return GetScore(state, rnd, subs);

		}


		public GameAction Action(GameState state, MT19937Generator rnd)
		{
			if (state.Own.Stack <= 0)
			{
			}

			var pWin = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, rnd, 10);

			// Only play doable small blinds.
			if (state.IsPreFlop && pWin < 0.4 && state.AmountToCall > 0)
			{
				return GameAction.Fold;
			}
			if (state.MaxinumRaise > 0)
			{
				if (pWin > 0.55 && state.AmountToCall == 0)
				{
					return GameAction.Raise(state.MaxinumRaise);
				}
				if (pWin > 0.7)
				{
					return GameAction.Raise(state.MaxinumRaise);
				}
			}
			if (pWin < 0.35 && state.AmountToCall > 0)
			{
				return GameAction.Fold;
			}

			return GameAction.CheckOrCall(state);
		}
	}
}
