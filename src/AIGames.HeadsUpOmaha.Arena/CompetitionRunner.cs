using AIGames.HeadsUpOmaha.Arena.Platform;
using AIGames.HeadsUpOmaha.Game;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.Arena
{
	public class CompetitionRunner
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(CompetitionRunner));

		private static readonly int[] TableSizes = new int[] { 0, 3, 4, 5 };

		public CompetitionRunner(DirectoryInfo dir) :
			this(dir, System.Environment.TickCount) { }

		public CompetitionRunner(DirectoryInfo dir, int seed)
		{
			this.RootDirectory = dir;
			this.Bots = new Bots();
			this.Rnd = new MT19937Generator(seed);
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
		public MT19937Generator Rnd { get; set; }

		public Settings GameSettings { get; set; }

		protected Dictionary<BotInfo, DirectoryInfo> BotLocations { get; set; }

		public Bot Player1 { get; set; }
		public Bot Player2 { get; set; }
		public Bot this[PlayerType player]
		{
			get
			{
				switch (player)
				{
					case PlayerType.player2: return this.Player2;
					case PlayerType.player1:
					default: return this.Player1;
				}
			}
		}
		public int Games { get; set; }

		public bool Run()
		{
			if (!ScanDirectory()) { return false; }
			UpdateScreen();
			this.Games++;

			Bot[] pair = Bots.CreatePair(this.Rnd, 12.0);
			this.Player1 = pair[0];
			this.Player2 = pair[1];

			var result = PlayMatch(this.Player1, this.Player2);
			CalculateNewElos(this.Player1, this.Player2, result);

			Bots.Save(new DirectoryInfo(ConfigurationManager.AppSettings["Bots.Dir"]));

			return true;
		}

		public void UpdateScreen()
		{
			int pos = 1;

			Console.Clear();
			Console.Write(' ');
			
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write('♦');
			
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" Omaha Arena ");
			
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write('♣');
			
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(@" {0:h\:mm\:ss} ", sw.Elapsed);
			
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write('♥');

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(@" {0:#,###0} ", this.Games);
			
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write('♠');
			
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(" {0:0.00}s/game", sw.ElapsedMilliseconds / 1000.0 / (double)this.Games);

			Console.WriteLine("=======================================================");
			Console.WriteLine("Pos   ELo   Score    Games  Bot");
			Console.WriteLine("=======================================================");
			foreach (var bot in this.Bots)
			{
				if (!bot.Info.Inactive)
				{
					if (pos == 1)
					{
						Console.BackgroundColor = ConsoleColor.Yellow;
						Console.ForegroundColor = ConsoleColor.Black;
					}
					else if (pos == 2)
					{
						Console.BackgroundColor = ConsoleColor.Gray;
						Console.ForegroundColor = ConsoleColor.Black;
					}
					else if (pos == 3)
					{
						Console.BackgroundColor = ConsoleColor.DarkYellow;
						Console.ForegroundColor = ConsoleColor.Black;
					}
					else if (bot.K < 13)
					{
						Console.ForegroundColor = ConsoleColor.White;
					}
					Console.WriteLine("{0,3}  {1:0000}  {2,6:0.0%} {3,8}  {4,-27}", pos++, bot.Rating, bot.Score, bot.Games, bot.Info.Name + ' ' + bot.Info.Version);
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.Gray;
				}
			}
			lastScreenUpdate = sw.ElapsedMilliseconds;
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
			using (var bot1 = ConsoleBot.Create(player1, BotLocations[player1.Info], CreateWriter(player1, player2)))
			{
				using (var bot2 = ConsoleBot.Create(player2, BotLocations[player2.Info], CreateWriter(player2, player1)))
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
						var lastaction = RunSubRounds(bots, state, state.OnButton);
						var pot = state.ApplyRoundResult(lastaction.GetResult());
						SendResult(bots, state, pot, lastaction);

						// upates the blind.
						state.UpdateBlind(state.Round++);

						if (state.Player1.Stack - state.BigBlind < 0 || state.Player2.Stack - state.BigBlind < 0)
						{
							var winner = state.Player1.Stack - state.Player2.Stack - state.BigBlind > 0 ? RoundResult.Player1Wins : RoundResult.Player2Wins;

							bot1.Writer.WriteLine(@"Engine says: ""{0}""", winner);
							bot2.Writer.WriteLine(@"Engine says: ""{0}""", winner);

							return winner;
						}
					}
				}
			}
		}

		private static StreamWriter CreateWriter(Bot player, Bot opponent)
		{
			var location = new FileInfo(Path.Combine(ConfigurationManager.AppSettings["Games.Dir"], player.FullName, string.Format("{0:yyyy-MM-dd_hh_mm_ss_f}.{1}.log", DateTime.Now, opponent.FullName)));
			if (!location.Directory.Exists)
			{
				location.Directory.Create();
			}
			return new StreamWriter(location.FullName);
		}

		private void SendResult(Dictionary<PlayerType, ConsoleBot> bots, GameState state, int pot, LastGameAction lastaction)
		{
			if (lastaction.Action != GameAction.Fold)
			{
				var hand1 = PokerHand.CreateFromHeadsUpOmaha(state.Table, state.Player1.Hand);
				var hand2 = PokerHand.CreateFromHeadsUpOmaha(state.Table, state.Player2.Hand);
				var compare = PokerHandComparer.Instance.Compare(hand1, hand2);

				if (compare > 0)
				{
					state.Result = RoundResult.Player1Wins;
				}
				else if (compare < 0)
				{
					state.Result = RoundResult.Player2Wins;
				}
				else
				{
					state.Result = RoundResult.Draw;
				}
			}
			else
			{
				state.Result = lastaction.Player == PlayerType.player1 ? RoundResult.Player2Wins : RoundResult.Player1Wins;
			}

			foreach (var kvp in bots)
			{
				kvp.Value.Result(state.Personalize(kvp.Key), pot, lastaction);
			}
		}

		private LastGameAction RunSubRounds(Dictionary<PlayerType, ConsoleBot> bots, GameState state, PlayerType playerToMove)
		{
			LastGameAction action = new LastGameAction(playerToMove, GameAction.Check);
			foreach (var tableSize in TableSizes)
			{
				// update the table.
				foreach (var kvp in bots)
				{
					kvp.Value.UpdateTable(Cards.Create(state.Table.Take(tableSize)));
				}

				action = RunBetting(bots, state, playerToMove);
				if (action.Action == GameAction.Fold)
				{
					return action;
				}
			}
			return action;
		}

		private LastGameAction RunBetting(Dictionary<PlayerType, ConsoleBot> bots, GameState state, PlayerType playerToMove)
		{
			var step = 1;
			while (true)
			{
				var action = bots[playerToMove].Action(state.Personalize(playerToMove));


				switch (action.ActionType)
				{
					case GameActionType.check: action = RunCheck(state, playerToMove); break;
					case GameActionType.call: action = RunCall(state, playerToMove); break;
					case GameActionType.raise: action = RunRaise(state, playerToMove, action.Amount); break;

					case GameActionType.fold:
					default:
						action = RunFold(state, playerToMove); break;
				}
				bots[playerToMove.Other()].Reaction(state.Personalize(playerToMove.Other()), action);

				// on fold return direct.
				if (action == GameAction.Fold) { return new LastGameAction(playerToMove, action); }

				// If not folding or raising, do at least two steps.
				if (action.ActionType != GameActionType.raise && step++ >= 2) { return new LastGameAction(playerToMove, action); }

				playerToMove = playerToMove.Other();
			}
		}

		private GameAction RunCheck(GameState state, PlayerType playerToMove)
		{
			if (state.AmountToCall != 0)
			{
				log.WarnFormat("{0} checked, while calling was required.", this[playerToMove].FullName);
				return RunCall(state, playerToMove);
			}
			return GameAction.Check;
		}
		private GameAction RunCall(GameState state, PlayerType playerToMove)
		{
			if (state.AmountToCall == 0)
			{
				log.WarnFormat("{0} called, while the amount to call was 0. Check was done.", this[playerToMove].FullName);
				return RunCheck(state, playerToMove);
			}
			state[playerToMove].Call(state.AmountToCall);
			return GameAction.Call;
		}
		private GameAction RunRaise(GameState state, PlayerType playerToMove, int raise)
		{
			// We only may call, if we have the small blind.
			if (state.AmountToCall == state.SmallBlind)
			{
				log.WarnFormat("The action is not re-opened to '{0}', raise action changed to 'call'.", this[playerToMove].FullName);
				return RunCall(state, playerToMove);
			}
			// In fact, we don't raise.
			if (raise < state.AmountToCall + state.BigBlind)
			{
				log.WarnFormat("Raise of '{0}' is below minimum amount, automatically changed to minimum.", this[playerToMove].FullName);
				return RunRaise(state, playerToMove, state.AmountToCall + state.BigBlind);
			}
			if (raise > state.BigBlind + state.MaxWinPot)
			{
				log.WarnFormat("Raise of '{0}' is above maximum amount, automatically changed to maximum.", this[playerToMove].FullName);
				return RunRaise(state, playerToMove, state.BigBlind + state.MaxWinPot);
			}
			var stackMin = Math.Min(state.Player1.Stack, state.Player2.Stack);
			if (raise > stackMin)
			{
				if (state.AmountToCall > 0)
				{
					log.WarnFormat("Raise of '{0}' would potentialy lead to a negative stack, raise action changed to 'call'.", this[playerToMove].FullName);
					return RunCall(state, playerToMove);
				}
				else
				{
					log.WarnFormat("Raise of '{0}' would potentialy lead to a negative stack, raise action changed to 'check'.", this[playerToMove].FullName);
					return RunCheck(state, playerToMove);
				}
			}
			state[playerToMove].Raise(raise);
			return GameAction.Raise(raise);
		}
		private GameAction RunFold(GameState state, PlayerType playerToMove)
		{
			state[playerToMove.Other()].Win(state.Pot);
			state[playerToMove].Fold();
			return GameAction.Fold;
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
				kvp.Value.UpdateNewRound(state.Personalize(kvp.Key));
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
				kvp.Value.ApplySettings(this.GameSettings.Personalize(kvp.Key));
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

		/// <summary>Scans the directory.</summary>
		/// <returns>
		/// Return true if at least two bots are active.
		/// </returns>
		private bool ScanDirectory()
		{
			int active = 0;
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
						active++;
					}
				}
			}
			return active > 1;
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
