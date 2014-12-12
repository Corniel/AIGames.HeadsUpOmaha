using AIGames.HeadsUpOmaha.Arena;
using AIGames.HeadsUpOmaha.Arena.Platform;
using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIGames.HeadsUpOmaha.UnitTests.Arena
{
	[TestFixture]
	public class ArenaTest
	{
		[Test]
		public void PlayMatch_TwoRandomBots_GoThroughTheSameStates()
		{
			var arena = new CompetitionRunner(ArenaConfig.BotsDirectory, 17);
			arena.GameSettings.StartingStack = 500;
			arena.Player1 = new HeadsUpOmaha.Arena.Bot() { Info = new BotInfo("Player1UT") };
			arena.Player2 = new HeadsUpOmaha.Arena.Bot() { Info = new BotInfo("Player2UT") };

			List<FlattenState> bot1s = null;
			List<FlattenState> bot2s = null;

			using (var bot1 = new ConsoleBotMock(arena.Player1))
			{
				using (var bot2 = new ConsoleBotMock(arena.Player2))
				{

					var result = arena.PlayMatch(bot1, bot2);

					Assert.AreEqual(RoundResult.Player2Wins, result);

					bot1s = bot1.States;
					bot2s = bot2.States;
				}
			}
			Write(bot1s, "bot1s.txt");
			Write(bot2s, "bot2s.txt");

			CollectionAssert.AreEqual(bot1s, bot2s);
		}
		private void Write(IEnumerable<FlattenState> states, string filename)
		{
			var fs = Path.Combine(ArenaConfig.BotsDirectory.FullName, "zzz.RandomBot.0002", filename);
			using (var writer = new StreamWriter(fs))
			{
				foreach(var state in states)
				{
					writer.WriteLine(state);
				}
			}
		}
	}

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
		public GameState State { get; private set; }
		public Stream Stream { get { return this.Writer.BaseStream; } }

		public override void UpdateNewRound(GameState state)
		{
			base.UpdateNewRound(state);

			// We want to compare, so these players should be equal.
			Add(state, GameAction.Invalid, PlayerType.player1);
			this.State = state;
		}

		public override GameAction Action(GameState state)
		{
			this.State = state;
			return base.Action(state);
		}
		public override void Reaction(GameAction reaction, PlayerType playerToMove)
		{
			Add(this.State, reaction, playerToMove);
			base.Reaction(reaction, playerToMove);
		}

		public override void Result(GameState state, int pot, LastGameAction lastaction)
		{
			Add(state, lastaction.Action, lastaction.Player);
			base.Result(state, pot, lastaction);
		}

		private void Add(GameState state, GameAction action, PlayerType playerToMove)
		{
			this.States.Add(FlattenState.Create(state, action, playerToMove));
		}
	}

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
