using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.Arena
{
	public static class ArenaConfig
	{
		/// <summary>Gets the bots directory.</summary>
		public static DirectoryInfo BotsDirectory
		{
			get
			{
				try
				{
					return new DirectoryInfo(ConfigurationManager.AppSettings["Bots.Dir"]);
				}
				catch
				{
					return new DirectoryInfo("bots");
				}
			}
		}
		
		/// <summary>Gets the initial value of K.</summary>
		public static double KInitial
		{
			get
			{
				double k;

				if(double.TryParse(ConfigurationManager.AppSettings["K.Initial"], NumberStyles.Number, CultureInfo.InvariantCulture, out k))
				{
					return k;
				}
				return 40.0;
			}
		}
		/// <summary>Gets the stable value of K.</summary>
		public static double KStable
		{
			get
			{
				double k;

				if(double.TryParse(ConfigurationManager.AppSettings["K.Stable"], NumberStyles.Number, CultureInfo.InvariantCulture, out k))
				{
					return k;
				}
				return 10.0;
			}
		}

		/// <summary>Gets the stablizer factor of K.</summary>
		/// <remarks>
		/// Should be less then 1.0 and bigger dan 0.0.
		/// </remarks>
		public static double KStabalizer
		{
			get
			{
				double k;

				if (double.TryParse(ConfigurationManager.AppSettings["K.Stabalizer"], NumberStyles.Number, CultureInfo.InvariantCulture, out k))
				{
					return k;
				}
				return 0.98;
			}
		}

		/// <summary>Gets the initial value of K.</summary>
		public static Elo EloInitial
		{
			get
			{
				Elo elo;

				if (Elo.TryParse(ConfigurationManager.AppSettings["Elo.Initial"], CultureInfo.InvariantCulture, out elo))
				{
					return elo;
				}
				return 1400.0;
			}
		}
	}
}
