using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Dbot
{
    class Program
    {
        private CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _services;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
        
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _client.Log += Log;
            _client.JoinedGuild += JoinedGuild;

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Credentials.DiscordApiKey);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        
        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommandAsync;
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null)
                return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            string nickname = (message.Author as SocketGuildUser).Guild.GetUser(_client.CurrentUser.Id).Nickname;
            char prefix = '!';
            if (!String.IsNullOrEmpty(nickname))
                prefix = nickname[0];

            if (!(message.HasCharPrefix(prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;
           
            // Create a Command Context
            var context = new SocketCommandContext(_client, message);
            
            if (message.Author.Id == _client.CurrentUser.Id)
            {
                //await context.Channel.SendMessageAsync("Hah, nice try.");
                return;
            }
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task JoinedGuild(SocketGuild guild)
        {
            var user = guild.GetUser(_client.CurrentUser.Id);
            user.ModifyAsync(x => {
                x.Nickname = '!' + user.Username;
            });

            return Task.CompletedTask;
        }
    }
}
