using AIGames.HeadsUpOmaha.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.ACDC
{
	[Serializable, XmlRoot("record")]
	public class ActionData : List<ActionDataRecord>
	{
		/// <summary>Adds a new record.</summary>
		public void Add(GameState state, GameAction reaction)
		{
			ActionDataRecord record = ActionDataRecord.Create(state, reaction);
			record.BetRound = 1 + this.Count(r => r.Round == record.Round && r.SubRound == record.SubRound);
			Add(record);
		}
		/// <summary>Updates the record and calculate the winning chance if possible.</summary>
		public void Update(GameState state, MT19937Generator rnd)
		{
			foreach (var record in this.Where(r => state.Round == r.Round))
			{
				record.Update(state, rnd);
			}
		}

		public double GetFoldRate(GameState state)
		{
			if (this.Count == 0 || this.Last().Round == 1) { return 0.0; }

			double folds = this.Count(r => r.Action == GameAction.Fold);
			double rounds = this.Last().Round - 1;

			return folds / rounds;
		}

		public double GetCheckRate(GameState state)
		{
			if (this.Count == 0 || this.Last().Round == 1) { return 0.0; }

			double folds = this.Count(r => r.Action == GameAction.Check);
			double rounds = this.Last().Round - 1;

			return folds / rounds;
		}

		public double GetCallRate(GameState state)
		{
			if (this.Count == 0 || this.Last().Round == 1) { return 0.0; }

			double folds = this.Count(r => r.Action == GameAction.Call);
			double rounds = this.Last().Round - 1;

			return folds / rounds;
		}

		public int GetRaiseRate(GameState state, MT19937Generator Rnd)
		{
			return state.MaxinumRaise;
		}

		#region I/O operations

		/// <summary>Saves the action data to a file.</summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		/// <param name="mode">
		/// The file mode.
		/// </param>
		public void Save(string fileName, FileMode mode = FileMode.Create) { Save(new FileInfo(fileName), mode); }

		/// <summary>Saves the action data to a file.</summary>
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

		/// <summary>Saves the action data to a stream.</summary>
		/// <param name="stream">
		/// The stream to save to.
		/// </param>
		public void Save(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(ActionData));
			serializer.Serialize(stream, this);
		}

		/// <summary>Loads the action data from a file.</summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		public static ActionData Load(string fileName) { return Load(new FileInfo(fileName)); }

		/// <summary>Loads the action data from a file.</summary>
		/// <param name="file">
		/// The file to load from.
		/// </param>
		public static ActionData Load(FileInfo file)
		{
			using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
			{
				return Load(stream);
			}
		}

		/// <summary>Loads the action data from a stream.</summary>
		/// <param name="stream">
		/// The stream to load from.
		/// </param>
		public static ActionData Load(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(ActionData));
			return (ActionData)serializer.Deserialize(stream);
		}
		#endregion
	}
}
