using AIGames.HeadsUpOmaha.Game;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.Analysis
{
	/// <summary>Evaluates poker hands.</summary>
	public class PokerHandEvaluator
	{
		/// <summary>Calculates the winning change of a hand.</summary>
		public static PokerHandOutcome Calculate(Cards hand, Cards table, MT19937Generator rnd, int runs = 1000)
		{
			UInt64Cards[] tblSub = null;

			var tAdd = 5 - table.Count;
			var uhnd = UInt64Cards.Create(hand);
			var utbl = UInt64Cards.Create(table);
			var dead = uhnd.Combine(utbl);

			var ownSub = uhnd.GetHandSubsets();

			var w = 0;
			var d = 0;
			var l = 0;

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

				var c = UInt32PokerHand.Compare(ownSub, oppSub, tblSub);
				if (c > 0) { w++; }
				else if (c < 0) { l++; }
				else { d++; }
			}
			return new PokerHandOutcome(w, d, l);
		}
	}
}
