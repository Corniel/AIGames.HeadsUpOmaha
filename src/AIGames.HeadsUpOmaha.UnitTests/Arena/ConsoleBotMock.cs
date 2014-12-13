using AIGames.HeadsUpOmaha.Arena;
using AIGames.HeadsUpOmaha.Arena.Platform;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.UnitTests.Mocking;
using System.Collections.Generic;
using System.IO;


namespace AIGames.HeadsUpOmaha.UnitTests.Arena
{
	public class ConsoleBotMock : ConsoleBot
	{
		public ConsoleBotMock(HeadsUpOmaha.Arena.Bot bot)
		{
			this.Bot = bot;
			this.States = new List<FlattenState>();

			var fs = Path.Combine(ArenaConfig.BotsDirectory.FullName, "zzz.RandomBot.0002", bot.Info.Name + ".txt");
			this.Writer = new StreamWriter(fs);

			var file = new FileInfo(Path.Combine(ArenaConfig.BotsDirectory.FullName, "zzz.RandomBot.0002/RandomBot.0002.exe"));
			this.process = CreateProcess(ArenaConfig.BotsDirectory, file);
		}
		public List<FlattenState> States { get; private set; }
		public GameState Current { get; private set; }
		public GameState Previous { get; private set; }
		public Stream Stream { get { return this.Writer.BaseStream; } }

		public override void UpdateNewRound(GameState state)
		{
			base.UpdateNewRound(state);

			// We want to compare, so these players should be equal.
			Add(state, GameAction.Invalid, PlayerType.player1);
			this.Previous = state.Copy();
			this.Current = state.Copy();
		}

		public override GameAction Action(GameState state)
		{
			this.Previous = this.Current;
			this.Current = state.Copy();
			return base.Action(state);
		}
		public override void Reaction(GameAction reaction, PlayerType playerToMove)
		{
			Add(this.Previous, reaction, playerToMove);
			base.Reaction(reaction, playerToMove);
		}

		private void Add(GameState state, GameAction action, PlayerType playerToMove)
		{
			this.States.Add(FlattenState.Create(state, action, playerToMove));
		}
	}
}
