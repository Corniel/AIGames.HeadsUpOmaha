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
		public static DirectoryInfo DeployDir
		{
			get
			{
				return new DirectoryInfo(Path.Combine(AppDir.FullName, "..", "deploy"));
			}
		}

		public static DirectoryInfo HeadsUpOmahaDir
		{
			get
			{
				return new DirectoryInfo(Path.Combine(AppDir.FullName, "AIGames.HeadsUpOmaha"));
			}
		}

		public static DirectoryInfo BotDir
		{
			get
			{
				return new DirectoryInfo(ConfigurationManager.AppSettings["Bot.Dir"]);
			}
		}
		public static DirectoryInfo TestDir
		{
			get
			{
				return new DirectoryInfo(Path.Combine(AppDir.FullName, ConfigurationManager.AppSettings["Test.Dir"]));
			}
		}

		public static FileInfo GetTestFile(string filename)
		{
			return new FileInfo(Path.Combine(AppConfig.TestDir.FullName, filename));
		}
	}
}
