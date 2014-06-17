using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.KingKong
{
	public class KingKongBot : IBot
	{
		public static void Main(string[] args) { ConsolePlatform.Run(new KingKongBot()); }

		public KingKongBot()
		{
			this.Rnd = new MT19937Generator();
		}
		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			var change = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd, 1000);

			// Only play doable small blinds.
			if (state.IsPreFlop && change < 0.4 && state.AmountToCall == state.SmallBlind)
			{
				return GameAction.Fold;
			}
			// Don't play bad hands on pre flop if we have to call.
			if (state.IsPreFlop && change < 0.3 && state.AmountToCall >= state.BigBlind)
			{
				return GameAction.Fold;
			}
			if (change > 0.55 && state.AmountToCall == 0)
			{
				return GameAction.Raise(state.MaxinumRaise);
			}
			if (change > 0.7)
			{
				return GameAction.Raise(state.MaxinumRaise);
			}
			if (change < 0.35 && state.AmountToCall > 0)
			{
				return GameAction.Fold;
			}

			if (state.AmountToCall > 0)
			{
				return GameAction.Call;
			}
			else
			{
				return GameAction.Check;
			}
		}
		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }
	}
}