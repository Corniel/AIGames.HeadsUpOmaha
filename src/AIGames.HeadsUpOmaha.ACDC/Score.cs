using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.ACDC
{
	[Serializable, DebuggerDisplay("{DebugToString()}")]
	public class Score : IComparable, IComparable<Score>
	{
		protected Score(GameAction action,GameState state, double chips)
		{
			this.Action = action;
			this.Value = (chips/ (double)state.BigBlind) - 1.0;
		}

		public GameAction Action { get; protected set; }
		public double Value { get; protected set; }

		private string DebugToString()
		{
			return string.Format("{0:0,000} {1}", this.Value, this.Action);
		}

		public static Score Raise(GameState state, int raise, double pFold, double pWin, double pAllin)
		{
			var chipsFold = state.Own.Stack + state.Pot;

			var chipsAllin = GetMaxRaise(state, raise);

			var chipsWin = state.Own.Stack + state.Pot + raise - state.AmountToCall + pAllin * (chipsAllin - raise);
			var chipsLoss = state.Own.Stack - raise - pAllin * (chipsAllin - raise);

			var pLoss = 1.0 - pWin;
			var chipsNoFold = pWin * chipsWin + pLoss * chipsLoss;

			var pNoFold = 1.0 - pFold;
			var chips = pFold * chipsFold + pNoFold * chipsNoFold;

			return new Score(GameAction.Raise(raise), state, chips);
		}

		public static Score Check(GameState state, double pWin, double pAllin)
		{
			var pLoss = 1.0 - pWin;

			var chipsAllin = GetMaxRaise(state);

			var chipsWin = state.Own.Stack + state.Pot + pAllin * chipsAllin;
			var chipsLoss = state.Own.Stack - pAllin * chipsAllin;

			var chips = pWin * chipsWin + pLoss * chipsLoss;

			return new Score(GameAction.Check, state, chips);
		}

		public static Score Call(GameState state, double pWin, double pAllin)
		{
			var pLoss = 1.0 - pWin;

			var chipsAllin = GetMaxRaise(state);

			var chipsWin = state.Own.Stack + state.Pot + pAllin * chipsAllin;
			var chipsLoss = state.Own.Stack - state.AmountToCall - pAllin * chipsAllin;

			var chips = pWin * chipsWin + pLoss * chipsLoss;

			return new Score(GameAction.Check, state, chips);
		}

		public static Score Fold(GameState state)
		{
			var chips = state.Own.Stack;

			return new Score(GameAction.Fold, state, chips);
		}

		/// <summary>Gets the maximum rais that can occure.</summary>
		/// <remarks>
		/// The assumption is that after this action only calls and checks follow
		/// from you side.
		/// </remarks>
		public static int GetMaxRaise(GameState state, int raise = 0)
		{
			var stackOwn = state.Own.Stack - state.AmountToCall;
			var stackOpp = state.Opp.Stack;
			var pot = state.Pot + state.AmountToCall;

			var subRound = state.SubRound;
			if (subRound == 0) { subRound = 2; }

			// Can ther be a bet in this round?
			if (state.AmountToCall > 0 || raise > 0 || state.OnButton == state.YourBot)
			{
				subRound--;
			}

			if (raise > 0)
			{
				stackOwn -= raise;
				stackOpp -= raise;
				pot += 2 * raise;
			}

			for (int i = subRound; i < 5; i++)
			{
				var stackMin = Math.Min(stackOpp, stackOpp);
				var r = Math.Min(stackMin, pot);
				if (r > state.BigBlind)
				{
					pot += 2 * r;
					stackOwn -= r;
					stackOpp -= r;
				}
			}
			return state.Own.Stack - stackOwn - state.AmountToCall;
		}

		public int CompareTo(object obj)
		{
			return CompareTo(obj as Score);
		}

		public int CompareTo(Score other)
		{
			return other == null ? 1 : other.Value.CompareTo(this.Value);
		}
	}
}
