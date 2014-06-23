using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	[TestFixture]
	public class CardPairsTest
	{
		[Test]
		public void Best_Empty_AreEqual()
		{
			var pairs = CardPairs.Create(Cards.Empty);

			var act = pairs.Best;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Best_56KQ_AreEqual()
		{
			var cards = Cards.Parse("[5c,6c,Kh,Qc]");
			var pairs = CardPairs.Create(cards);

			var act = pairs.Best;
			var exp = 13;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Best_56KK_AreEqual()
		{
			var cards = Cards.Parse("[5c,6c,Kh,Kc]");
			var pairs = CardPairs.Create(cards);

			var act = pairs.Best;
			var exp = 13;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Second_56KQ_AreEqual()
		{
			var cards = Cards.Parse("[5c,6c,Kh,Qc]");
			var pairs = CardPairs.Create(cards);

			var act = pairs.Second;
			var exp = 12;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Second_56KK_AreEqual()
		{
			var cards = Cards.Parse("[5c,6c,Kh,Kc]");
			var pairs = CardPairs.Create(cards);

			var act = pairs.Second;
			var exp = 13;

			Assert.AreEqual(exp, act);
		}
	}
}
