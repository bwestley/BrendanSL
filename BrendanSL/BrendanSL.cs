using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Net.Sockets;
using PlayerHandler = Exiled.Events.Handlers.Player;
using ServerHandler = Exiled.Events.Handlers.Server;
using MapHandler = Exiled.Events.Handlers.Map;

namespace BrendanSL
{
    public class BrendanSL : Plugin<Config>
    {
        private static readonly Lazy<BrendanSL> LazyInstance = new Lazy<BrendanSL>(() => new BrendanSL());
        public static BrendanSL Instance => LazyInstance.Value;
        private BrendanSL() { }

        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        private Handlers.Server serverHandler;
        private Handlers.Player playerHandler;
        private Handlers.Map mapHandler;

        public TcpClient tcpClient;
        public NetworkStream networkStream;

        private Coroutine coroutine = new Coroutine();

        public override void OnEnabled()
        {
            Log.Info($"Connecting to mapping server at {Config.Address}:{Config.Port}.");
            tcpClient = new TcpClient();
            tcpClient.Connect(Config.Address, Config.Port);
            networkStream = tcpClient.GetStream();
            Log.Info($"Connected to mapping server at {Config.Address}:{Config.Port}.");
            coroutine.Start();
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            coroutine.Stop();
            UnregisterEvents();
        }

        public void RegisterEvents()
        {
            serverHandler = new Handlers.Server();
            playerHandler = new Handlers.Player();
            mapHandler = new Handlers.Map();

            PlayerHandler.Left += playerHandler.OnLeft;
            PlayerHandler.Joined += playerHandler.OnJoined;
            PlayerHandler.InteractingDoor += playerHandler.OnInteractingDoor;

            ServerHandler.WaitingForPlayers += serverHandler.OnWaitingForPlayers;
            ServerHandler.RoundStarted += serverHandler.OnRoundStarted;

            MapHandler.Generated += mapHandler.OnGenerated;
        }

        private void UnregisterEvents()
        {
            PlayerHandler.Left -= playerHandler.OnLeft;
            PlayerHandler.Joined -= playerHandler.OnJoined;
            PlayerHandler.InteractingDoor -= playerHandler.OnInteractingDoor;

            ServerHandler.WaitingForPlayers -= serverHandler.OnWaitingForPlayers;
            ServerHandler.RoundStarted -= serverHandler.OnRoundStarted;

            MapHandler.Generated -= mapHandler.OnGenerated;

            serverHandler = null;
            playerHandler = null;
            mapHandler = null;
        }
    }
}
