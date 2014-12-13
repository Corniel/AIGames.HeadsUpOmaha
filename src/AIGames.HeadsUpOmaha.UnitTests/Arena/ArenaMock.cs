using AIGames.HeadsUpOmaha.Arena;
using AIGames.HeadsUpOmaha.Arena.Platform;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.UnitTests.Mocking;
using System.Collections.Generic;

namespace AIGames.HeadsUpOmaha.UnitTests.Arena
{
	public class ArenaMock : CompetitionRunner
	{
		public ArenaMock()
			: base(ArenaConfig.BotsDirectory, 17)
		{
			this.States = new List<FlattenState>();
		}
		public List<FlattenState> States { get; private set; }
		public GameState State { get; private set; }

		protected override void StartNewRound(Dictionary<PlayerType, ConsoleBot> bots, GameState state)
		{
			base.StartNewRound(bots, state);
			this.State = state;
			Add(this.State, GameAction.Invalid, PlayerType.player1);
		}
		protected override GameAction GetAction(Dictionary<PlayerType, ConsoleBot> bots, GameState state, PlayerType playerToMove)
		{
			var action = base.GetAction(bots, state, playerToMove);
			Add(this.State, action, playerToMove);
			return action;
		}

		private void Add(GameState state, GameAction action, PlayerType playerToMove)
		{
			this.States.Add(FlattenState.Create(state, action, playerToMove));
		}
	}
}
