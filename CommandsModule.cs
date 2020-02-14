using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SecretSanta.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretSanta
{
    public class CommandsModule : ModuleBase<SocketCommandContext>
    {
        [Command("help"), Summary("The help command.")]
        [Alias("h")]
        public async Task Help()
        {
            await Context.Message.DeleteAsync();
            List<CommandInfo> _commands = Program._commands.Commands.Take(Program._commands.Commands.Count() / 2).ToList();
            SocketDMChannel DM = await Context.User.GetOrCreateDMChannelAsync() as SocketDMChannel;

            EmbedBuilder CommandsEmbed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder()
                {
                    Name = $"SecretSantaBot — Commands",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl()
                },
                Color = Color.Teal,
            };

            foreach (CommandInfo command in _commands.Where(x => x.Remarks == null).ToList())
            {
                string embedFieldText = command.Summary ?? "No description available\n";
                string parameters = "";
                foreach (ParameterInfo parameter in command.Parameters)
                {
                    if (parameter.IsOptional) parameters += $"({parameter.Name}) ";
                    else parameters += $"[{parameter.Name}] ";
                }
                CommandsEmbed.AddField($".{command.Name} {parameters}", embedFieldText);
            }
            try
            {
                await DM.SendMessageAsync(embed: CommandsEmbed.Build());
            }
            catch(System.Exception ex)
            {
                await ReplyAsync(ex.Message);
            }

        }

        [Command("createsanta"), Summary("Creates everything to run your Secret Santa !")]
        [Alias("create", "cs")]
        public async Task CreateSanta(uint MaxPrice, [Remainder]string Description = "No description")
        {
            await Context.Message.DeleteAsync();
            SantaData Bucket = SantaStorage.CreateSanta(Context.Guild.Id, Context.User.Id, MaxPrice, Description);
            if (Bucket == null)
            {
                await ReplyAsync("There's already a Secret Santa ongoing on this server. Wait until it ends.");
            }
            else
            {
                Bucket.Entrants.Add(Context.User.Id);
                var Message = await ReplyAsync($"Add a reaction to enter !", false, SantaStorage.CreateEmbed(Bucket, Context));
                Bucket.MessageID = Message.Id;
                await Message.AddReactionAsync(Emote.Parse("<:RemSanta:660402975109283843>"));
            }
        }

        [Command("drawsanta"), Summary("Draws the Entrants list, and send in Private Message the name of the chosen one !")]
        [Alias("draw")]
        public async Task DrawSanta()
        {
            await Context.Message.DeleteAsync();
            SantaData Bucket = SantaStorage.GetSanta(Context.Guild.Id);
            if (Bucket == null)
            {
                await ReplyAsync("No Secret Santa has been found.");
            }
            else if (Bucket.OwnerID != Context.User.Id)
            {
                await ReplyAsync("Draw must be issued by the Owner.");
            }
            else if(Bucket.Entrants.Count <= 1)
            {
                await ReplyAsync("There's not enough entrants to draw.");
            }
            else
            {
                var Message = await Context.Channel.GetMessageAsync(Bucket.MessageID);
                await Message.DeleteAsync();
                await SantaStorage.DrawSanta(Bucket, Context);
                SantaStorage.Santas.Remove(Bucket);
            }
        }


    }
}
