using AIGames.HeadsUpOmaha.Game;
using System;
using System.Diagnostics;

namespace AIGames.HeadsUpOmaha.Arena
{
	[Serializable, DebuggerDisplay("{DebugToString()}")]
	public class LastGameAction
	{
		/// <summary>Constructs a new last game action.</summary>
		public LastGameAction(PlayerType player, GameAction action)
		{
			this.Player = player;
			this.Action = action;
		}
		public PlayerType Player { get; set; }
		public GameAction Action { get; set; }

		public RoundResult GetResult()
		{
			if (Action == GameAction.Fold)
			{
				switch (this.Player)
				{
					case PlayerType.player1: return RoundResult.Player2Wins;
					case PlayerType.player2: return RoundResult.Player1Wins;
				}
			}
			return RoundResult.NoResult;
		}

		/// <summary>Represents a card as a debug string.</summary>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		private string DebugToString()
		{
			return string.Format("Last: {0}, {1}", this.Player, this.Action);
		}

		
	}
}
