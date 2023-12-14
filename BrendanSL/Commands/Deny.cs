using CommandSystem;
using System;

namespace BrendanSL.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Deny : ICommand
    {
        public string Command { get; } = "deny";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "A command that is denied.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "The command was not a success!";
            return false;
        }
    }
}
