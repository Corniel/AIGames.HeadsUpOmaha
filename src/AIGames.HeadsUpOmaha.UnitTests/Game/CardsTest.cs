using AIGames.HeadsUpOmaha.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	[TestClass]
	public class CardsTest
	{
		public static readonly Cards TestInstance = Cards.Parse("[Tc,8d,Ah,7s,8s]");

		[TestMethod]
		public void ToString_f_AreEqual()
		{
			var act = TestInstance.ToString("f");
			var exp = "[10♣,8♦,A♥,7♠,8♠]";

			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void Copy_TestInstance_AreEqualNotIdentical()
		{
			var exp = TestInstance;
			var act = exp.Copy();

			Assert.AreEqual(exp.ToString(), act.ToString());
			Assert.AreNotSame(exp, act);
		}
	}
}
