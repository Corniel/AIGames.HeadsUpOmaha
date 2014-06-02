using AIGames.HeadsUpOmaha.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	[TestClass]
	public class CardTest
	{
		[TestMethod]
		public void Empty_None_StringEmpty()
		{
			var act = Card.Empty.ToString();
			var exp = "";

			Assert.AreEqual(exp, act.ToString());
		}

		[TestMethod]
		public void Create_H12Hearts_QueenOfHearts()
		{
			var act = Card.Create(12, CardSuit.Hearts);
			var exp = "Qh";

			Assert.AreEqual(exp, act.ToString());
		}


		[TestMethod]
		public void Parse_Td_10Diamond()
		{
			var act = Card.Parse("Td");

			IsCard(10, CardSuit.Diamonds, act);
		}

		[TestMethod]
		public void Parse_As_AceOfSpades()
		{
			var act = Card.Parse("As");

			IsCard(14, CardSuit.Spades, act);
		}

		[TestMethod]
		public void ParseSet_5Cards_AreEqual()
		{
			var act = Cards.Parse("[7s,Js,3h,5d,9s]").ToArray();

			Assert.AreEqual(5, act.Length, "Length");

			IsCard(7, CardSuit.Spades, act[0]);
			IsCard(11, CardSuit.Spades, act[1]);
			IsCard(3, CardSuit.Hearts, act[2]);
			IsCard(5, CardSuit.Diamonds, act[3]);
			IsCard(9, CardSuit.Spades, act[4]);
		}


		private static void IsCard(int expHeight, CardSuit expSuit, Card act)
		{
			Assert.AreEqual(expHeight, act.Height, "Height");
			Assert.AreEqual(expSuit, act.Suit, "Suit");
		}
	}
}
