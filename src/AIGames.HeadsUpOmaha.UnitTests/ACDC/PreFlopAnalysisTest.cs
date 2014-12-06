using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.UnitTests.ACDC
{
	[TestFixture]
	public class PreFlopAnalysisTest
	{
		[Test]
		public void Test()
		{
			var ranges = new Dictionary<int, int>();
			for (var i = 0; i <= 100; i++)
			{
				ranges[i] = 0;
			}

			var rnd = new MT19937Generator();

			var sets = 50000;
			var tests = 1000;

			for (var set = 0; set < sets; set++)
			{
				var shuffled = Cards.GetShuffledDeck(rnd);
				var hand = Cards.Create(shuffled.Take(4));

				var p = PokerHandEvaluator.Calculate(hand, Cards.Empty, rnd, tests);

				int index = (int)Math.Round(100 * p);

				ranges[index]++;
			}

			foreach (var kvp in ranges)
			{
				Console.WriteLine("{0}%\t{1}", kvp.Key, kvp.Value);
			}
		}
	}
}
