using Discord.WebSocket;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;

namespace AzraelBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private SocketGuild guild;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += Log;

            // Você pode colocar seu token de forma manual dessa forma porem não é muito seguro
            // string token = "token";

            // Aqui estou lendo o token de um arquivo externo
            try
            {
                var token = File.ReadAllText(@"D:\Matheus\Programação\Programas em C#\AzraelBot\token.txt");
                await _client.LoginAsync(TokenType.Bot, token);
            }
            catch (Exception E)
            {
                Console.WriteLine($"Error: {E.Source}\nDescription: {E.Message}\n{E.StackTrace}");
            }

            await _client.StartAsync();
            await Task.Delay(-1);

        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandsAsync;
            // await _commands.AddModuleAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task HandleCommandsAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }

        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}