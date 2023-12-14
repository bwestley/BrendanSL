using Exiled.API.Interfaces;
using System.ComponentModel;

namespace BrendanSL
{
    public sealed class Config : IConfig
    {
        [Description("Determines if the plugin should be enabled or disabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Map socket bind address.")]
        public string Address { get; set; } = "127.0.0.1";

        [Description("Map socket bind port.")]
        public int Port { get; set; } = 8777;

        [Description("Sets the message for when someone joins the server. {player} will be replaced with the players name.")]
        public string JoinedMessage { get; set; } = "{player} has joined the server.";

        [Description("Sets the message for when someone leaves the server. {player} will be replaced with the players name.")]
        public string LeftMessage { get; set; } = "{player} has left the server.";

        [Description("Sets the message to be played when a round has been started.")]
        public string RoundStartedMessage { get; set; } = "Round Started";

        [Description("Sets the message for when someone triggers a booby trap.")]
        public string BoobyTrapMessage { get; set; } = "ACCESS DENIED!";
    }
}
