using NUnit.Framework;

namespace AIGames.HeadsUpOmaha.UnitTests
{
	[TestFixture]
	public class BitsTest
	{
		[Test]
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
