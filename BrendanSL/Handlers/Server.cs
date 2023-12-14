using Exiled.API.Features;

namespace BrendanSL.Handlers
{
    class Server
    {
        public void OnWaitingForPlayers()
        {
            Log.Info("Waiting for players...");
        }

        public void OnRoundStarted()
        {
            Exiled.API.Features.Map.Broadcast(6, BrendanSL.Instance.Config.RoundStartedMessage);
        }
    }
}