using AIGames.HeadsUpOmaha.Game;

namespace AIGames.HeadsUpOmaha.Bot
{
	/// <summary>Represents a poker bot.</summary>
	public interface IBot
	{
		/// <summary>The reaction of the opponent.</summary>
		/// <param name="state">
		/// The current state.
		/// </param>
		/// <param name="reaction">
		/// The action.
		/// </param>
		void Reaction(GameState state, GameAction reaction);

		/// <summary>The action of the the bot.</summary>
		/// <param name="state">
		/// The current state.
		/// </param>
		GameAction Action(GameState state);

		/// <summary>The state when a result (win, loss, draw) made.</summary>
		void Result(GameState state);
	}
}
