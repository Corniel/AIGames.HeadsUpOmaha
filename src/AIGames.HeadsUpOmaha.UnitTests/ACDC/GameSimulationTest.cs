using AIGames.HeadsUpOmaha.ACDC;
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
	public class GameSimulationTest
	{
		[Test]
		public void Simulate_Starting_Result()
		{
			var subScore = new Dictionary<int, double>();
			var subCount = new Dictionary<int, int>();


			var rnd = new MT19937Generator(17);

			var score = 0.0;
			var runs = 200000;

			var len = 0.0;

			for (var run = 0; run < runs; run++)
			{
				var state = new GameState(new Settings());
				state.OnButton = (run & 1) == 1 ? PlayerType.player1 : PlayerType.player2;

				var simulator = new GameSimulation();

				var subs = new List<int>();
				var sc = simulator.GetScore(state, rnd, subs);
				score += sc;

				foreach (var sub in subs)
				{
					var key0 = 20 * ((10 + sub) / 20);
					var key1 = 4000 - key0;

					if (!subCount.ContainsKey(key0))
					{
						subScore[key0] = 0;
						subCount[key0] = 0;
					}
					if (!subCount.ContainsKey(key1))
					{
						subScore[key1] = 0;
						subCount[key1] = 0;
					}
					subScore[key0]+= sc;
					subCount[key0]+= 1;

					subScore[key1] += 1.0-sc;
					subCount[key1] += 1;
				}

				len += subs.Count;
			}
			var act = score / runs;
			var actLen = len / runs;

			Console.WriteLine();

			foreach (var key in subCount.Keys.OrderBy(s => s))
			{
				Console.WriteLine("{0}\t{1:0.0%}\t{2}", key, subScore[key]/subCount[key], subCount[key]);
			}

			Console.WriteLine("score: {0:0.0%}, len: {1:0.0}", act, actLen);

			var exp = 1.0;
			Assert.AreEqual(exp, act);
		}
	}
}
