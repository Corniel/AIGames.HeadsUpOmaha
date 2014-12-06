using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.ACDC
{
	public class OpponentBot : ISimulationBot
	{
		public OpponentBot()
		{
			this.Rnd = new MT19937Generator();

			this.Data = new ActionData();
		}

		public ActionData Data { get; protected set; }

		/// <summary>The randomizer.</summary>
		public MT19937Generator Rnd { get; set; }

		public GameAction Action(GameState state)
		{
			var rnd = Rnd.NextDouble();

			var threadhold = Data.GetFoldRate(state);
			if (rnd < threadhold) { return GameAction.Fold; }

			threadhold+= Data.GetCheckRate(state);
			if (rnd < threadhold) { return GameAction.Check; }

			threadhold+= Data.GetCallRate(state);
			if (rnd < threadhold) { return GameAction.Check; }

			int raise = Data.GetRaiseRate(state, Rnd);
			return GameAction.Raise(raise);
		}
	}
}
