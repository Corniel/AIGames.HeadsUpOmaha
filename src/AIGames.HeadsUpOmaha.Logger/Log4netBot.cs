using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using log4net;

namespace AIGames.HeadsUpOmaha.Logger
{
	public class Log4netBot : IBot
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Log4netBot));

		public GameAction Action(GameState state)
		{
			var action = state.AmountToCall > 0 ? GameAction.Call : GameAction.Check;

			log.InfoFormat("Action[{0}]: {1}", state.Round, action);

			return action;
		}
		public void Reaction(GameState state, GameAction reaction)
		{
			log.InfoFormat("Reaction[{0}]: {1}", state.Round, reaction);
		}
		public void Result(GameState state)
		{
			log.InfoFormat("Result[{0}]: {1}", state.Round, state.Result);
		}
		public void FinalResult(GameState state) { }
	}
}
