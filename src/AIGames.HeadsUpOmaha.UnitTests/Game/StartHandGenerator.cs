using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AIGames.HeadsUpOmaha.UnitTests.Game
{
	/// <summary>Summary description for StartHandGenerator</summary>
	[TestFixture]
	public class StartHandGenerator
	{
		[Test, Ignore]
		public void Generate_()
		{
			//var h1 = "AKQJT98765432";
			var h = "FEDCB98765432";

			var tp = new string[]
			{
				"1234",
				"1233",
				"1223",
				"1123",
				"1222",
				"1122",
				"1112",
				"1111",
			};

			var cs = new string[]
			{
				"aaaa",
				
				"aaab",
				"aaba",
				"abaa",
				"baaa",
				
				"aabb",
				"abab",
				"abba",

				"aabc",
				"abac",
				"abca",
				"baca",
				"bcaa",

				"abcd",
			};

			var list = new HashSet<string>();

			for (var i0 = 0; i0 < 13; i0++)
			{
				for (var i1 = i0; i1 < 13; i1++)
				{
					for (var i2 = i1; i2 < 13; i2++)
					{
						for (var i3 = i2; i3 < 13; i3++)
						{
							foreach (var c in cs)
							{
								var chs = new char[] { h[i0], c[0], h[i1], c[1], h[i2], c[2], h[i3], c[3] };

								var str = new string(chs);

								var str0 = str.Substring(0, 2);
								var str1 = str.Substring(2, 2);
								var str2 = str.Substring(4, 2);
								var str3 = str.Substring(6, 2);

								if (
									str0 != str1 &&
									str0 != str2 &&
									str0 != str3 &&
									str1 != str2 &&
									str1 != str3 &&
									str2 != str3)
								{
									var sorted = String.Concat(new string[] { str0, str1, str2, str3 }.OrderBy(s => s));
									var sn = sorted.Replace('B', 'T').Replace('C', 'J').Replace('D', 'Q').Replace('E', 'K').Replace('F', 'A');
									list.Add(sn);
								}
							}
						}
					}
				}
			}

			using (var writer = new StreamWriter(@"c:\temp\options.txt", false))
			{
				foreach (var line in list)
				{
					writer.WriteLine(line);
				}
			}
			
			Assert.AreEqual(16003, list.Count);
		}
	}
}
