using CommandSystem;
using RemoteAdmin;
using System;

namespace BrendanSL.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class HelloWorld : ICommand
    {
        public string Command { get; } = "hello";

        public string[] Aliases { get; } = new string[] { "helloworld" };

        public string Description { get; } = "A command that says hello to the world.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is PlayerCommandSender player)
            {
                response = $"Hello {player.Nickname}!";
                return false;
            }
            else
            {
                response = "World!";
                return true;
            }
        }
    }
}
