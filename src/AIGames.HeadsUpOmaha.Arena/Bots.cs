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

		public bool HasActive { get { return this.Any(bot => !bot.Info.Inactive); } }

		public Bot GetRandom(MT19937Generator Rnd)
		{
			if (!HasActive) { return null; }

			var bot = this[Rnd.Next(0, this.Count)];
			while (bot.Info.Inactive)
			{
				bot = this[Rnd.Next(0, this.Count)];
			}
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
