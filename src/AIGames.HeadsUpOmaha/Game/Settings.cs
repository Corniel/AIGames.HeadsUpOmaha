using AIGames.HeadsUpOmaha.Platform;
using System;
using System.IO;
using System.Xml.Serialization;

namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>The settings of the game.</summary>
	[Serializable]
	public class Settings
	{
		/// <summary>Constructor.</summary>
		public Settings()
		{
			this.TimeBank = 5000;
			this.TimePerMove = 500;
			this.HandsPerLevel = 10;
			this.StartingStack = 2000;
			this.SmallBlind = 10;
			this.YourBot = PlayerType.player1;
		}

		/// <summary>Maximum time in milliseconds that your bot has in its time bank.</summary>
		public int TimeBank { get; set; }
		
		/// <summary>Time in milliseconds that is added to your bot's time bank each move.</summary>
		public int TimePerMove { get; set; }

		/// <summary>Number of hands that is played within each blind level.</summary>
		public int HandsPerLevel { get; set; }

		/// <summary>Amount of chips that each bot starts the tournament with.</summary>
		public int StartingStack { get; set; }

		/// <summary>The initial size of the small blind.</summary>
		public int SmallBlind { get; set; }

		/// <summary>The name of your bot during this match.</summary>
		public PlayerType YourBot { get; set; }
		
		/// <summary>Update the settings.</summary>
		/// <param name="instruction">
		/// The instruction of the input line.
		/// </param>
		/// <remarks>
		/// Typically Settings [property] [number/player1/player2]
		/// </remarks>
		public void Update(Instruction instruction)
		{
			switch (instruction.Action)
			{
				case "timeBank": this.TimeBank = instruction.Int32Value; break;
				case "timePerMove": this.TimePerMove = instruction.Int32Value; break;
				case "handsPerLevel": this.HandsPerLevel = instruction.Int32Value; break;
				case "startingStack": this.StartingStack = instruction.Int32Value; break;
				case "yourBot": this.YourBot = instruction.PlayerTypeValue; break;
			}
		}

		/// <summary>Represents the settings as a set of instructions.</summary>
		public Instruction[] ToInstructions()
		{
			return new Instruction[]
			{
				Instruction.Create(InstructionType.Settings, "timeBank", this.TimeBank),
				Instruction.Create(InstructionType.Settings,"timePerMove", this.TimePerMove),
				Instruction.Create(InstructionType.Settings,"handsPerLevel", this.HandsPerLevel),
				Instruction.Create(InstructionType.Settings,"startingStack", this.StartingStack),
				Instruction.Create(InstructionType.Settings,"yourBot", this.YourBot),
			};
		}
		
		/// <summary>Makes a full copy of the settings for a specified player.</summary>
		public Settings Personalize(PlayerType player)
		{
			this.YourBot = player;
			return this;
		}

		#region I/O operations

		/// <summary>Saves the settings to settings.xml in a directory.</summary>
		public void Save(DirectoryInfo dir)
		{
			Save(new FileInfo(Path.Combine(dir.FullName, "settings.xml")));
		}

		/// <summary>Saves the settings to a file.</summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		/// <param name="mode">
		/// The file mode.
		/// </param>
		public void Save(string fileName, FileMode mode = FileMode.Create) { Save(new FileInfo(fileName), mode); }

		/// <summary>Saves the settings to a file.</summary>
		/// <param name="file">
		/// The file to save to.
		/// </param>
		/// <param name="mode">
		/// The file mode.
		/// </param>
		public void Save(FileInfo file, FileMode mode = FileMode.Create)
		{
			using (var stream = new FileStream(file.FullName, mode, FileAccess.Write))
			{
				Save(stream);
			}
		}

		/// <summary>Saves the settings to a stream.</summary>
		/// <param name="stream">
		/// The stream to save to.
		/// </param>
		public void Save(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Settings));
			serializer.Serialize(stream, this);
		}

		/// <summary>Loads the settings from settings.xml in a directory.</summary>
		public static Settings Load(DirectoryInfo dir)
		{
			return Load(new FileInfo(Path.Combine(dir.FullName, "settings.xml")));
		}

		/// <summary>Loads the settings from a file.</summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		public static Settings Load(string fileName) { return Load(new FileInfo(fileName)); }

		/// <summary>Loads the settings from a file.</summary>
		/// <param name="file">
		/// The file to load from.
		/// </param>
		public static Settings Load(FileInfo file)
		{
			using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
			{
				return Load(stream);
			}
		}

		/// <summary>Loads the settings from a stream.</summary>
		/// <param name="stream">
		/// The stream to load from.
		/// </param>
		public static Settings Load(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Settings));
			return (Settings)serializer.Deserialize(stream);
		}

		#endregion
	}
}
