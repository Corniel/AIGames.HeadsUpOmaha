using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;

namespace AIGames.HeadsUpOmaha.Chicken
{
	/// <summary>Represents a chicken of a bot.</summary>
	public class ChickenBot : IBot
	{
		/// <summary>Gets the action.</summary>
		/// <remarks>
		/// If the amount to call is bigger than zero, and the pot is will be filled
		/// with a least 4 times the big blindm fold.
		/// 
		/// Otherwise call or check.
		/// </remarks>
		public GameAction Action(GameState state)
		{
			if (state.AmountToCall > 0 && (state.Pot + state.AmountToCall) >= 4 * state.BigBlind)
			{
				return GameAction.Fold;
			}
			if (state.AmountToCall > 0)
			{
				return GameAction.Call;
			}
			return GameAction.Check;
		}

		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }
		public void FinalResult(GameState state) { }
	}
}
