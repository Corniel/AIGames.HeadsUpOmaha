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
		public static Instruction[] Read001Txt = Instruction.Read(new StreamReader(typeof(InstructionTest).Assembly.GetManifestResourceStream("AIGames.HeadsUpOmaha.UnitTests.Platform.InstructionTest.Read001.txt"))).ToArray();

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

		[Test]
		public void Read_Read001Txt_HasInstructions()
		{
			using (var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("AIGames.HeadsUpOmaha.UnitTests.Platform.InstructionTest.Read001.txt")))
			{
				var act = Instruction.Read(reader).ToList();

				Assert.AreEqual(1476, act.Count);
			}
		}
	}
}
