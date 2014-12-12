using AIGames.HeadsUpOmaha.ACDC;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.UnitTests.ACDC
{
	[TestFixture]
	public class ActionDataTest
	{
		[Test, Ignore]
		public void Update_()
		{
			var rnd = new MT19937Generator(17);
			var data = ActionData.Load(@"C:\Code\GitHub\AIGames.HeadsUpOmaha\src\AIGames.HeadsUpOmaha.UnitTests\ACDC\data.xml");

			foreach (var rec in data)
			{
				if (rec.Table.Count == 0)
				{
					rec.Table = null;
				}
				if (rec.HandOpp.Count == 0)
				{
					rec.HandOpp = null;
				}

				rec.Calculate(rnd);
			}

			data.Save(@"C:\Code\GitHub\AIGames.HeadsUpOmaha\src\AIGames.HeadsUpOmaha.UnitTests\ACDC\data.update.xml");
		}
	}
}
