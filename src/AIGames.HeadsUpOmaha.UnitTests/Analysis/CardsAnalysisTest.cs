using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.Analysis
{
	[TestFixture]
	public class CardsAnalysisTest
	{
		[Test]
		public void Distribution_SomRuns_ShouldBeFairlyEqual()
		{
			var distribution = new Dictionary<Card, int[]>();
			foreach (var card in Cards.Deck)
			{
				distribution[card] = new int[9];
			}

			var runs = 52 * 100;
			var rnd = new MT19937Generator(17);

			for (var run = 0; run < runs; run++)
			{
				var shuffle = Cards.GetShuffledDeck(rnd);

				var i = 0;
				foreach (var card in shuffle.Take(9))
				{
					distribution[card][i++]++;
				}
			}

			foreach (var kvp in distribution)
			{
				Console.WriteLine("{0}\t{1}", kvp.Key, String.Join("\t", kvp.Value));
			}

		}
	}
}
