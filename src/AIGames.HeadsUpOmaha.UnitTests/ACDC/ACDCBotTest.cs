using AIGames.HeadsUpOmaha.ACDC;
using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.UnitTests.ACDC
{
	[TestFixture]
	public class ACDCBotTest
	{
		[Test]
		public void GetP_Range_AreEqual()
		{
			var state = new GameState();

			var tests = new int[] { 60, 100, 300, 400, 1000, 1800, 1900, 2000, 2100, 3000, 19 };

			var exp = new double[] { 0.0228, 0.0251, 0.0398, 0.0495, 0.1513, 0.4183, 0.4589, 0.5, 0.5411, 0.8487, 0 };

			for (var i = 0; i < tests.Length; i++)
			{
				var z = Math.Round(ACDCBot.GetP(tests[i], state), 4);

				Console.WriteLine(z);

				Assert.AreEqual(exp[i], z, "GetZ({0}) should lead to {1}.", tests[i], exp[i]);
			}
		}
	}
}
