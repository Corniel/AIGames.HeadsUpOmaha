using System.Configuration;
using System.IO;

namespace AIGames.HeadsUpOmaha.UnitTests
{
	public static class AppConfig
	{
		public static DirectoryInfo AppDir
		{
			get
			{
				return new DirectoryInfo(ConfigurationManager.AppSettings["App.Dir"]);
			}
		}
		public static FileInfo GetTestFile(string filename)
		{
			return new FileInfo(Path.Combine(ConfigurationManager.AppSettings["Test.Dir"], filename));
		}
	}
}
