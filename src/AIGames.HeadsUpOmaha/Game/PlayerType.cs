
namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>The player type.</summary>
	public enum PlayerType
	{
		/// <summary>Player 1.</summary>
		player1 = 0,
		/// <summary>Player 2.</summary>
		player2 = 1,
	}

	/// <summary>Extensions on player type.</summary>
	public static class PlayerTypeExtensions
	{
		/// <summary>Get the other player.</summary>
		public static PlayerType Other(this PlayerType current)
		{
			return (PlayerType)((int)current ^ 1);
		}
	}
}
