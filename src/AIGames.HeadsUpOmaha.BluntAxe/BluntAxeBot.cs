using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.BluntAxe
{
	public class BluntAxeBot : IBot
	{
		public BluntAxeBot()
		{
			this.Rnd = new MT19937Generator();
		}
		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			var change = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd);

			if (state.AmountToCall == state.SmallBlind && change < 0.4)
			{
				return GameAction.Fold;
			}
			if (change > 0.75)
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
	}
}
