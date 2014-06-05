using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using System;
using System.Linq;

namespace AIGames.HeadsUpOmaha.PlayBetterHandsOnly
{
	public class PlayBetterHandsOnlyBot : IBot
	{
		private Random rnd = new Random();
		private const int Simulations = 1024;

		public GameAction Action(GameState state)
		{
			if (state.AmountToCall > 0)
			{
				var other = Cards.Deck.ToList();
				var table = state.Table.ToList();

				var wins = 0;

				foreach (var card in state.Own.Hand) { other.Remove(card); }
				foreach (var card in table) { other.Remove(card); }

				for (int i = 0; i < Simulations; i++)
				{
					other = other.OrderBy(c => rnd.Next()).ToList();
					var tbl = table.ToList();

					var oppo = other.Take(4);
					tbl.AddRange(other.Skip(4).Take(5 - tbl.Count));

					var ownHand = PokerHand.CreateFromHeadsUpOmaha(table, state.Own.Hand);
					var oppHand = PokerHand.CreateFromHeadsUpOmaha(table, oppo);
					var compare = PokerHandComparer.Instance.Compare(ownHand, oppHand);

					if (compare > 0)
					{
						wins += 2;
					}
					else if (compare == 0)
					{
						wins++;
					}

				}
				if (wins < Simulations)
				{
					return GameAction.Fold;
				}
				return GameAction.Call;
			}

			return GameAction.Check;
		}

		public void Reaction(GameState state, GameAction reaction) { }

		public void Result(GameState state) { }
	}
}
