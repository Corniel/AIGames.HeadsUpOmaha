using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AIGames.HeadsUpOmaha.UnitTests.Platform
{
	[TestFixture]
	public class InstructionTest
	{
		[Test]
		public void Finished_All4Statics_Matches()
		{
			Assert.AreEqual("Engine says: \"player1 finished 1\"", Instruction.Player1Finished1.ToString(), "Player1Finished1.ToString()");
			Assert.AreEqual("Engine says: \"player2 finished 1\"", Instruction.Player2Finished1.ToString(), "Player2Finished1.ToString()");
			Assert.AreEqual("Engine says: \"player1 finished 2\"", Instruction.Player1Finished2.ToString(), "Player1Finished2.ToString()");
			Assert.AreEqual("Engine says: \"player2 finished 2\"", Instruction.Player2Finished2.ToString(), "Player2Finished2.ToString()");

			Assert.AreEqual(RoundResult.Player1Wins, Instruction.Player1Finished1.FinalResult, "Player1Finished1.FinalResult");
			Assert.AreEqual(RoundResult.Player2Wins, Instruction.Player2Finished1.FinalResult, "Player2Finished1.FinalResult");
			Assert.AreEqual(RoundResult.Player2Wins, Instruction.Player1Finished2.FinalResult, "Player1Finished2.FinalResult");
			Assert.AreEqual(RoundResult.Player1Wins, Instruction.Player2Finished2.FinalResult, "Player2Finished2.FinalResult");
		}

		[Test, Ignore]
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
