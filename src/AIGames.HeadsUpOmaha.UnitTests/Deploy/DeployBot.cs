using AIGames.HeadsUpOmaha.BotDeployment;
using NUnit.Framework;
using System.IO;

namespace AIGames.HeadsUpOmaha.UnitTests.Deploy
{
	[TestFixture]
	public class DeployBot
	{
#if DEBUG
		private const bool IsDebugDeploy = true;
#else
		private const bool IsDebugDeploy = false;
#endif

		[Test]
		public void Deploy_AllIn_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.AllIn"));
			Deployer.Run(collectDir, "AllIn", "0003", IsDebugDeploy);
		}
		[Test]
		public void Deploy_BluntAxe_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.BluntAxe"));
			Deployer.Run(collectDir, "BluntAxe", "0012", IsDebugDeploy);
		}
		[Test]
		public void Deploy_Checkers_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.Checkers"));
			Deployer.Run(collectDir, "Checkers", "0002", IsDebugDeploy);
		}
		[Test]
		public void Deploy_Chicken_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.Chicken"));
			Deployer.Run(collectDir, "Chicken", "0003", IsDebugDeploy);
		}
		[Test]
		public void Deploy_KingKong_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.KingKong"));
			Deployer.Run(collectDir, "KingKong", "0013", IsDebugDeploy);
		}
#if DEBUG
		[Test]
		public void Deploy_Log4netBot_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.Logger"));
			Deployer.Run(collectDir, "Log4netBot", "", IsDebugDeploy);
		}
#endif
		[Test]
		public void Deploy_PlayBetterHandsOnly_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.PlayBetterHandsOnly"));
			Deployer.Run(collectDir, "PlayBetterHandsOnly", "0003", IsDebugDeploy);
		}
		[Test]
		public void Deploy_StarterBot_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.StarterBot"));
			Deployer.Run(collectDir, "StarterBot", "0001", IsDebugDeploy);
		}
		[Test]
		public void Deploy_RandomBot_Successful()
		{
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.AppDir.FullName, "AIGames.HeadsUpOmaha.Rnd"));
			Deployer.Run(collectDir, "RandomBot", "0002", IsDebugDeploy);
		}
	}
}
