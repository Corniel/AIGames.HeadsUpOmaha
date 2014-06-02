using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace AIGames.HeadsUpOmaha.UnitTests.Platform
{
	[TestClass]
	public class ConsolePlatformTest
	{
		[TestMethod]
		public void DoRun_Input001Txt_AreEqual()
		{
			var sb = new StringBuilder();
			var writer = new StringWriter(sb);

			using (var cp = new ConsolePlatformMock(new StreamReader(AppConfig.GetTestFile("Input.001.txt").FullName), writer))
			{
				cp.DoRun(new BotMock());
			}

			var actions = sb.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None);

			Assert.AreEqual(58, actions.Length);
		}
	}

	public class ConsolePlatformMock : ConsolePlatform
	{
		public ConsolePlatformMock(TextReader reader, TextWriter writer) 
		: base(reader, writer){ }
	}

	public class BotMock : IBot
	{
		public GameAction Action(GameState state)
		{
			return GameAction.Fold;
		}
		public void Reaction(GameState state, GameAction reaction) { }

		public void Result(GameState state) { }
	}
}
