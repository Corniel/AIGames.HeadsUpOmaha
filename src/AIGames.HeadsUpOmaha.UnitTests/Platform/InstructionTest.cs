using AIGames.HeadsUpOmaha.Platform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AIGames.HeadsUpOmaha.UnitTests.Platform
{
	[TestClass]
	public class InstructionTest
	{
		[TestMethod]
		public void Parse_Input001Txt_AreEqual()
		{
			var instruction = new List<Instruction>();
			using (var reader = new StreamReader(AppConfig.GetTestFile("Input.001.txt").FullName))
			{
				string line;

				while ((line = reader.ReadLine()) != null)
				{
					instruction.Add(Instruction.Parse(line));
				}
			}

			var nonNull = instruction.Where(t => t.InstructionType != InstructionType.None).ToList();

			Assert.AreEqual(622, nonNull.Count);
		}
	}
}
