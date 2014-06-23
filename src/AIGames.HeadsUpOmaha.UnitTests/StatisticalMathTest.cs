using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;

namespace AIGames.HeadsUpOmaha.UnitTests
{
	[TestFixture]
	public class StatisticalMathTest
	{
		[Test]
		public void Permutation_5Over2Equals5Over3_AreEqual()
		{
			var act = StatisticalMath.Permutation(5, 2);
			var exp = StatisticalMath.Permutation(5, 3);

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Permutation_52Over5_AreEqual()
		{
			var act = StatisticalMath.Permutation(52, 5);
			var exp = PokerHand.HandTotal;

			Assert.AreEqual(exp, act);
		}
	}
}
