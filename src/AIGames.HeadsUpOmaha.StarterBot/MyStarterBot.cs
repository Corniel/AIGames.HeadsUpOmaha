using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;

namespace AIGames.HeadsUpOmaha.StarterBot
{
	public class MyStarterBot : IBot
	{
		public void Reaction(GameState state, GameAction reaction) { }

		public GameAction Action(GameState state)
		{
			var pairs = CardPairs.Create(state.Own.Hand);

			// We have to (Small Blind), and a creapy hand.
			if (state.AmountToCall != 0 && state.Table.Count == 0)
			{
				if (pairs.Max < 2 && state.Own.Hand.Best.Height < 11)
				{
					return GameAction.Fold;
				}
				return GameAction.Call;
			}
			// If we need to call, we call.			
			if (state.AmountToCall != 0)
			{
				return GameAction.Call;
			}
			// we check.
			return GameAction.Check;
		}

		public void Result(GameState state) { }
	}
}
