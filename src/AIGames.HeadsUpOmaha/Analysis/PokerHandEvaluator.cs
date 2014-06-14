using AIGames.HeadsUpOmaha.Game;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.Analysis
{
	public class PokerHandEvaluator
	{
		public static double Calculate(Cards hand, Cards table, MT19937Generator rnd, int runs = 4000)
		{
			UInt64Cards[] tblSub = null;

			var tAdd = 5 - table.Count;
			var uhnd = UInt64Cards.Create(hand);
			var utbl = UInt64Cards.Create(table);
			var dead = uhnd.Combine(utbl);

			var ownSub = uhnd.GetHandSubsets();

			var score = 0;

			if (tAdd == 0)
			{
				tblSub = utbl.GetTableSubsets();
			}

			for (var i = 0; i < runs; i++)
			{
				var tbl = utbl;
				if (tAdd > 0)
				{
					tbl = tbl.Combine(UInt64Cards.GetRandom(dead, tAdd, rnd));
					tblSub = tbl.GetTableSubsets();
				}
				var uopp = UInt64Cards.GetRandom(dead.Combine(tbl), 4, rnd);
				var oppSub = uopp.GetHandSubsets();

				score += UInt32PokerHand.Compare(ownSub, oppSub, tblSub);
			}
			var e = 0.5 + 0.5 * (double)(score) / (double)runs;
			return e;
		}
	}
}
