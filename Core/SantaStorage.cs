using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretSanta.Core
{
    public static class SantaStorage
    {
        internal static List<SantaData> Santas = new List<SantaData>();

        static SantaStorage()
        {
            Santas = new List<SantaData>();
        }

        public static bool IsSantaNotExists(ulong guildId)
        {
            if (Santas.FirstOrDefault(x => x.GuildID == guildId) == null) return true;
            else return false;
        }

        public static bool IsUserAlreadyParticipate(ulong userId, ulong guildId)
        {
            if (Santas.FirstOrDefault(x => x.GuildID == guildId).Entrants.Contains(userId)) return true;
            else return false;
        }

        public static SantaData CreateSanta(ulong guildid, ulong userid, uint maxPrice, string Description)
        {
            if (!IsSantaNotExists(guildid))
            {
                return null;
            }
            else
            {
                SantaData NewSanta = new SantaData()
                {
                    GuildID = guildid,
                    OwnerID = userid,
                    Description = Description,
                    Price = maxPrice,
                    Entrants = new List<ulong>()
                };
                Santas.Add(NewSanta);
                return NewSanta;
            }
        }

        internal static SantaData GetSanta(ulong guildId)
        {
            return Santas.FirstOrDefault(x => x.GuildID == guildId);
        }

        internal static async Task DrawSanta(SantaData Bucket, Discord.Commands.SocketCommandContext Context)
        {

            List<ulong> ShuffledBucket = ShuffleList(Bucket.Entrants).ToList();

            for (int i = 0; i < Bucket.Entrants.Count(); i++)
            {
                if (Bucket.Entrants[i] == ShuffledBucket[i])
                {
                    ShuffledBucket = ShuffleList(Bucket.Entrants).ToList();
                    i = -1;
                }
            }

            for (int i = 0; i < Bucket.Entrants.Count(); i++)
            {
                var DM = await Context.Guild.GetUser(Bucket.Entrants[i]).GetOrCreateDMChannelAsync();
                await DM.SendMessageAsync($"Hey {Context.Guild.GetUser(Bucket.Entrants[i]).Mention} ! " +
                    $"\n>>> For Christmas you have to give a present to {Context.Guild.GetUser(ShuffledBucket[i]).ToString()} !");
            }

        }

        internal static Embed CreateEmbed(SantaData Bucket, Discord.Commands.SocketCommandContext Context)
        {
            EmbedBuilder embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Name = $"{Context.User.ToString()} Secret Santa's"
                },

                ThumbnailUrl = Context.User.GetAvatarUrl(),
                Description = Bucket.Description,
                Color = new Color(255, 255, 254),
                Timestamp = DateTime.Now,
                Fields =
                {
                        new EmbedFieldBuilder
                        {
                            Name = $"Maximal Price",
                            Value = $"{Bucket.Price}€",
                        },
                        new EmbedFieldBuilder
                        {
                            Name = $"Entrants",
                            Value = GetParticipantsList(Bucket, Context),
                        }
                },
            };
            return embed.Build();
        }

        internal static Embed CreateEmbed(SantaData Bucket, SocketGuild Guild)
        {
            EmbedBuilder embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Guild.GetUser(657987596785418260).GetAvatarUrl(),
                    Name = $"{Guild.GetUser(Bucket.OwnerID).ToString()} Secret Santa's"
                },
                ThumbnailUrl = Guild.GetUser(Bucket.OwnerID).GetAvatarUrl(),
                Description = Bucket.Description,
                Color = new Color(255, 255, 254),
                Timestamp = DateTime.Now,
                Fields =
                {
                        new EmbedFieldBuilder
                        {
                            Name = $"Maximal Price",
                            Value = $"{Bucket.Price}€",
                        },
                        new EmbedFieldBuilder
                        {
                            Name = $"Entrants",
                            Value = GetParticipantsList(Bucket, Guild),
                        }
                }
            };
            return embed.Build();
        }

        internal static object GetParticipantsList(SantaData Bucket, SocketGuild Guild)
        {
            object ParticipantsList = "";
            foreach (ulong UserId in Bucket.Entrants)
            {
                ParticipantsList += Guild.GetUser(UserId).Mention + "\n";
            }
            return ParticipantsList;
        }

        internal static object GetParticipantsList(SantaData Bucket, Discord.Commands.SocketCommandContext Context)
        {
            object ParticipantsList = "";
            foreach (ulong UserId in Bucket.Entrants)
            {
                ParticipantsList += Context.Guild.GetUser(UserId).Mention + "\n";
            }
            return ParticipantsList;
        }

        // https://stackoverflow.com/a/56836629
        private static IEnumerable<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> TempList = inputList.ToList();
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex;
            while (TempList.Count > 0)
            {
                randomIndex = r.Next(0, TempList.Count); //Choose a random object in the list
                randomList.Add(TempList[randomIndex]); //add it to the new, random list
                TempList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }
    }
}
