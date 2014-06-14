using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.UnitTests.Analysis
{
	[TestClass]
	public class UInt64CardsTest
	{
		[TestMethod]
		public void GetScore_StraightFlush_AreEqual()
		{
			var hand = UInt64Cards.Parse("[7c,Jc,8c,Tc,9c]");

			var act = hand.GetScore();

			//Assert.AreEqual("[Jc,Tc,9c,8c,7c]", act.ToString());
			Assert.AreEqual(PokerHandType.StraightFlush, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_FourOfAKind_AreEqual()
		{
			var hand = UInt64Cards.Parse("[Ac,6c,6d,6h,6s]");

			var act = hand.GetScore();

			//Assert.AreEqual("[6s,6h,6c,6d,Ac]", act.ToString());
			Assert.AreEqual(PokerHandType.FourOfAKind, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_FullHouse_AreEqual()
		{
			var hand = UInt64Cards.Parse("[5s,8d,8h,5d,5h]");

			var act = hand.GetScore();

			//Assert.AreEqual("[5s,5h,5d,8h,8d]", act.ToString());
			Assert.AreEqual(PokerHandType.FullHouse, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_Flush_AreEqual()
		{
			var hand = UInt64Cards.Parse("[3s,As,Ts,6s,Qs]");

			var act = hand.GetScore();

			//Assert.AreEqual("[As,Qs,Ts,6s,3s]", act.ToString());
			Assert.AreEqual(PokerHandType.Flush, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_Straight_AreEqual()
		{
			var hand = UInt64Cards.Parse("[5c,7s,6h,9s,8d]");

			var act = hand.GetScore();

			//Assert.AreEqual("[9s,8d,7s,6h,5c]", act.ToString());
			Assert.AreEqual(PokerHandType.Straight, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_StraightWithAce_AreEqual()
		{
			var hand = UInt64Cards.Parse("[Ac,2d,3c,5h,4d]");

			var act = hand.GetScore();

			//Assert.AreEqual("[5h,4d,3c,2d,Ac]", act.ToString());
			Assert.AreEqual(PokerHandType.Straight, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_ThreeOfAKind_AreEqual()
		{
			var hand = UInt64Cards.Parse("[3c,Qs,2s,3h,3d]");

			var act = hand.GetScore();

			//Assert.AreEqual("[3h,3c,3d,Qs,2s]", act.ToString());
			Assert.AreEqual(PokerHandType.ThreeOfAKind, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_TwoPair_AreEqual()
		{
			var hand = UInt64Cards.Parse("[9d,Kc,9h,Ks,Jh]");

			var act = hand.GetScore();

			//Assert.AreEqual("[Ks,Kc,9h,9d,Jh]", act.ToString());
			Assert.AreEqual(PokerHandType.TwoPair, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_OnePair_AreEqual()
		{
			var hand = UInt64Cards.Parse("[7h,2d,6c,2h,Qh]");

			var act = hand.GetScore();

			//Assert.AreEqual("[2h,2d,Qh,7h,6c]", act.ToString());
			Assert.AreEqual(PokerHandType.OnePair, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_HighCard_AreEqual()
		{
			var hand = UInt64Cards.Parse("[Th,Kh,2c,Qc,Ah]");

			var act = hand.GetScore();

			//Assert.AreEqual("[Ah,Kh,Qc,Th,2c]", act.ToString());
			Assert.AreEqual(PokerHandType.HighCard, act.ScoreType);
		}

		[TestMethod]
		public void GetScore_Speed_IsDoable()
		{
			var sw0 = new Stopwatch();
			var sw1 = new Stopwatch();

			var runs = 100000;
			var rnd = new MT19937Generator(17);

			var d = new UInt64Cards(0);

			var uPrev = default(UInt32PokerHand);
			var cPrev = default(PokerHand);
			var uCurr = default(UInt32PokerHand);
			var cCurr = default(PokerHand);

			int uCompare = 0;
			int cCompare = 0;

			Cards old = Cards.Empty;

			for (int i = 0; i < runs; i++)
			{
				var deck = Cards.GetShuffledDeck(rnd);
				var hand = deck.Take(5).ToList();

				sw0.Start();
				var ucards = UInt64Cards.Create(hand);
				uCurr = ucards.GetScore();
				uCompare = uCurr.CompareTo(uPrev);
				sw0.Stop();
				

				sw1.Start();
				cCurr = PokerHand.CreateFrom5(hand);
				cCompare = PokerHandComparer.Instance.Compare(cCurr, cPrev);
				sw1.Stop();


				if (Math.Sign(cCompare) != Math.Sign(uCompare))
				{
					var log = Cards.Create(hand);
					Assert.Fail("Old: {0:f}, New: {1:f}", old, log);
				}
				old = Cards.Create(hand);
				uPrev = uCurr;
				cPrev = cCurr;
			}

			Console.WriteLine("Avarage: {0:#,##0.00} Ticks/hand", (double)sw1.ElapsedTicks / (double)runs);
			Console.WriteLine("Avarage: {0:#,##0.00} Ticks/hand new", (double)sw0.ElapsedTicks / (double)runs);
		}

		[TestMethod]
		public void Generate_Permuations_Successful()
		{
			var p4_2 = new List<uint>();
			for (uint i = 0; i < Bits.Flag[4]; i++)
			{
				if (Bits.Count(i) == 2) { p4_2.Add(i); }
			}
			Console.WriteLine(String.Join(",", p4_2));
			Assert.AreEqual(6, p4_2.Count);

			var p5_3 = new List<uint>();
			for (uint i = 0; i < Bits.Flag[5]; i++)
			{
				if (Bits.Count(i) == 3) { p5_3.Add(i); }
			}
			Console.WriteLine(String.Join(",", p5_3));
			Assert.AreEqual(10, p5_3.Count);
		}
	}
}
