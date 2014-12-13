using AIGames.HeadsUpOmaha.Arena;
using AIGames.HeadsUpOmaha.Arena.Platform;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.UnitTests.Mocking;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIGames.HeadsUpOmaha.UnitTests.Arena
{
	[TestFixture]
	public class ArenaTest
	{
		[Test, Ignore]
		public void PlayMatch_TwoRandomBots_GoThroughTheSameStates()
		{
			var arena = new ArenaMock();
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
			Write(arena.States, "arena.txt");

			CollectionAssert.AreEqual(arena.States, bot1s);
			CollectionAssert.AreEqual(arena.States, bot2s);
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
}
