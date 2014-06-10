using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.BluntAxe
{
	public class BluntAxeBot : IBot
	{
		public BluntAxeBot()
		{
			this.Rnd = new MT19937Generator(42);
		}
		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			var change = Calculate(state);

			if (state.AmountToCall == state.SmallBlind && change < 0.4)
			{
				return GameAction.Fold;
			}
			if (change > 0.75)
			{
				var raise = state.AmountToCall + this.Rnd.Next(state.BigBlind, state.MaxWinPot) * change * change;
				return GameAction.Raise((int)raise);
			}
			if (change < 0.35 && state.AmountToCall > 0)
			{
				var treshold = this.Rnd.Next();

				var situation = (double)(state.AmountToCall + state.Own.Pot) / (double)(state.Own.Pot + state.Own.Stack);

				if (treshold < situation)
				{
					return GameAction.Fold;
				}
			}

			if (state.AmountToCall > 0)
			{
				return GameAction.Call;
			}
			else
			{
				return GameAction.Check;
			}
		}
		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }

		private double Calculate(GameState state, int simulations = 500)
		{
			var other = Cards.Deck.ToList();
			var table = state.Table.ToList();

			var wins = 0.0;

			foreach (var card in state.Own.Hand) { other.Remove(card); }
			foreach (var card in table) { other.Remove(card); }

			for (int i = 0; i < simulations; i++)
			{
				other = other.OrderBy(c => Rnd.Next()).ToList();
				var tbl = table.ToList();

				var oppo = other.Take(4);
				tbl.AddRange(other.Skip(4).Take(5 - tbl.Count));

				var ownHand = PokerHand.CreateFromHeadsUpOmaha(tbl, state.Own.Hand);
				var oppHand = PokerHand.CreateFromHeadsUpOmaha(tbl, oppo);
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
			return 0.5 * wins / simulations;
		}
	}
}
