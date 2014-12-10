using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.AntiPasta
{
	public class AntiPastaBot : IBot
	{
		public static void Main(string[] args) { ConsolePlatform.Run(new AntiPastaBot()); }

		public AntiPastaBot()
		{
			this.Rnd = new MT19937Generator();
		}
		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			var change = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd);

			if (change > 0.8 && state.MaxinumRaise > 0)
			{
				return GameAction.Raise(state.MaxinumRaise);
			}

			if (state.IsPreFlop && state.AmountToCall  == state.SmallBlind &&  change < 0.52)
			{
				return GameAction.Fold;
			}
			if (state.AmountToCall > 0 && change < 0.51)
			{
				return GameAction.Fold;
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
		public void FinalResult(GameState state) { }
	}
}
