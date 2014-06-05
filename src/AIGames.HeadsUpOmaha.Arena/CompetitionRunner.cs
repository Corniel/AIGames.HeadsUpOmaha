using AIGames.HeadsUpOmaha.Arena.Platform;
using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AIGames.HeadsUpOmaha.Arena
{
	public class CompetitionRunner
	{
		private static readonly int[] TableSizes = new int[] { 0, 3, 4, 5 };

		public CompetitionRunner(DirectoryInfo dir) :
			this(dir, System.Environment.TickCount) { }

		public CompetitionRunner(DirectoryInfo dir, int seed)
		{
			this.RootDirectory = dir;
			this.Bots = new Bots();
			this.Rnd = new Random(seed);
			this.BotLocations = new Dictionary<BotInfo, DirectoryInfo>();

			this.GameSettings = new Settings()
			{
				HandsPerLevel = 10,
				StartingStack = 1000,
				TimeBank = 5000,
				TimePerMove = 500,
			};
			sw.Start();
			ScanDirectory();
		}

		public DirectoryInfo RootDirectory { get; protected set; }
		public Bots Bots { get; set; }
		public Random Rnd { get; set; }

		public Settings GameSettings { get; set; }

		protected Dictionary<BotInfo, DirectoryInfo> BotLocations { get; set; }

		public void Run()
		{
			ScanDirectory();

			if (Bots.Count(bot => !bot.Info.Inactive) < 2) { return; }

			Bot player1 = Bots.GetRandom(Rnd);
			Bot player2 = Bots.GetRandom(Rnd);

			while (player1.Equals(player2))
			{
				player2 = Bots.GetRandom(Rnd);
			}
			var result = PlayMatch(player1, player2);
			CalculateNewElos(player1, player2, result);

			Bots.Save(new DirectoryInfo("."));

			UpdateScreen();
		}

		public void UpdateScreen()
		{
			if (sw.ElapsedMilliseconds - lastScreenUpdate > 2400)
			{
				lastScreenUpdate = sw.ElapsedMilliseconds;

				Console.Clear();
				Console.WriteLine(@"Running: {0:h\:mm\:ss}", sw.Elapsed);
				Console.WriteLine();
				for (int i = 0; i < Bots.Count; i++)
				{
					var b = Bots[i];

					Console.WriteLine("{0,3}  {1:0000}  {2,5:0.0%}  {3} {4}", i + 1, b.Rating, b.Score, b.Info.Name, b.Info.Version);
				}
			}
		}
		private long lastScreenUpdate = 0;
		private Stopwatch sw = new Stopwatch();

		/// <summary>Play the match.</summary>
		/// <param name="player1">
		/// Player 1.
		/// </param>
		/// <param name="player2">
		/// Player 2.
		/// </param>
		private RoundResult PlayMatch(Bot player1, Bot player2)
		{
			using (var bot1 = ConsoleBot.Create(player1, BotLocations[player1.Info]))
			{
				using (var bot2 = ConsoleBot.Create(player2, BotLocations[player2.Info]))
				{
					var bots = new Dictionary<PlayerType, ConsoleBot>()
					{
						{ PlayerType.player1, bot1 },
						{ PlayerType.player2, bot2 },
					};

					var state = CreateState();
					ApplySettings(bots);

					while (true)
					{
						state.StartNewRound(this.Rnd);

						StartNewRound(bots, state);
						HandleBlinds(state);
						RunSubRounds(bots, state, state.OnButton);
						var pot = state.ApplyRoundResult();
						SendResult(bots, state, pot);

						// upates the blind.
						state.UpdateBlind(state.Round++);

						if (state.Player1.Stack - state.BigBlind < 0 || state.Player2.Stack - state.BigBlind < 0)
						{
							return state.Player1.Stack - state.Player2.Stack - state.BigBlind > 0 ? RoundResult.Player1Wins : RoundResult.Player2Wins;
						}
					}
				}
			}
		}

		private static void SendResult(Dictionary<PlayerType, ConsoleBot> bots, GameState state, int pot)
		{
			foreach (var kvp in bots)
			{
				kvp.Value.Result(state.Copy(kvp.Key), pot);
			}
		}

		private static void RunSubRounds(Dictionary<PlayerType, ConsoleBot> bots, GameState state, PlayerType playerToMove)
		{
			foreach (var tableSize in TableSizes)
			{
				// update the table.
				foreach (var kvp in bots)
				{
					kvp.Value.UpdateTable(Cards.Create(state.Table.Take(tableSize)));
				}

				var action = RunBetting(bots, state, playerToMove, tableSize != 0);
				if (action == GameActionType.fold) { return; }
			}
		}

		private static GameActionType RunBetting(Dictionary<PlayerType, ConsoleBot> bots, GameState state, PlayerType playerToMove, bool canRaise)
		{
			while (true)
			{
				var action = bots[playerToMove].Action(state.Copy(playerToMove));
				bots[playerToMove.Other()].Reaction(state.Copy(playerToMove.Other()), action);

				switch (action.ActionType)
				{
					case GameActionType.check:
						if (state.AmountToCall != 0)
						{
							state[playerToMove].Call(state.AmountToCall);
						}
						return GameActionType.check;
					case GameActionType.call:
						state[playerToMove].Call(state.AmountToCall);
						if (canRaise) { return GameActionType.call; }
						break;
					case GameActionType.raise:
						var stackMin = Math.Min(state.Player1.Stack, state.Player2.Stack);

						var raise = action.Amount;
						if (raise < state.AmountToCall) { raise = state.AmountToCall; }
						else if (raise > state.MaxWinPot) { raise = state.MaxWinPot; }
						if (raise > stackMin)
						{
							raise = stackMin;
						}
						// the small blind can not raise.
						if (!canRaise)
						{
							raise = state.AmountToCall;
						}
						state[playerToMove].Raise(raise);
						if (raise == 0)
						{
							return GameActionType.call;
						}
						break;
					case GameActionType.fold:
					default:
						state[playerToMove.Other()].Win(state.Pot);
						state[playerToMove].Fold();
						return GameActionType.fold;
				}

				playerToMove = playerToMove.Other();
				canRaise = true;
			}
		}

		private static void HandleBlinds(GameState state)
		{
			state.Button.Post(state.SmallBlind);
			state.Blind.Post(state.BigBlind);
		}

		private static void StartNewRound(Dictionary<PlayerType, ConsoleBot> bots, GameState state)
		{
			foreach (var kvp in bots)
			{
				kvp.Value.UpdateNewRound(state.Copy(kvp.Key));
			}
		}

		private GameState CreateState()
		{
			var state = new GameState(this.GameSettings)
			{
				Round = 1,
				OnButton = (PlayerType)Rnd.Next(0, 2),
			};
			return state;
		}

		private void ApplySettings(Dictionary<PlayerType, ConsoleBot> bots)
		{
			foreach (var kvp in bots)
			{
				kvp.Value.ApplySettings(this.GameSettings.Copy(kvp.Key));
			}
		}

		/// <summary>Calculate the new elos for the players.</summary>
		private static void CalculateNewElos(Bot player1, Bot player2, RoundResult result)
		{
			var score1 = 0.0;
			var elo1 = player1.Rating;
			var elo2 = player2.Rating;
			var k1 = player1.K;
			var k2 = player2.K;

			switch (result)
			{
				case RoundResult.Player1Wins:
					player1.Wins++;
					player2.Losses++;
					score1 = 1.0;
					break;
				case RoundResult.Player2Wins:
					player1.Losses++;
					player2.Wins++;
					score1 = 0.0;
					break;
				case RoundResult.Draw:
					player1.Draws++;
					player2.Draws++;
					score1 = 0.5;
					break;
				case RoundResult.NoResult:
				default:
					throw new ArgumentException("There should be a final rersult", "result");
			}
			var score2 = 1.0 - score1;

			player1.Rating = player1.Rating.GetNew(elo2, score1, k1);
			player2.Rating = player2.Rating.GetNew(elo1, score2, k2);

			player1.K = NewK(k1, k2);
			player2.K = NewK(k2, k1);
		}

		private void ScanDirectory()
		{
			BotLocations.Clear();
			// Disable all.
			foreach (var bot in Bots)
			{
				bot.Info = bot.Info.SetIsActive(true);
			}

			foreach (var dir in RootDirectory.GetDirectories().Where(d => d.Name != "bin" && d.Name != "zips"))
			{
				BotInfo info;
				if (BotInfo.TryCreate(dir, out info))
				{
					var bot = Bots.GetOrCreate(info);
					if (!info.Inactive)
					{
						BotLocations[bot.Info] = dir;
					}
				}
			}
		}

		public static double NewK(double kOwn, double kOther)
		{
			if (kOwn <= kOther)
			{
				kOwn *= 0.99;
				return Math.Max(12.0, kOwn);
			}
			kOwn = (kOwn - kOther) * 0.90 + kOther;

			return kOwn;
		}
	}
}
