using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AIGames.HeadsUpOmaha.UnitTests.Deploy
{
	[TestClass]
	public class DeployBot
	{
		[TestMethod]
		public void Run()
		{
			var deployDir = new DirectoryInfo(Path.Combine(AppConfig.DeployDir.FullName, AppConfig.BotDir.Name));
			var collectDir = AppConfig.BotDir;
			Deploy(collectDir, deployDir);
		}

		public static void Deploy(DirectoryInfo collectDir, DirectoryInfo deployDir)
		{
			if (!deployDir.Exists)
			{
				deployDir.Create();
			}
			else
			{
				foreach (var file in deployDir.GetFiles("*.cs"))
				{
					file.Delete();
				}
			}

			foreach (var file in Collect(collectDir))
			{
				file.CopyTo(Path.Combine(deployDir.FullName, file.Name));
			}
			foreach (var file in Collect(AppConfig.HeadsUpOmahaDir))
			{
				file.CopyTo(Path.Combine(deployDir.FullName, file.Name));
			}
		}

		public static IEnumerable<FileInfo> Collect(DirectoryInfo dir)
		{
			foreach (var child in dir.GetDirectories().Where(d=> !ExcludeDirs.Contains(d.Name)))
			{
				foreach (var file in Collect(child))
				{
					yield return file;
				}
			}
			foreach (var file in dir.GetFiles("*.cs"))
			{
				yield return file;
			}
		}
		private static readonly string[] ExcludeDirs = new string[] { "bin", "obj", "Properties" };
	}
}
