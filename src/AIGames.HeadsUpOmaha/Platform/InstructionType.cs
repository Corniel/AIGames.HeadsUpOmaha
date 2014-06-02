namespace AIGames.HeadsUpOmaha.Platform
{
	/// <summary>Represents the type of the instruction.</summary>
	public enum InstructionType
	{
		/// <summary>None.</summary>
		/// <remarks>
		/// Used for a empty instruction.
		/// </remarks>
		None = 0,
		
		/// <summary>Player (1 or 2).</summary>
		Player,
		
		/// <summary>Action.</summary>
		Action,
		
		/// <summary>Match.</summary>
		Match,
		
		/// <summary>Settings.</summary>
		Settings,

		/// <summary>Output.</summary>
		Output,
	}
}
