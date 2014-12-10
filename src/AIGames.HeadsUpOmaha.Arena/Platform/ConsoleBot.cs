using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AIGames.HeadsUpOmaha.Arena.Platform
{
	[DebuggerDisplay("{DebugToString()}")]
	public class ConsoleBot : IDisposable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ConsoleBot));
		private Stopwatch Timer = new Stopwatch();

		/// <summary>Gets the player.</summary>
		public PlayerType Player { get; protected set; }

		/// <summary>Start the timer for the bot.</summary>
		public void Start() { Timer.Start(); }
		/// <summary>Stop the timer for the bot.</summary>
		public void Stop() { Timer.Stop(); }
		/// <summary>Get the elapsed milliseconds.</summary>
		public long ElapsedMilliseconds { get { return Timer.ElapsedMilliseconds; } }

		/// <summary>Did the bot time out?</summary>
		public bool TimedOut { get; protected set; }

		/// <summary>Gets the bot.</summary>
		public Bot Bot { get; protected set; }

		/// <summary>Gets the writer.</summary>
		protected StreamWriter Writer { get; set; }

		/// <summary>Applies the settings to the console bot.</summary>
		public void ApplySettings(Settings settings)
		{
			if (settings == null) { throw new ArgumentNullException("settings"); }

			this.Player = settings.YourBot;
			WriteInstructions(settings.ToInstructions());
		}

		/// <summary>updates the bot for the new round.</summary>
		public void UpdateNewRound(GameState state)
		{
			if (state == null) { throw new ArgumentNullException("state"); }

			var instructions = new Instruction[]
			{
				Instruction.Create(InstructionType.Match, "round", state.Round),
				Instruction.Create(InstructionType.Match, "smallBlind", state.SmallBlind),
				Instruction.Create(InstructionType.Match, "bigBlind", state.BigBlind),
				Instruction.Create(InstructionType.Match, "onButton", state.OnButton),
				
				// Stack
				Instruction.Create(PlayerType.player1, "stack", state.Player1.Stack),
				Instruction.Create(PlayerType.player2, "stack", state.Player2.Stack),
				
				// Blind post
				Instruction.Create(state.OnButton, "post", state.SmallBlind),
				Instruction.Create(state.OnButton.Other(), "post", state.BigBlind),
				
				// Hand.
				Instruction.Create(state.YourBot, "hand", state.Own.Hand),
			};

			WriteInstructions(instructions);
		}

		/// <summary>Updates the table.</summary>
		public void UpdateTable(Cards cards)
		{
			if (cards != null && cards.Count > 0)
			{
				var instruction = Instruction.Create(InstructionType.Match, "table", cards);
				WriteInstructions(instruction);
			}
		}

		/// <summary>Updates the table.</summary>
		public void UpdateBettting(GameState state)
		{
			var instructions = new Instruction[]
			{
				Instruction.Create(InstructionType.Match, "maxWinPot", state.MaxWinPot),
				Instruction.Create(InstructionType.Match, "amountToCall", state.AmountToCall),
			};
			WriteInstructions(instructions);
		}

		/// <summary>The action of the the bot.</summary>
		public GameAction Action(GameState state)
		{
			var time = (int)state[this.Player].TimeBank.TotalMilliseconds;
			if (time < 0) { time = 0; }
			var instruction = Instruction.Create(InstructionType.Action, this.Player.ToString(), time);
			
			try
			{
				WriteInstructions(instruction);
				var action = ReadInstruction(state[this.Player].TimeBank);
				return action;
			}
			catch (Exception x)
			{
				Console.WriteLine(x);
				return GameAction.Fold;
			}
		}

		/// <summary>The reaction of the opponent.</summary>
		public void Reaction(GameState state, GameAction reaction)
		{
			var instruction = Instruction.Create(this.Player.Other(), reaction.ActionType.ToString(), reaction.Amount);
			WriteInstructions(instruction);
		}

		/// <summary>Communicate the result with the bot.</summary>
		public void Result(GameState state, int pot, LastGameAction lastaction)
		{
			if (lastaction.Action != GameAction.Fold)
			{
				WriteInstructions(Instruction.Create(PlayerType.player1, "hand", state.Player1.Hand));
				WriteInstructions(Instruction.Create(PlayerType.player2, "hand", state.Player2.Hand));
			}

			var win1 = Instruction.Create(PlayerType.player1, "wins", pot);
			var win2 = Instruction.Create(PlayerType.player2, "wins", pot);

			switch (state.Result)
			{
				case RoundResult.Player1Wins:
					WriteInstructions(win1);
					break;
				case RoundResult.Player2Wins:
					WriteInstructions(win2);
					break;
				case RoundResult.Draw:
					WriteInstructions(win1);
					WriteInstructions(win2);
					break;
				default:
					throw new NotSupportedException("The game state is not final.");
			}
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public string DebugToString()
		{
			return this.Player + ": " + this.Bot.DebugToString();
		}

		/// <summary>Writes a line to the log file if the writer is set.</summary>
		public void WriteLine(string format, params object[] args)
		{
			if (this.Writer != null)
			{
				this.Writer.WriteLine(format, args);
			}
		}
		/// <summary>Writes a line to the logfile if the writer is set.</summary>
		public void WriteLine(object obj)
		{
			if (this.Writer != null)
			{
				this.Writer.WriteLine(obj);
			}
		}

		#region Process

		/// <summary>The process required to run the bot.</summary>
		protected Process process;

		/// <summary>Writes the instructions to the console.</summary>
		protected void WriteInstructions(params Instruction[] instructions)
		{
			foreach (var instruction in instructions)
			{
				WriteLine(instruction);
				process.StandardInput.WriteLine(instruction);
			}
		}

		protected GameAction ReadInstruction(TimeSpan timeout)
		{
			Start();
			var tokenSource = new CancellationTokenSource();
			CancellationToken token = tokenSource.Token;
			var task = Task.Factory.StartNew(() => process.StandardOutput.ReadLine(), token);
			if (!task.Wait((int)timeout.TotalMilliseconds, token))
			{
				process.Kill();
				this.TimedOut = true;
				return GameAction.Fold;
			}
			Stop();
			var line = task.Result;

			// the bat file for java engines seems to output this line first.
			if ((line ?? string.Empty).Contains(">java "))
			{
				line = process.StandardOutput.ReadLine();
			}
			GameAction action;
			if (GameAction.TryParse(line, out action))
			{
				WriteLine("{0} {1}", this.Player, action);
				return action;
			}
			log.ErrorFormat("Could not parse action '{0}' for '{1}'.", line, this.Bot.FullName);
			WriteLine("{0} {1}", this.Player, GameAction.Fold);
			return GameAction.Fold;
		}
	
		#endregion

		/// <summary>Creates an console bot.</summary>
		/// <param name="bot"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static ConsoleBot Create(Bot bot, DirectoryInfo location, StreamWriter writer)
		{
			if (bot == null) { throw new ArgumentNullException("bot"); }
			if (location == null) { throw new ArgumentNullException("location"); }

			var exe = location.GetFiles("run.bat").FirstOrDefault();

			if (exe == null)
			{
				exe = location.GetFiles("*.exe").FirstOrDefault();
			}
			if (exe == null) { throw new FileNotFoundException("Could not find an executable.", "*.exe|run.bat"); }

			var p = new Process();
			p.StartInfo.WorkingDirectory = location.FullName;
			p.StartInfo.FileName = exe.FullName;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.Start();

			var cb = new ConsoleBot()
			{
				process = p,
				Bot = bot,
				Writer = writer,
			};

			return cb;
		}

		#region IDisposable

		/// <summary>Dispose the console platform.</summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Dispose the console platform.</summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!m_IsDisposed)
			{
				if (disposing)
				{
					if (process != null)
					{
						process.Dispose();
					}
					if (this.Writer != null)
					{
						this.Writer.Flush();
						this.Writer.Dispose();
					}
				}
				m_IsDisposed = true;
			}
		}

		/// <summary>Destructor</summary>
		~ConsoleBot() { Dispose(false); }

		private bool m_IsDisposed = false;

		#endregion
	}
}
