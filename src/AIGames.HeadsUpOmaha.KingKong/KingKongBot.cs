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
			var pWin = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd, 1000);

			// Only play doable small blinds.
			if (state.IsPreFlop && pWin < 0.4 && state.AmountToCall == state.SmallBlind)
			{
				return GameAction.Fold;
			}
			// Don't play bad hands on pre flop if we have to call.
			if (state.IsPreFlop && pWin < 0.3 && state.AmountToCall >= state.BigBlind)
			{
				return GameAction.Fold;
			}
			if (pWin > 0.55 && state.AmountToCall == 0)
			{
				return GameAction.Raise(state.MaxinumRaise);
			}
			if (pWin > 0.7)
			{
				return GameAction.Raise(state.MaxinumRaise);
			}
			if (pWin < 0.35 && state.AmountToCall > 0)
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