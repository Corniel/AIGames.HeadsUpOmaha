using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.Rnd
{
	public class RndBot : IBot
	{
		public RndBot()
		{
			this.Rnd = new MT19937Generator(17);
		}

		protected MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			switch (Rnd.Next(0, 8))
			{
				case 0: return FoldOnRequiredCall(state);
				case 1:
				case 2:
				case 3: return RaiseIfPossible(state);
				default: return GameAction.CheckOrCall(state);
			}
		}

		private GameAction RaiseIfPossible(GameState state)
		{
			if (state.MaxinumRaise > 0)
			{
				var raise = Rnd.Next(state.MinimumRaise, state.MaxinumRaise + 1);
				return GameAction.Raise(raise);
			}
			return GameAction.CheckOrCall(state);
		}
		private GameAction FoldOnRequiredCall(GameState state)
		{
			if (state.AmountToCall > 0)
			{
				return GameAction.Fold;
			}
			return GameAction.Check;
		}

		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }
		public void FinalResult(GameState state) { }
	}
}
