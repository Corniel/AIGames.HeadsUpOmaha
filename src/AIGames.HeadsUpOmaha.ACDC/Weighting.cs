using AIGames.HeadsUpOmaha.Game;

namespace AIGames.HeadsUpOmaha.ACDC
{
	public class Weighting
	{
		public static double ToWeight(double l, double r)
		{
			if (r == 0.0) { return 0.0; }
			var w = l / r;
			return w > 1.0 ? r / l : w;
		}
	}
}
