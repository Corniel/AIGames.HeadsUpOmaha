using AIGames.HeadsUpOmaha.BluntAxe;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AIGames.HeadsUpOmaha.UnitTests.Platform
{
	[TestFixture]
	public class ConsolePlatformTest
	{
		[Test]
		public void DoRun_Input003Txt_AreEqual()
		{
			var sb = new StringBuilder();
			var writer = new StringWriter(sb);
			var mock = new BotMock();
			mock.Actions.AddRange(InstructionTest.Read001Txt.ToGameActions(PlayerType.player2));

			using (var cp = new ConsolePlatformTestImpl())
			{
				cp.DoRun(mock, InstructionTest.Read001Txt);
			}
			var act = mock.Reactions;
			var exp = InstructionTest.Read001Txt.ToGameActions(PlayerType.player1).ToList();

			CollectionAssert.AreEqual(exp, act);
		}
	}

	public class ConsolePlatformTestImpl : ConsolePlatform
	{
		public ConsolePlatformTestImpl() { }
	}
	
	public class BotMock : IBot
	{
		public BotMock()
		{
			this.Reactions = new List<GameAction>();
			this.Actions = new List<GameAction>();
		}

		public GameAction Action(GameState state)
		{
			if (Current >= Actions.Count)
			{
				return GameAction.Fold;
			}
			return Actions[Current++];
		}
		public void Reaction(GameState state, GameAction reaction) { Reactions.Add(reaction); }
		public void Result(GameState state) { }
		public void FinalResult(GameState state) { }

		public List<GameAction> Actions { get; private set; }
		public int Current { get; set; }
		public List<GameAction> Reactions { get; private set; }
	}
}
