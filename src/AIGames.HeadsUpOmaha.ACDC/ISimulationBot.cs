using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.ACDC
{
	public interface ISimulationBot
	{
		GameAction Action(GameState state);
	}
}
