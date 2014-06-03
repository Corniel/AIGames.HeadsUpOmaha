using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AIGames.HeadsUpOmaha.Arena
{
	[Serializable]
	[DebuggerDisplay("{DebugToString()}")]
    public struct BotInfo : ISerializable, IXmlSerializable
	{
		public static readonly Regex DirectoryPattern = new Regex(@"^(?<disabled>z+\.)?(?<name>.+?)(\.(?<version>[0-9]{1,9}))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string m_Name;
        private int m_Version;
        private bool m_Inactive;

        public BotInfo(string name, int version = 0, bool inactive = false)
        {
            if (String.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }
            if (version < 0) { throw new ArgumentOutOfRangeException("version", "Version should be greater or equal then 0."); }

            m_Name = name;
            m_Version = version;
            m_Inactive = inactive;
        }
        
        #region (XML) (De)serialization

        /// <summary>Initializes a new instance of Elo based on the serialization info.</summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        private BotInfo(SerializationInfo info, StreamingContext context)
        {
            if (info == null) { throw new ArgumentNullException("info"); }
            m_Name = info.GetString("Name");
            m_Version = info.GetInt32("Version");
            m_Inactive = info.GetBoolean("Inactive");
        }

        /// <summary>Adds the underlying propererty of Elo to the serialization info.</summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) { throw new ArgumentNullException("info"); }
            info.AddValue("Name", m_Name);
            info.AddValue("Version", m_Version);
            info.AddValue("Inactive", m_Inactive);
        }

        /// <summary>Gets the xml schema to (de) xml serialize an Elo.</summary>
        /// <remarks>
        /// Returns null as no schema is required.
        /// </remarks>
        XmlSchema IXmlSerializable.GetSchema() { return null; }

        /// <summary>Reads the Elo from an xml writer.</summary>
        /// <remarks>
        /// Uses the string parse function of Elo.
        /// </remarks>
        /// <param name="reader">An xml reader.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var s = reader.ReadElementString();
            BotInfo info;
            TryParse(s, out info);
            m_Name = info.m_Name;
            m_Version = info.m_Version;
            m_Inactive = info.m_Inactive;
        }

        /// <summary>Writes the Elo to an xml writer.</summary>
        /// <remarks>
        /// Uses the string representation of Elo.
        /// </remarks>
        /// <param name="writer">An xml writer.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if(String.IsNullOrEmpty(m_Name))
            {
                writer.WriteString(String.Empty);
            }
            else
            {
                writer.WriteString(String.Format("{0}.{1}", m_Name, m_Version));
            }
        }

        #endregion

        public string Name { get { return m_Name; } }
        public int Version { get { return m_Version; } }
        public bool Inactive { get { return m_Inactive; } }

        public BotInfo SetIsActive(bool inactive)
        {
            return new BotInfo(m_Name, m_Version, inactive);
        }

        public override bool Equals(object obj) { return base.Equals(obj); }

        public override int GetHashCode()
        {
            var hash = m_Version;
            hash ^= (m_Name ?? String.Empty).GetHashCode();
            hash ^= m_Inactive.GetHashCode();
            return hash;
        }

		public string DebugToString()
		{
			return String.Format("Bot: {0}, Version: {1}{2}",
				Name,
				Version,
				Inactive ? ", Inactive" : "");
		}

		public static bool TryCreate(DirectoryInfo dir, out BotInfo info)
		{
			return TryParse(dir.Name, out info);
		}
		
		public static bool TryParse(string dir, out BotInfo info)
        {
            var match = DirectoryPattern.Match(dir ?? String.Empty);

            if (match.Success)
            {
                var inactive = !String.IsNullOrEmpty(match.Groups["disabled"].Value);
                var name = match.Groups["name"].Value;
                var version = String.IsNullOrEmpty(match.Groups["version"].Value) ? 0 : Int32.Parse(match.Groups["version"].Value);

                info = new BotInfo(name, version, inactive);
                return true;
            }
            else
            {
                info = default(BotInfo);
                return false;
            }
        }
    }
}