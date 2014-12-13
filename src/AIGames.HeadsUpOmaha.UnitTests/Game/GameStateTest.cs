using AIGames.HeadsUpOmaha.Game;
using NUnit.Framework;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	[TestFixture]
	public class GameStateTest
	{
		[Test]
		public void Chips_Stack3000Stack1000Pot100Pot200_4300()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 3000,
					Pot = 100,
				},
				Player2 = new PlayerState()
				{
					Stack = 1000,
					Pot = 200,
				},
			};

			var act = state.Chips;
			var exp = 4300;

			Assert.AreEqual(exp, act);
		}
		
		[Test]
		public void Pot_Stack3000Stack1000Pot100Pot200_300()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 3000,
					Pot = 100,
				},
				Player2 = new PlayerState()
				{
					Stack = 1000,
					Pot = 200,
				},
			};

			var act = state.Pot;
			var exp = 300;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void AmountToCall_Pot100VsPot100Player1_0()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1900,
					Pot = 100,
				},
				Player2 = new PlayerState()
				{
					Stack = 1900,
					Pot = 100,
				},
			};

			var act = state.AmountToCall;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void AmountToCall_Pot10VsPot20Player1_10()
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

			var act = state.AmountToCall;
			var exp = 10;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void AmountToCall_Pot40VsPot10Player2_30()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player2,
				Player1 = new PlayerState()
				{
					Stack = 1960,
					Pot = 40,
				},
				Player2 = new PlayerState()
				{
					Stack = 1990,
					Pot = 10,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player2);

			var act = state.AmountToCall;
			var exp = 30;

			Assert.AreEqual(exp, act);
		}

#if DEBUG
		[Test, ExpectedException(typeof(InvalidStateException), UserMessage="Amount to call should not be negative: -10, Opp: 10, Own: 20")]
		public void AmountToCall_Pot10VsPot20Player2_ThrowsInvalidStateException()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player2,
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
			state.AmountToCall = state.GetAmountToCall(PlayerType.player2);
		}
		[Test, ExpectedException(typeof(InvalidStateException), UserMessage = "Amount to call should not be negative: -30, Opp: 10, Own: 40")]
		public void AmountToCall_Pot40VsPot10Player1_ThrowsInvalidStateException()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1960,
					Pot = 40,
				},
				Player2 = new PlayerState()
				{
					Stack = 1990,
					Pot = 10,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player1);
		}
#else
		[Test]
		public void AmountToCall_Pot10VsPot20Player2_0()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player2,
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
			state.AmountToCall = state.GetAmountToCall(PlayerType.player2);

			var act = state.AmountToCall;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void AmountToCall_Pot40VsPot10Player1_0()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1960,
					Pot = 40,
				},
				Player2 = new PlayerState()
				{
					Stack = 1990,
					Pot = 10,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player1);

			var act = state.AmountToCall;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}
#endif
		[Test]
		public void MinimumRaise_SmallBlind35_70()
		{
			var state = new GameState()
			{
				SmallBlind = 35,
				Player1 = new PlayerState()
				{
					Stack = 1900,
					Pot = 100,
				},
				Player2 = new PlayerState()
				{
					Stack = 1900,
					Pot = 100,
				},
			};

			var act = state.MinimumRaise;
			var exp = 70;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void MinimumRaise_OpponentPotSmallerThenBigBlind_0()
		{
			var state = new GameState()
			{
				SmallBlind = 35,
				Player1 = new PlayerState()
				{
					Stack = 3810,
					Pot = 70,
				},
				Player2 = new PlayerState()
				{
					Stack = 50,
					Pot = 70,
				},
			};

			var act = state.MinimumRaise;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void MinimumRaise_OwnPotIncludingAmountToCallSmallerThenBigBlind_0()
		{
			var state = new GameState()
			{
				SmallBlind = 35,
				Player1 = new PlayerState()
				{
					Stack = 100,
					Pot = 100,
				},
				Player2 = new PlayerState()
				{
					Stack = 3610,
					Pot = 140,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player1);
			state.MaxWinPot = state.GetMaxWinPot();

			var act = state.MinimumRaise;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void MaxinumRaise_AmountToCallIsSmallBlind_0()
		{
			var state = new GameState()
			{
				SmallBlind = 40,
				Player1 = new PlayerState()
				{
					Stack = 1960,
					Pot = 40,
				},
				Player2 = new PlayerState()
				{
					Stack = 1920,
					Pot = 80,
				},
			};

			var act = state.MaxinumRaise;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void MaxinumRaise_OpponentPotSmallerThenBigBlind_0()
		{
			var state = new GameState()
			{
				SmallBlind = 35,
				Player1 = new PlayerState()
				{
					Stack = 3810,
					Pot = 70,
				},
				Player2 = new PlayerState()
				{
					Stack = 50,
					Pot = 70,
				},
			};

			var act = state.MaxinumRaise;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void MaxinumRaise_OwnPotIncludingAmountToCallSmallerThenBigBlind_0()
		{
			var state = new GameState()
			{
				SmallBlind = 35,
				Player1 = new PlayerState()
				{
					Stack = 100,
					Pot = 100,
				},
				Player2 = new PlayerState()
				{
					Stack = 3610,
					Pot = 140,
				},
			};

			var act = state.MaxinumRaise;
			var exp = 0;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void MaxinumRaise_Pot080Pot100_200()
		{
			var state = new GameState()
			{
				SmallBlind = 35,
				Player1 = new PlayerState()
				{
					Stack = 1920,
					Pot = 80,
				},
				Player2 = new PlayerState()
				{
					Stack = 1900,
					Pot = 100,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player1);
			state.MaxWinPot = state.GetMaxWinPot();

			var act = state.MaxinumRaise;
			var exp = 200;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void MaxinumRaise_Pot100Pot100_200()
		{
			var state = new GameState()
			{
				SmallBlind = 35,
				Player1 = new PlayerState()
				{
					Stack = 1900,
					Pot = 100,
				},
				Player2 = new PlayerState()
				{
					Stack = 1900,
					Pot = 100,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player1);
			state.MaxWinPot = state.GetMaxWinPot();

			var act = state.MaxinumRaise;
			var exp = 200;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void MaxinumRaise_Stack3700Stack100_100()
		{
			var state = new GameState()
			{
				SmallBlind = 35,
				Player1 = new PlayerState()
				{
					Stack = 3700,
					Pot = 100,
				},
				Player2 = new PlayerState()
				{
					Stack = 100,
					Pot = 100,
				},
			};
			state.AmountToCall = state.GetAmountToCall(PlayerType.player1);
			state.MaxWinPot = state.GetMaxWinPot();

			var act = state.MaxinumRaise;
			var exp = 100;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void IsPreFlop_NullCards_IsTrue()
		{
			var state = new GameState();
			Assert.IsTrue(state.IsPreFlop);
		}
		[Test]
		public void IsPreFlop_EmptryCards_IsTrue()
		{
			var state = new GameState()
			{
				Table = Cards.Empty,
			};
			Assert.IsTrue(state.IsPreFlop);
		}
		[Test]
		public void IsPreFlop_TwoCards_IsFalse()
		{
			var state = new GameState()
			{
				Table = Cards.Parse("[4s,Th]"),
			};
			Assert.IsFalse(state.IsPreFlop);
		}

		[Test]
		public void Personlize_Player1_YourBotIsPlayer1()
		{
			var state = new GameState();
			var act = state.Personalize(PlayerType.player1).YourBot;
			var exp = PlayerType.player1;

			Assert.AreEqual(act, exp);
		}
		[Test]
		public void Personlize_Player2_YourBotIsPlayer2()
		{
			var state = new GameState();
			var act = state.Personalize(PlayerType.player2).YourBot;
			var exp = PlayerType.player2;

			Assert.AreEqual(act, exp);
		}

		[Test]
		public void StartNewRound_Round01_AreEqual()
		{
			var rnd = new MT19937Generator(17);
			var state = new GameState();

			Assert.IsTrue(state.StartNewRound(rnd), "StartNewRound()");

			Assert.AreEqual(PlayerType.player2, state.OnButton, "OnButton");
			Assert.AreEqual(Cards.Parse("[Ks,9c,Kd,3s,6s]"), state.Table, "Table");
			Assert.AreEqual(10, state.SmallBlind, "SmallBlind");
			Assert.AreEqual(1, state.Round, "SmallBlind");

			Assert.AreEqual(Cards.Parse("[2d,Ah,5h,8h]"), state.Player1.Hand, "Player1.Hand");
			Assert.AreEqual(Cards.Parse("[8d,4c,Jh,8s]"), state.Player2.Hand, "Player2.Hand");

			Assert.AreEqual(1980, state.Player1.Stack, "Player1.Stack");
			Assert.AreEqual(1990, state.Player2.Stack, "Player2.Stack");

			Assert.AreEqual(20, state.Player1.Pot, "Player1.Pot");
			Assert.AreEqual(10, state.Player2.Pot, "Player2.Pot");
		}
		[Test]
		public void StartNewRound_Round11_AreEqual()
		{
			var rnd = new MT19937Generator(17);
			var state = new GameState(){ Round = 10 };

			Assert.IsTrue(state.StartNewRound(rnd), "StartNewRound()");

			Assert.AreEqual(15, state.SmallBlind, "SmallBlind");
			Assert.AreEqual(11, state.Round, "SmallBlind");
		}
		[Test]
		public void StartNewRound_PlayerWithTooSmallStack_IsFalse()
		{
			var rnd = new MT19937Generator(17);
			var state = new GameState() { Player1 = new PlayerState() { Stack = 15 } };

			Assert.IsFalse(state.StartNewRound(rnd), "StartNewRound()");
		}

		[Test]
		public void ApplyRoundResult_Player2Wins_1990Vs2010()
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

			state.ApplyRoundResult(RoundResult.Player2Wins);

			var act1 = state.Player1.Stack;
			var act2 = state.Player2.Stack;

			var exp1 = 1990;
			var exp2 = 2010;

			Assert.AreEqual(exp1, act1, "Player1.Stack");
			Assert.AreEqual(exp2, act2, "Player2.Stack");
		}
		[Test]
		public void ApplyRoundResult_Player1Wins_2020Vs1980()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1890,
					Pot = 110,
				},
				Player2 = new PlayerState()
				{
					Stack = 1980,
					Pot = 20,
				},
			};

			state.ApplyRoundResult(RoundResult.Player1Wins);

			var act1 = state.Player1.Stack;
			var act2 = state.Player2.Stack;

			var exp1 = 2020;
			var exp2 = 1980;

			Assert.AreEqual(exp1, act1, "Player1.Stack");
			Assert.AreEqual(exp2, act2, "Player2.Stack");
		}
		[Test]
		public void ApplyRoundResult_NoResultPlayer1Wins_2200Vs1800()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1800,
					Pot = 200,
					Hand = Cards.Parse("[4d,4c,Th,Qh]"),
				},
				Player2 = new PlayerState()
				{
					Stack = 1800,
					Pot = 200,
					Hand = Cards.Parse("[6c,5h,5s,Tc]"),
				},
				Table = Cards.Parse("[2h,3h,4h,4s,2c]"),
			};

			state.ApplyRoundResult(RoundResult.NoResult);

			var act1 = state.Player1.Stack;
			var act2 = state.Player2.Stack;

			var exp1 = 2200;
			var exp2 = 1800;

			Assert.AreEqual(exp1, act1, "Player1.Stack");
			Assert.AreEqual(exp2, act2, "Player2.Stack");
		}
		[Test]
		public void ApplyRoundResult_NoResultPlayer2Wins_1700Vs2300()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
					Hand = Cards.Parse("[4d,4c,Th,Qh]"),
				},
				Player2 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
					Hand = Cards.Parse("[Ah,5h,5s,Tc]"),
				},
				Table = Cards.Parse("[2h,3h,4h,4s,2c]"),
			};

			state.ApplyRoundResult(RoundResult.NoResult);

			var act1 = state.Player1.Stack;
			var act2 = state.Player2.Stack;

			var exp1 = 1700;
			var exp2 = 2300;

			Assert.AreEqual(exp1, act1, "Player1.Stack");
			Assert.AreEqual(exp2, act2, "Player2.Stack");
		}
		[Test]
		public void ApplyRoundResult_NoResultPlayerDraw_2000Vs2000()
		{
			var state = new GameState()
			{
				YourBot = PlayerType.player1,
				Player1 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
					Hand = Cards.Parse("[4d,5c,Tc,Ah]"),
				},
				Player2 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
					Hand = Cards.Parse("[Ac,5h,5s,Tc]"),
				},
				Table = Cards.Parse("[2h,3h,4h,4s,2c]"),
			};

			state.ApplyRoundResult(RoundResult.NoResult);

			var act1 = state.Player1.Stack;
			var act2 = state.Player2.Stack;

			var exp1 = 2000;
			var exp2 = 2000;

			Assert.AreEqual(exp1, act1, "Player1.Stack");
			Assert.AreEqual(exp2, act2, "Player2.Stack");
		}

		[Test]
		public void Reset_None_TableAndPlayerHandsAreEmpty()
		{
			var state = new GameState()
			{
				Table = Cards.Parse("[Ks,9c,Kd,3s,6s]"),
				Result = RoundResult.Player1Wins,
				Player1 = new PlayerState(){ Hand = Cards.Parse("[2d,Ah,5h,8h]") },
				Player2 = new PlayerState(){ Hand = Cards.Parse("[8d,4c,Jh,8s]") },
			};
			state.Reset();

			Assert.AreEqual(RoundResult.NoResult, state.Result, "Result");
			Assert.AreEqual(Cards.Empty, state.Table, "Table");
			Assert.AreEqual(Cards.Empty, state.Player1.Hand, "Player1.Hand");
			Assert.AreEqual(Cards.Empty, state.Player2.Hand, "Player2.Hand");
		}

		[Test]
		public void SetFinalResult_Player1Wins_4000Vs0()
		{
			var state = new GameState()
			{
				Player1 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
				},
				Player2 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
				},
			};

			Assert.IsTrue(state.SetFinalResult(RoundResult.Player1Wins));

			var act1 = state.Player1.Stack;
			var act2 = state.Player2.Stack;

			var exp1 = 4000;
			var exp2 = 0;

			Assert.AreEqual(exp1, act1, "Player1.Stack");
			Assert.AreEqual(exp2, act2, "Player2.Stack");
		}
		[Test]
		public void SetFinalResult_Player2Wins_0Vs4000()
		{
			var state = new GameState()
			{
				Player1 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
				},
				Player2 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
				},
			};

			Assert.IsTrue(state.SetFinalResult(RoundResult.Player2Wins));

			var act1 = state.Player1.Stack;
			var act2 = state.Player2.Stack;

			var exp1 = 0;
			var exp2 = 4000;

			Assert.AreEqual(exp1, act1, "Player1.Stack");
			Assert.AreEqual(exp2, act2, "Player2.Stack");
		}
		[Test]
		public void SetFinalResult_NoResult_1700Vs1700()
		{
			var state = new GameState()
			{
				Player1 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
				},
				Player2 = new PlayerState()
				{
					Stack = 1700,
					Pot = 300,
				},
			};

			Assert.IsFalse(state.SetFinalResult(RoundResult.NoResult));

			var act1 = state.Player1.Stack;
			var act2 = state.Player2.Stack;

			var exp1 = 1700;
			var exp2 = 1700;

			Assert.AreEqual(exp1, act1, "Player1.Stack");
			Assert.AreEqual(exp2, act2, "Player2.Stack");
		}
	}
}
