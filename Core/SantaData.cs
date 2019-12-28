using System.Collections.Generic;

namespace SecretSanta.Core
{
    public class SantaData
    {

        public ulong OwnerID { get; set; }

        public ulong GuildID { get; set; }

        public ulong MessageID { get; set; }

        public string Description { get; set; }

        public uint Price { get; set; }

        public List<ulong> Entrants { get; set; }
    }
}
