using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.UnitTests.Analysis
{
	[TestFixture]
	public class PokerHandEvaluatorTest
	{
		[Test, Ignore]
		public void Caculate_StartHands_50PercentOnAverage()
		{
			var runs = 100000;
			var rnd = new MT19937Generator(17);

			var results = new List<double>();

			var distribution = new Dictionary<decimal, int>();

			for (var i = 0.1m; i < 0.9m;i+= 0.005m)
			{
				distribution[i] = 0;

			}
				for (var run = 0; run < runs; run++)
				{
					var shuffled = Cards.GetShuffledDeck(rnd);

					var own = Cards.Create(shuffled.Take(4));
					var p = PokerHandEvaluator.Calculate(own, Cards.Empty, rnd, 1000);
					results.Add(p);

					decimal per = (decimal)Math.Round(p * 2, 2) / 2.0m;
					distribution[per]++;
				}

			var avg = results.Average();
			Console.WriteLine(avg);
			Assert.IsTrue(avg > 0.48, "Avg min {0}", avg);
			Assert.IsTrue(avg < 0.52, "Avg max {0}", avg);

			foreach (var kvp in distribution)
			{
				Console.WriteLine("{0:0.0%}\t{1}", kvp.Key, kvp.Value);
			}
			
		}
		[Test]
		public void Calculate_SpeedTable0_IsDoable()
		{
			var sw = new Stopwatch();

			var runs = 100000;
			var rnd = new MT19937Generator(17);

			var shuffled = Cards.GetShuffledDeck(rnd);
			var hand = Cards.Create(shuffled.Take(4));
			var table = Cards.Empty;

			sw.Start();

			var score = PokerHandEvaluator.Calculate(hand, table, rnd, runs);

			sw.Stop();

			Console.WriteLine("Avarage: {0:#,##0.00} runs/ms", (double)runs / (double)sw.ElapsedMilliseconds);
		}
		[Test]
		public void Calculate_SpeedTable3_IsDoable()
		{
			var sw = new Stopwatch();

			var runs = 100000;
			var rnd = new MT19937Generator(17);

			var shuffled = Cards.GetShuffledDeck(rnd);
			var hand = Cards.Create(shuffled.Take(4));
			var table = Cards.Create(shuffled.Skip(4).Take(3));

			sw.Start();

			var score = PokerHandEvaluator.Calculate(hand, table, rnd, runs);

			sw.Stop();

			Console.WriteLine("Avarage: {0:#,##0.00} runs/ms", (double)runs / (double)sw.ElapsedMilliseconds);
		}
		[Test]
		public void Calculate_SpeedTable4_IsDoable()
		{
			var sw = new Stopwatch();

			var runs = 100000;
			var rnd = new MT19937Generator(17);

			var shuffled = Cards.GetShuffledDeck(rnd);
			var hand = Cards.Create(shuffled.Take(4));
			var table = Cards.Create(shuffled.Skip(4).Take(4));

			sw.Start();

			var score = PokerHandEvaluator.Calculate(hand, table, rnd, runs);

			sw.Stop();

			Console.WriteLine("Avarage: {0:#,##0.00} runs/ms", (double)runs / (double)sw.ElapsedMilliseconds);
		}
		[Test]
		public void Calculate_SpeedTable5_IsDoable()
		{
			var sw = new Stopwatch();

			var runs = 100000;
			var rnd = new MT19937Generator(17);

			var shuffled = Cards.GetShuffledDeck(rnd);
			var hand = Cards.Create(shuffled.Take(4));
			var table = Cards.Create(shuffled.Skip(4).Take(5));

			sw.Start();

			var score = PokerHandEvaluator.Calculate(hand, table, rnd, runs);

			sw.Stop();

			Console.WriteLine("Avarage: {0:#,##0.00} runs/ms", (double)runs / (double)sw.ElapsedMilliseconds);
		}

		[Test]
		public void Calculate_2d2c2h2sTable0_AreEqual()
		{
			var rnd = new MT19937Generator(17);

			var hand = Cards.Parse("[2d,2c,2h,2s]");
			var table = Cards.Empty;

			//var ch = OmahaHand.Calculate(hand, table, 1024, rnd);
			var ch = PokerHandEvaluator.Calculate(hand, table, rnd, 100000);

			Assert.AreEqual(0.09, ch, 0.01);
		}

		[Test]
		public void Calculate_2d3c5h6sTable0_AreEqual()
		{
			var rnd = new MT19937Generator(17);

			var hand = Cards.Parse("[2d,3c,5h,6s]");
			var table = Cards.Empty;

			//var ch = OmahaHand.Calculate(hand, table, 1024, rnd);
			var ch = PokerHandEvaluator.Calculate(hand, table, rnd, 100000);

			Assert.AreEqual(0.357, ch, 0.01);
		}

		[Test]
		public void Calculate_Tc9c8dQhTable0_AreEqual()
		{
			var rnd = new MT19937Generator(17);

			var hand = Cards.Parse("[Tc,9c,8d,Qh]");
			var table = Cards.Empty;

			//var ch = OmahaHand.Calculate(hand, table, 1024, rnd);
			var ch = PokerHandEvaluator.Calculate(hand, table, rnd, 100000);

			Assert.AreEqual(0.536, ch, 0.01);
		}

		[Test]
		public void Calculate_AcAdKCKdTable0_AreEqual()
		{
			var rnd = new MT19937Generator(17);

			var hand = Cards.Parse("[Ac,Ad,Kc,Kd]");
			var table = Cards.Empty;

			//var ch = OmahaHand.Calculate(hand, table, 1024, rnd);
			var ch = PokerHandEvaluator.Calculate(hand, table, rnd, 100000);

			Assert.AreEqual(0.706, ch, 0.01);
		}

		[Test]
		public void Calculate_Tc9c8dQhTable3_AreEqual()
		{
			var rnd = new MT19937Generator(17);

			var hand = Cards.Parse("[Tc,9c,8d,Qh]");
			var table = Cards.Parse("[Jc,2d,4s]");

			//var ch = OmahaHand.Calculate(hand, table, 1024, rnd);
			var ch = PokerHandEvaluator.Calculate(hand, table, rnd, 100000);

			Assert.AreEqual(0.309, ch, 0.01);
		}

		[Test]
		public void Calculate_Tc9c8dQhTable4_AreEqual()
		{
			var rnd = new MT19937Generator(17);

			var hand = Cards.Parse("[Tc,9c,8d,Qh]");
			var table = Cards.Parse("[Jc,2d,4s,3h]");

			//var ch = OmahaHand.Calculate(hand, table, 1024, rnd);
			var ch = PokerHandEvaluator.Calculate(hand, table, rnd, 100000);

			Assert.AreEqual(0.096, ch, 0.01);
		}
		[Test]
		public void Calculate_Tc9c8dQhTable5_AreEqual()
		{
			var rnd = new MT19937Generator(17);

			var hand = Cards.Parse("[Tc,9c,8d,Qh]");
			var table = Cards.Parse("[Qd,Jc,2d,4s,3h]");

			//var ch = OmahaHand.Calculate(hand, table, 1024, rnd);
			var ch = PokerHandEvaluator.Calculate(hand, table, rnd, 100000);

			Assert.AreEqual(0.382, ch, 0.01);
		}
	}
}
