using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	[TestFixture]
	public class PlayerTypeTest
	{
		[Test]
		public void Other_Player1_IsPlayer2()
		{
			var act = PlayerType.player1.Other();
			var exp = PlayerType.player2;

			Assert.AreEqual(act, exp);
		}

		[Test]
		public void Other_Player2_IsPlayer1()
		{
			var act = PlayerType.player2.Other();
			var exp = PlayerType.player1;

			Assert.AreEqual(act, exp);
		}
	}
}
