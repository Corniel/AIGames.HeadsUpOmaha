using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AIGames.HeadsUpOmaha.Analysis
{
	/// <summary>Represents the outcome of a poker hand (analysis).</summary>
	/// <remarks>
	/// Has a score for wins, draws and losses.
	/// </remarks>
	[DebuggerDisplay("{DebuggerDisplay}")]
	public struct PokerHandOutcome
	{
		private float m_Win;
		private float m_Los;

		/// <summary>Initializes a new poker hand outcome.</summary>
		public PokerHandOutcome(int w, int d, int l)
		{
			if (w < 0) { throw new ArgumentOutOfRangeException("Number of wins should be positive."); }
			if (d < 0) { throw new ArgumentOutOfRangeException("Number of draws should be positive."); }
			if (l < 0) { throw new ArgumentOutOfRangeException("Number of losses should be positive."); }

			float total = w + d + l;

			m_Win = w / total;
			m_Los = l / total;
		}

		/// <summary>Gets the wining change.</summary>
		public double Win { get { return m_Win; } }

		/// <summary>Gets the Drawing change.</summary>
		public double Draw { get { return 1.0 - m_Win - m_Los; } }

		/// <summary>Gets the losing change.</summary>
		public double Loss { get { return m_Los; } }

		/// <summary>Gets P.</summary>
		public double P { get { return Win + 0.5 * Draw; } }

		/// <summary>Gets 1-P.</summary>
		public double OneMinP { get { return Loss + 0.5 * Draw; } }

		/// <summary>Casts the outcome P implicitly to a double.</summary>
		public static implicit operator double(PokerHandOutcome outcome) { return outcome.P; }

		/// <summary>Represents the poker hand outcome as string.</summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Win: {0}, Draw: {1}, Loss: {2}", Win, Draw, Loss);
		}

		/// <summary>Returns if the object equals the poker hand outcome, otherwise false.</summary>
		public override bool Equals(object obj) { return base.Equals(obj); }

		/// <summary>Gets an hash code for the poker hand outcome.</summary>
		public override int GetHashCode()
		{
			return m_Win.GetHashCode() ^ m_Los.GetHashCode();
		}

		/// <summary>Represents the poker hand outcome as debugger display.</summary>
		[ExcludeFromCodeCoverage, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string DebuggerDisplay
		{
			get
			{
				return String.Format(CultureInfo.InvariantCulture, "W: {0:0.0%}, D: {1:0.0%}, L: {2:0.0%} ({3:0.0%})", Win, Draw, Loss, P);
			}
		}
	}
}
