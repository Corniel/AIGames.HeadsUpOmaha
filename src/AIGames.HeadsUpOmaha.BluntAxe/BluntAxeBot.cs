using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.BluntAxe
{
	public class BluntAxeBot : IBot
	{
		public static void Main(string[] args) { ConsolePlatform.Run(new BluntAxeBot()); }

		public BluntAxeBot()
		{
			this.Rnd = new MT19937Generator();
		}
		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			var change = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd);

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

			// It's look like we're winning, use brute force.
			if (state.Own.Chips / state.Opp.Chips > 2.5 && change > 0.7 && state.MaxinumRaise > 0)
			{
				return GameAction.Raise(state.MaxinumRaise);
			}
			if (change > 0.75 && state.MaxinumRaise > 0)
			{
				var raise = state.AmountToCall + state.BigBlind + this.Rnd.Next(state.MaxWinPot - state.BigBlind) * change * change;
				return GameAction.Raise((int)raise);
			}
			if (change < 0.35 && state.AmountToCall > 0)
			{
				var treshold = this.Rnd.Next();

				var situation = (double)(state.AmountToCall + state.Own.Pot) / (double)(state.Own.Pot + state.Own.Stack);

				if (treshold < situation)
				{
					return GameAction.Fold;
				}
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
		public void FinalResult(GameState state) { }
	}
}
