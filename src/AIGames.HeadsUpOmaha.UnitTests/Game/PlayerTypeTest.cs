using AIGames.HeadsUpOmaha.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	[TestClass]
	public class PlayerTypeTest
	{
		[TestMethod]
		public void Other_Player1_IsPlayer2()
		{
			var act = PlayerType.player1.Other();
			var exp = PlayerType.player2;

			Assert.AreEqual(act, exp);
		}

		[TestMethod]
		public void Other_Player2_IsPlayer1()
		{
			var act = PlayerType.player2.Other();
			var exp = PlayerType.player1;

			Assert.AreEqual(act, exp);
		}
	}
}
