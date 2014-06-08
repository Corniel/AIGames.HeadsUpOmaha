using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using System;

namespace AIGames.HeadsUpOmaha.AllIn
{
	public class AllInBot : IBot
	{
		public GameAction Action(GameState state)
		{
			var raise = Math.Max(state.Own.Stack, state.MaxWinPot + state.AmountToCall);
			if (raise > 0)
			{
				return GameAction.Raise(raise);
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
