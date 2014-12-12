using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using System;

namespace AIGames.HeadsUpOmaha.AllIn
{
	public class AllInBot : IBot
	{
		public GameAction Action(GameState state)
		{
			if (state.MaxinumRaise > 0)
			{
				return GameAction.Raise(state.MaxinumRaise);
			}
			return GameAction.CheckOrCall(state);
		}
		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }
		public void FinalResult(GameState state) { }
	}
}
