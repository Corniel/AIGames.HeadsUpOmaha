using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIGames.HeadsUpOmaha.Game;

namespace AIGames.HeadsUpOmaha.UnitTests
{
	[TestClass]
	public class StatisticalMathTest
	{
		[TestMethod]
		public void Permutation_5Over2Equals5Over3_AreEqual()
		{
			var act = StatisticalMath.Permutation(5, 2);
			var exp = StatisticalMath.Permutation(5, 3);

			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void Permutation_52Over5_AreEqual()
		{
			var act = StatisticalMath.Permutation(52, 5);
			var exp = PokerHand.HandTotal;

			Assert.AreEqual(exp, act);
		}
	}
}
