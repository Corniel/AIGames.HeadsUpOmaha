using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.KingKong
{
	public class KingKongBot : IBot
	{
		public static void Main(string[] args) { ConsolePlatform.Run(new KingKongBot()); }

		public KingKongBot()
		{
			this.Rnd = new MT19937Generator();
		}
		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			var pWin = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd, 5000);

			if (state.IsPreFlop && pWin < 0.40 && state.AmountToCall > 0)
			{
				return GameAction.Fold;
			}
			if (state.MaxinumRaise > 0)
			{
				if (pWin > 0.65 && state.AmountToCall == 0)
				{
					return GameAction.Raise(state.MaxinumRaise);
				}
				if (pWin > 0.55 && state.AmountToCall == 0)
				{
					return GameAction.Raise(state.MinimumRaise);
				}
				if (pWin > 0.7)
				{
					return GameAction.Raise(state.MaxinumRaise);
				}
			}
			if (pWin < 0.35 && state.AmountToCall > 0)
			{
				return GameAction.Fold;
			}

			return GameAction.CheckOrCall(state);
		}
		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }
		public void FinalResult(GameState state) { }
	}
}