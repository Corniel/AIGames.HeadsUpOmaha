using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace AIGames.HeadsUpOmaha.UnitTests.Deploy
{
	[TestClass]
	public class DeployBot
	{
		public static readonly Assembly[] CompileAssemblies = new Assembly[]
		{
			typeof(System.Int32).Assembly,
			typeof(System.Linq.Enumerable).Assembly,
			typeof(System.Xml.XmlNode).Assembly,
			typeof(System.Text.RegularExpressions.Regex).Assembly,
		};

		[TestMethod]
		public void Deploy_AllIn_Successful()
		{
			var deployDir = new DirectoryInfo(Path.Combine(AppConfig.DeployDir.FullName, "AllIn"));
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.CoreDir.FullName, "AIGames.HeadsUpOmaha.AllIn"));
			Deploy(collectDir, deployDir);
			Zip(deployDir);
			Compile(deployDir);
		}
		[TestMethod]
		public void Deploy_Checkers_Successful()
		{
			var deployDir = new DirectoryInfo(Path.Combine(AppConfig.DeployDir.FullName, "Checkers"));
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.CoreDir.FullName, "AIGames.HeadsUpOmaha.Checkers"));
			Deploy(collectDir, deployDir);
			Zip(deployDir);
			Compile(deployDir);
		}
		[TestMethod]
		public void Deploy_StarterBot_Successful()
		{
			var deployDir = new DirectoryInfo(Path.Combine(AppConfig.DeployDir.FullName, "StarterBot"));
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.CoreDir.FullName, "AIGames.HeadsUpOmaha.StarterBot"));
			Deploy(collectDir, deployDir);
			Zip(deployDir);
			Compile(deployDir);
		}
		[TestMethod]
		public void Deploy_RandomBot_Successful()
		{
			var deployDir = new DirectoryInfo(Path.Combine(AppConfig.DeployDir.FullName, "RandomBot"));
			var collectDir = new DirectoryInfo(Path.Combine(AppConfig.CoreDir.FullName, "AIGames.HeadsUpOmaha.Rnd"));
			Deploy(collectDir, deployDir);
			Zip(deployDir);
			Compile(deployDir);
		}

		public static void Zip(DirectoryInfo deployDir)
		{
			var destination = new FileInfo(Path.Combine(AppConfig.CoreDir.FullName, @"..\bots\zips", deployDir.Name + ".zip"));

			if (destination.Exists)
			{
				destination.Delete();
			}
			ZipFile.CreateFromDirectory(deployDir.FullName, destination.FullName);
		}

		public static void Compile(DirectoryInfo deployDir)
		{
			using (var provider = new CSharpCodeProvider())
			{
				var options = new CompilerParameters();
				options.GenerateExecutable = true;

				foreach (var assembly in CompileAssemblies)
				{
					options.ReferencedAssemblies.Add(assembly.Location);
				}

				options.OutputAssembly = Path.Combine(AppConfig.CoreDir.FullName, @"..\bots\bin", deployDir.Name.Substring(deployDir.Name.LastIndexOf('.') + 1) + ".exe");

				var csFiles = deployDir.GetFiles("*.cs").Select(f => f.FullName).ToArray();

				var exe = provider.CompileAssemblyFromFile(options, csFiles);

				if (exe.Errors.HasErrors)
				{
					foreach (var error in exe.Errors.OfType<CompilerError>().Where(e => !e.IsWarning))
					{
						Console.WriteLine(error);
					}
					Assert.Fail("Compiled with errors.");
				}
			}
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
