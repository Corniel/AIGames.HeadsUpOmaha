using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.ACDC
{
	public class Gauss
	{
		public static double GetZ(double x)
		{
			var a1 = 0.254829592;
			var a2 = -0.284496736;
			var a3 = 1.421413741;
			var a4 = -1.453152027;
			var a5 = 1.061405429;
			var p = 0.3275911;

			var sign = 1;
			if (x < 0)
			{
				sign = -1;
			}
			x = Math.Abs(x) / Math.Sqrt(2.0);

			var t = 1.0 / (1.0 + p * x);

			var y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);
			return 0.5 * (1.0 + sign * y);
		}
	}
}
