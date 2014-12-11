using System;
namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>Represents the outcome of a round.</summary>
	public enum RoundResult
	{
		/// <summary>No result (yet).</summary>
		NoResult = 0,
		/// <summary>Player 1 wins.</summary>
		Player1Wins,
		/// <summary>Player 2 wins.</summary>
		Player2Wins,
		/// <summary>It's a draw.</summary>
		Draw,
	}
}
