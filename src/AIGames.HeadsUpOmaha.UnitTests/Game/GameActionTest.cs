using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	[TestFixture]
	public class GameActionTest
	{
		public static readonly GameAction TestInstance = GameAction.Raise(17);

		[Test]
		public void Equals_OtherInstance_IsTrue()
		{
			object other = GameAction.Raise(17);
			Assert.IsTrue(TestInstance.Equals(other));
		}

		[Test]
		public void Equals_OtherInstance_IsFalse()
		{
			object other = GameAction.Raise(16);
			Assert.IsFalse(TestInstance.Equals(other));
		}

		[Test]
		public void GetHashCode_None_AreEqual()
		{
			var act = TestInstance.GetHashCode();
			var exp = 70;
			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Parse_Raise17_AreEqual()
		{
			var act = GameAction.Parse("raise 17");
			var exp = TestInstance;
			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Parse_Check_AreEqual()
		{
			var act = GameAction.Parse("Check");
			var exp = GameAction.Check;
			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Parse_Check0_AreEqual()
		{
			var act = GameAction.Parse("check 0");
			var exp = GameAction.Check;
			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Parse_StringEmpty_AreEqual()
		{
			var act = GameAction.Parse("");
			var exp = GameAction.Check;
			Assert.AreEqual(exp, act);
		}


		[Test]
		public void CheckOrCall_StateWithAmountToCall_Call()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1990,
					Pot = 10,
				},
				Player2 = new PlayerState()
				{
					Stack = 1980,
					Pot = 20,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player1);

			var act = GameAction.CheckOrCall(state);
			var exp = GameAction.Call;
			Assert.AreEqual(exp, act);
		}
		[Test]
		public void CheckOrCall_StateWithNoAmountToCall_Check()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1980,
					Pot = 20,
				},
				Player2 = new PlayerState()
				{
					Stack = 1980,
					Pot = 20,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player1);

			var act = GameAction.CheckOrCall(state);
			var exp = GameAction.Check;
			Assert.AreEqual(exp, act);
		}
	}
}
