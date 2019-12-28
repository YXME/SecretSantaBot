using Discord;
using Discord.WebSocket;
using SecretSanta.Core;
using System.Linq;
using System.Threading.Tasks;

namespace SecretSanta
{
    public class Events
    {
        internal static async Task ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (arg3.User.Value.IsBot) return;
            SantaData Bucket = SantaStorage.Santas.FirstOrDefault(x => x.MessageID == arg3.MessageId);
            if (Bucket == null || ((Emote)arg3.Emote).Id != Emote.Parse("<:RemSanta:660402975109283843>").Id || arg3.UserId == Bucket.OwnerID) return;
            else
            {
                Bucket.Entrants.Add(arg3.UserId);
                var ReactionMessage = await arg2.GetMessageAsync(arg3.MessageId) as IUserMessage;
                await ReactionMessage.ModifyAsync(x => x.Embed = SantaStorage.CreateEmbed(Bucket, ((SocketGuildUser)arg3.User.Value).Guild));
            }
        }

        internal static async Task ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (arg3.User.Value.IsBot) return;
            SantaData Bucket = SantaStorage.Santas.FirstOrDefault(x => x.MessageID == arg3.MessageId);
            if (Bucket == null || ((Emote)arg3.Emote).Id != Emote.Parse("<:RemSanta:660402975109283843>").Id || arg3.UserId == Bucket.OwnerID) return;
            else
            {
                Bucket.Entrants.Remove(arg3.UserId);
                var ReactionMessage = await arg2.GetMessageAsync(arg3.MessageId) as IUserMessage;
                await ReactionMessage.ModifyAsync(x => x.Embed = SantaStorage.CreateEmbed(Bucket, ((SocketGuildUser)arg3.User.Value).Guild));
            }
        }
    }
}
