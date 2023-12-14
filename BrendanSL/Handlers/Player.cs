using Exiled.API.Features;
using Exiled.Events.EventArgs;

namespace BrendanSL.Handlers
{
    class Player
    {
        public void OnLeft(LeftEventArgs ev)
        {
            string message = BrendanSL.Instance.Config.LeftMessage.Replace("{player}", ev.Player.Nickname);
            Exiled.API.Features.Map.Broadcast(6, message);
        }

        public void OnJoined(JoinedEventArgs ev)
        {
            string message = BrendanSL.Instance.Config.JoinedMessage.Replace("{player}", ev.Player.Nickname);
            Exiled.API.Features.Map.Broadcast(6, message);
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            float exactState = ev.Door.GetExactState();
            if (ev.IsAllowed == false && ev.Door.RequiredPermissions.RequiredPermissions > 0 && exactState == 0)
            {
                ev.Player.Broadcast(3, BrendanSL.Instance.Config.BoobyTrapMessage);
                ev.Player.Hurt(10f, DamageTypes.Tesla, "DoorSecuritySystem");
            }
        }
    }
}