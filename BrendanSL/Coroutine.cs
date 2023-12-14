using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;

namespace BrendanSL
{
    class Coroutine
    {
        private bool running = false;

        public void Start()
        {
            running = true;
            Timing.RunCoroutine(Step());
        }

        public void Stop()
        {
            running = false;
        }

        private IEnumerator<float> Step()
        {
            Log.Info("Coroutine started.");
            Room parentRoom;
            String position;
            while (running)
            {
                foreach (KeyValuePair<UnityEngine.GameObject, Player> player in Player.Dictionary)
                {
                    parentRoom = Map.FindParentRoom(player.Key);
                    if (!parentRoom.LightsOff)
                    {
                        //parentRoom.TurnOffLights(10f);
                    }
                    position = player.Value.Position.ToString();
                    player.Value.ShowHint($"Position: {position} Room: {parentRoom.Name}", 2f);
                }
                yield return Timing.WaitForSeconds(1f);
            }
            Log.Info("Coroutine stopped.");
        }
    }
}
