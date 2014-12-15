using AIGames.HeadsUpOmaha.ACDC;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.UnitTests.ACDC
{
	[TestFixture]
	public class GaussTest
	{
		[Test]
		public void GetZ_Range_AreEqual()
		{
			var tests = new double[]{-4, -3, -2, -1 -0.5, 0, 0.5, 1, 2, 3, 4};

			var exp = new double[] { 0, 0.0013, 0.0228, 0.0668, 0.5, 0.6915, 0.8413, 0.9772, 0.9987, 1 };

			for (var i = 0; i < tests.Length; i++)
			{
				var z = Math.Round(Gauss.GetZ(tests[i]), 4);

				Console.WriteLine(z);

				Assert.AreEqual(exp[i], z, "GetZ({0}) should lead to {1}.", tests[i], exp[i]);
			}
		}
	}
}
