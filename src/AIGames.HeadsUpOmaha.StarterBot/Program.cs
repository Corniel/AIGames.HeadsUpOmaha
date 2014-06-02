using AIGames.HeadsUpOmaha.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.StarterBot
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ConsolePlatform.Run(new MyStarterBot());
		}
	}
}
