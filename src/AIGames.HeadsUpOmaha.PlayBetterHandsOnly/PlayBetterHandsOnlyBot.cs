using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using System.Linq;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.PlayBetterHandsOnly
{
	public class PlayBetterHandsOnlyBot : IBot
	{
		public PlayBetterHandsOnlyBot()
		{
			this.Rnd = new MT19937Generator();
		}
		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			if (state.AmountToCall > 0)
			{
				var other = Cards.Deck.ToList();
				var table = state.Table.ToList();

				var chance = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd);

				if (chance < 0.6)
				{
					return GameAction.Fold;
				}
				return GameAction.Call;
			}

			return GameAction.Check;
		}

		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }
		public void FinalResult(GameState state) { }
	}
}
