using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.PlayBetterHandsOnly
{
	public class PlayBetterHandsOnlyBot : IBot
	{
		public static void Main(string[] args)
		{
			ConsolePlatform.Run(new PlayBetterHandsOnlyBot());
		}

		public PlayBetterHandsOnlyBot()
		{
			this.Rnd = new MT19937Generator();
		}
		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			var chance = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd, 200);

			if (state.AmountToCall == state.SmallBlind)
			{
				if (chance < 0.4)
				{
					return GameAction.Fold;
				}
				return GameAction.Call;
			}
			if (state.AmountToCall >= state.BigBlind)
			{
				if (chance < 0.5)
				{
					return GameAction.Fold;
				}
				return GameAction.Call;
			}
			if(chance > 0.65 && state.MaxinumRaise > 0)
			{
				int raise = (int)(chance * 100) - 60 + state.BigBlind;
				if(raise <= state.MaxinumRaise)
				{
					return GameAction.Raise(raise);
				}
			}
			return GameAction.Check;
		}

		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }
		public void FinalResult(GameState state) { }
	}
}
