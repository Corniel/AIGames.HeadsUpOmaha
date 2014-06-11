using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.Arena
{
    [Serializable]
	public class Bots : List<Bot>
	{
		public bool HasActive { get { return GetActive().Any(); } }

		public IEnumerable<Bot> GetActive()
		{
			return this.Where(bot => !bot.Info.Inactive);
		}
		public IEnumerable<Bot> GetStable(double k)
		{
			return this.GetActive().Where(bot => bot.K <= k);
		}
		public IEnumerable<Bot> GetUnstable(double k)
		{
			return this.GetActive().Where(bot => bot.K > k);
		}

		public Bot[] CreatePair(MT19937Generator rnd, double k)
		{
			var bots = new Bot[2];

			var f = rnd.Next(2);
			var s = f == 1 ? 0 : 1;

			var stable = GetStable(k).ToList();
			var unstable = GetUnstable(k).ToList();

			// pair a stable and an unstabel
			if (stable.Count > 0 && unstable.Count > 0)
			{
				bots[f] = stable[rnd.Next(stable.Count)];
				bots[s] = unstable[rnd.Next(unstable.Count)];
			}
			else
			{
				var random = GetActive().OrderBy(b => rnd.Next()).ToList();
				bots[f] = random[0];
				bots[s] = random[1];
			}
			return bots;
		}

		public Bot GetOrCreate(BotInfo info)
		{
			var bot = this.FirstOrDefault(item => item.Info.Name == info.Name && item.Info.Version == info.Version);
			if (bot != null) 
			{
				bot.Info = info.SetIsActive(info.Inactive);
				return bot;
			}

			var previous = this
				.Where(item => item.Info.Name == info.Name && item.Info.Version < info.Version)
				.OrderByDescending(item => item.Info.Version).FirstOrDefault();

			bot = new Bot()
			{
				Info = info,
			};
			if(previous != null)
			{
				bot.Rating = previous.Rating;
			}

			this.Add(bot);
			return bot;
		}

		
        #region Load & Save

        public void Save(DirectoryInfo dir)
        {
            Save(new FileInfo(Path.Combine(dir.FullName, "bots.xml")));
        }
        public void Save(FileInfo file)
        {
            using (var stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
            {
                Save(stream);
            }
        }
        public void Save(Stream stream)
        {
			this.Sort();
            var serializer = new XmlSerializer(typeof(Bots));
            serializer.Serialize(stream, this);
        }

        public static Bots Load(DirectoryInfo dir)
        {
            return Load(new FileInfo(Path.Combine(dir.FullName, "bots.xml")));
        }
        public static Bots Load(FileInfo file)
        {
            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                return Load(stream);
            }
        }
        public static Bots Load(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Bots));
            var data = (Bots)serializer.Deserialize(stream);
            return data;
        }

        #endregion
	}
}
