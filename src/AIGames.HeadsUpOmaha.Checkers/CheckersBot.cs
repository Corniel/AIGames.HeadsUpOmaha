using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;

namespace AIGames.HeadsUpOmaha.Checkers
{
	public class CheckersBot : IBot
	{
		public GameAction Action(GameState state)
		{
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
