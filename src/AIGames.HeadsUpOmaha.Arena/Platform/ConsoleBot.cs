﻿using AIGames.HeadsUpOmaha.Game;
using AIGames.HeadsUpOmaha.Platform;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AIGames.HeadsUpOmaha.Arena.Platform
{
	[DebuggerDisplay("{DebugToString()}")]
	public class ConsoleBot : IDisposable
	{
		/// <summary>Gets the player.</summary>
		public PlayerType Player { get; protected set; }

		/// <summary>Gets the bot.</summary>
		public Bot Bot { get; protected set; }

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


		/// <summary>The action of the the bot.</summary>
		public GameAction Action(GameState state)
		{
			var instruction  = Instruction.Create(InstructionType.Action, this.Player.ToString(), (int)state.Own.TimeBank.TotalMilliseconds);
			
			try
			{
				WriteInstructions(instruction);

				return ReadInstruction();
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
		public void Result(GameState state, int pot)
		{
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
			}
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public string DebugToString()
		{
			return this.Player + ": " + this.Bot.DebugToString();
		}

		#region Process

		/// <summary>The process required to run the bot.</summary>
		protected Process process;

		/// <summary>Writes the instructions to the console.</summary>
		protected void WriteInstructions(params Instruction[] instructions)
		{
			foreach (var instruction in instructions)
			{
				process.StandardInput.WriteLine(instruction);
			}
		}

		protected GameAction ReadInstruction()
		{

			var line = process.StandardOutput.ReadLine();
			GameAction action;
			if (GameAction.TryParse(line, out action))
			{
				return action;
			}
			return GameAction.Fold;
		}

		#endregion

		/// <summary>Creates an console bot.</summary>
		/// <param name="bot"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static ConsoleBot Create(Bot bot, DirectoryInfo location)
		{
			if (bot == null) { throw new ArgumentNullException("bot"); }
			if (location == null) { throw new ArgumentNullException("location"); }

			var exe = location.GetFiles("*.exe").FirstOrDefault();
			if (exe == null) { throw new FileNotFoundException("Could not find an executable.", "*.exe"); }

			var p = new Process();
			p.StartInfo.FileName = exe.FullName;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.Start();

			var cb = new ConsoleBot()
			{
				process = p,
				Bot = bot
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
