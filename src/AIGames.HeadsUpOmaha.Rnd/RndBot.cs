using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using System;

namespace AIGames.HeadsUpOmaha.Rnd
{
	public class RndBot : IBot
	{
		public RndBot()
		{
			this.Rnd = new Random(17);
		}

		protected Random Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			switch (Rnd.Next(0, 16))
			{
				case 0: return GameAction.Fold;
				case 1:
				case 2:
				case 3:
					var raise = Math.Max(state.Own.Stack, state.MaxWinPot);
					if (raise > 0)
					{
						return GameAction.Raise(raise);
					}
					else
					{
						return GameAction.Check;
					}
				default:
					if (state.AmountToCall > 0)
					{
						return GameAction.Call;
					}
					else
					{
						return GameAction.Check;
					}
			}
		}
		public void Reaction(GameState state, GameAction reaction) { }
		public void Result(GameState state) { }
	}
}
