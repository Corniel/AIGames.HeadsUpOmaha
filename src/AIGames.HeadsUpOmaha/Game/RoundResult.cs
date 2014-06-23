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

	/// <summary>Extensions on round result.</summary>
	public static class RoundResultExtensions
	{
		/// <summary>Returns the player that became first.</summary>
		public static PlayerType GetFirst(this RoundResult result)
		{
			switch (result)
			{
				case RoundResult.Player1Wins: return PlayerType.player1;
				case RoundResult.Player2Wins: return PlayerType.player2;
				case RoundResult.Draw:
				case RoundResult.NoResult:
				default:
					throw new NotSupportedException("GetFirst() is only supported for winning.");
			}
		}
		/// <summary>Returns the player that became second.</summary>
		public static PlayerType GetSecond(this RoundResult result)
		{
			switch (result)
			{
				case RoundResult.Player1Wins: return PlayerType.player2;
				case RoundResult.Player2Wins: return PlayerType.player1;
				case RoundResult.Draw:
				case RoundResult.NoResult:
				default:
					throw new NotSupportedException("GetFirst() is only supported for winning.");
			}
		}
	}
}
