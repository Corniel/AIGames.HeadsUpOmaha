using AIGames.HeadsUpOmaha.Game;

namespace AIGames.HeadsUpOmaha.Bot
{
	/// <summary>Represents a poker bot.</summary>
	public interface IBot
	{
		/// <summary>The action of the the bot.</summary>
		/// <param name="state">
		/// The current state.
		/// </param>
		GameAction Action(GameState state);

		/// <summary>The reaction of the opponent.</summary>
		/// <param name="state">
		/// The current state.
		/// </param>
		/// <param name="reaction">
		/// The action.
		/// </param>
		void Reaction(GameState state, GameAction reaction);

		/// <summary>The state when a result (win, loss, draw) was made.</summary>
		void Result(GameState state);

		/// <summary>The state when a final result (first, second) was made.</summary>
		void FinalResult(GameState state);
	}
}
