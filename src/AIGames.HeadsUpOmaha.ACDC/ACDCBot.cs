using AIGames.HeadsUpOmaha.Analysis;
using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.ACDC
{
	public class ACDCBot : IBot
	{
		public ACDCBot()
		{
			this.Rnd = new MT19937Generator();
			this.Opponent = new OpponentBot();

			try
			{
				this.Data = ActionData.Load("data.xml");
			}
			catch
			{
				this.Data = new ActionData();
			}
		}

		/// <summary>Gets the opponent bot.</summary>
		public OpponentBot Opponent { get; protected set; }

		/// <summary>The randomizer.</summary>
		public MT19937Generator Rnd { get; set; }

		///// <summary>Gets the action data.</summary>
		public ActionData Data { get; protected set; }

		/// <summary>The action of the the bot.</summary>
		/// <param name="state">
		/// The current state.
		/// </param>
		public GameAction Action(GameState state)
		{
			var pWin = PokerHandEvaluator.Calculate(state.Own.Hand, state.Table, this.Rnd, 1000);

			// Only play doable small blinds.
			if (state.IsPreFlop)
			{
				return PreFlopAction(state, pWin);
			}

			if (pWin > 0.8)
			{
				return GameAction.Raise(state.MaxinumRaise);
			}
			if (pWin > 0.7)
			{
				return GameAction.Raise(state.MinimumRaise);
			}
			if (pWin > 0.55 && state.AmountToCall == 0)
			{
				return GameAction.Raise(state.MinimumRaise);
			}
			
			if (pWin < 0.35 && state.AmountToCall > 0)
			{
				return GameAction.Fold;
			}
			
			if (state.AmountToCall > 0)
			{
				return GameAction.Call;
			}
			return GameAction.Check;
		}
			
		public GameAction PreFlopAction(GameState state, double pWin)
		{
			// Only play doable small blinds.
			if (pWin < 0.4 && state.AmountToCall > 0)
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
		
		/// <summary>The reaction of the opponent.</summary>
		/// <param name="state">
		/// The current state.
		/// </param>
		/// <param name="reaction">
		/// The action.
		/// </param>
		public void Reaction(GameState state, GameAction reaction)
		{
			this.Data.Add(state, reaction);
		}

		/// <summary>The state when a result (win, loss, draw) made.</summary>
		public void Result(GameState state) 
		{
			this.Data.Update(state, this.Rnd);
			this.Data.Save("data.xml");
		}

		/// <summary>The state when a final result (first, second) was made.</summary>
		public void FinalResult(GameState state) { }
	}
}
