using AIGames.HeadsUpOmaha.Game;
using System;

namespace AIGames.HeadsUpOmaha.Rnd
{
	public struct FlattenState
	{
		private int round;
		private int stack1;
		private int stack2;
		private int pot1;
		private int pot2;
		private GameAction action;
		private PlayerType player;
		private PlayerType button;

		public int Round { get { return round; } }
		public int Stack1 { get { return stack1; } }
		public int Stack2 { get { return stack2; } }
		public int Pot1 { get { return pot1; } }
		public int Pot2 { get { return pot2; } }
		public PlayerType Player { get { return player; } }
		public PlayerType Button { get { return button; } }

		public override string ToString()
		{
			return String.Format("[{0}] P1: {1}{8}, P2: {2}{9}, {3}+{4}={5}, p{6} {7}",
				round,
				stack1, stack2,
				pot1, pot2, pot1 + pot2,
				player.ToString().Substring(6),
				action,
				button == PlayerType.player1 ? "(btn)" : "",
				button == PlayerType.player2 ? "(btn)" : ""
				);
		}

		public static FlattenState Create(GameState state, GameAction act, PlayerType playerToMove)
		{
			return new FlattenState()
			{
				round = state.Round,
				stack1 = state.Player1.Stack,
				stack2 = state.Player2.Stack,
				pot1 = state.Player1.Pot,
				pot2 = state.Player2.Pot,
				action = act,
				button = state.OnButton,
				player = playerToMove,
			};
		}
	}
}
