using AIGames.HeadsUpOmaha.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	[TestClass]
	public class GameActionTest
	{
		public static readonly GameAction TestInstance = GameAction.Raise(17);

		[TestMethod]
		public void Equals_OtherInstance_IsTrue()
		{
			object other = GameAction.Raise(17);
			Assert.IsTrue(TestInstance.Equals(other));
		}

		[TestMethod]
		public void Equals_OtherInstance_IsFalse()
		{
			object other = GameAction.Raise(16);
			Assert.IsFalse(TestInstance.Equals(other));
		}

		[TestMethod]
		public void GetHashCode_None_AreEqual()
		{
			var act = TestInstance.GetHashCode();
			var exp = 70;
			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void Parse_Raise17_AreEqual()
		{
			var act = GameAction.Parse("raise 17");
			var exp = TestInstance;
			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void Parse_Check0_AreEqual()
		{
			var act = GameAction.Parse("check 0");
			var exp = GameAction.Check;
			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void Parse_StringEmpty_AreEqual()
		{
			var act = GameAction.Parse("");
			var exp = GameAction.Check;
			Assert.AreEqual(exp, act);
		}
	}
}
