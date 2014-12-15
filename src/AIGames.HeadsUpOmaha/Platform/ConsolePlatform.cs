using AIGames.HeadsUpOmaha.Bot;
using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIGames.HeadsUpOmaha.Platform
{
	/// <summary>Represents a console platform to run a bot on.</summary>
	public class ConsolePlatform : IDisposable
	{
		/// <summary>Runs the bot.</summary>
		public static void Run(IBot bot)
		{
			var platform = new ConsolePlatform();
			platform.DoRun(bot);
		}

		/// <summary>The reader.</summary>
		protected TextReader Reader { get; set; }
		/// <summary>The reader.</summary>
		protected TextWriter Writer { get; set; }

		/// <summary>Constructs a console platform with Console.In and Console.Out.</summary>
		protected ConsolePlatform() : this(Console.In, Console.Out) { }

		/// <summary>Constructs a console platform.</summary>
		protected ConsolePlatform(TextReader reader, TextWriter writer)
		{
			if (reader == null) { throw new ArgumentNullException("reader"); }
			if (writer == null) { throw new ArgumentNullException("writer"); }

			this.Reader = reader;
			this.Writer = writer;
		}

		/// <summary>Runs it all.</summary>
		public void DoRun(IBot bot)
		{
			if (bot == null) { throw new ArgumentNullException("bot"); }
			DoRun(bot, Instruction.Read(this.Reader));
		}

		/// <summary>Runs it all.</summary>
		public void DoRun(IBot bot, IEnumerable<Instruction> instructions)
		{
			if (bot == null) { throw new ArgumentNullException("bot"); }
			if (instructions == null) { throw new ArgumentNullException("instructions"); }

			var settings = new Settings();
			var state = new GameState(settings);

			foreach (var instruction in instructions)
			{
				var previous = state.Copy();
				HandleOpponentReaction(bot, previous, instruction);
				state.Update(instruction);

				switch (instruction.InstructionType)
				{
					case InstructionType.Settings:
						settings.Update(instruction);
						state.Update(settings);
						break;

					case InstructionType.Player:
						if (state.Result != RoundResult.NoResult) { bot.Result(state); }
						break;
					
					case InstructionType.Action:
						var action = bot.Action(state);
						Writer.WriteLine(action);
						break;

					case InstructionType.Match:
					case InstructionType.Output:
					case InstructionType.None:
					default: break;
				}
			}
		}

		private static void HandleOpponentReaction(IBot bot, GameState state, Instruction instruction)
		{
			if (instruction.InstructionType == InstructionType.Player && state.YourBot != instruction.Player)
			{
				var action = instruction.ToGameAction();
				if (action != GameAction.Invalid)
				{
					bot.Reaction(state, action);
				}
			}
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
					this.Reader.Dispose();
					this.Writer.Dispose();
				}
				m_IsDisposed = true;
			}
		}

		/// <summary>Destructor</summary>
		~ConsolePlatform() { Dispose(false); }

		private bool m_IsDisposed = false;

		#endregion
	}
}
