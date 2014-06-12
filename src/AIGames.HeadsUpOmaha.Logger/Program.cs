using AIGames.HeadsUpOmaha.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.Logger
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ConsolePlatform.Run(new Log4netBot());
		}
	}
}
