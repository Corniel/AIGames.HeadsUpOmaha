using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AIGames.HeadsUpOmaha.UnitTests
{
	[TestClass]
	public class BitsTest
	{
		[TestMethod]
		public void Count_Ushort_AreEqual()
		{
			for (ulong i = 0; i < Bits.Flag[14]; i++)
			{
				var exp = Bits.Count(i);
				var act = Bits.Count((uint)i);

				Assert.AreEqual(exp, act, i.ToString());
			}
		}
	}
}
