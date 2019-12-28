using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SecretSanta
{
    public class Program
    {

        public static CommandService _commands;
        public static DiscordSocketClient _client;
        public IServiceProvider _services;

        public static void Main() => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            if (Config.bot.Token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;


            await _client.LoginAsync(TokenType.Bot, Config.bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync(".help");

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = false

            });

            _services = new ServiceCollection()
                .BuildServiceProvider();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);


            await InstalEventHandler();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }

        public async Task InstalEventHandler()
        {
            _client.MessageReceived += HandleCommand;
            _client.ReactionAdded += Events.ReactionAdded;
            _client.ReactionRemoved += Events.ReactionRemoved;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage Message)) return;

            SocketCommandContext context = new SocketCommandContext(_client, Message);
            if (context.User.IsBot == true || context.User.IsWebhook == true) return;

            int argPos = 0;
            if (Message.HasStringPrefix(".", ref argPos))
            {

                IResult result = await _commands.ExecuteAsync(context, argPos, _services, MultiMatchHandling.Best);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
                return;
            }
        }
    }
}