﻿using System.Configuration;
using System.IO;

namespace AIGames.HeadsUpOmaha.Arena
{
	class Program
	{
		static void Main(string[] args)
		{
			var dir = new DirectoryInfo(ConfigurationManager.AppSettings["Bots.Dir"]);
            if (args != null && args.Length > 0)
            {
                dir = new DirectoryInfo(args[0]);
            }

            var arena = new CompetitionRunner(dir);

            try
            {
                arena.Bots = Bots.Load(dir);
            }
            catch { }

			while (arena.Run()) { }
		}
	}
}
