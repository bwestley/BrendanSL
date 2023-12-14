using System.Collections.Generic;
using System.Text;
using Exiled.API.Features;
using Exiled.API.Enums;
using MapAPI = Exiled.API.Features.Map;
using PlayerAPI = Exiled.API.Features.Player;
using RoomType = BrendanSL.Enums.RoomType;
using System.Text.RegularExpressions;

namespace BrendanSL.Handlers
{
    class Map
    {
        private static Regex typeRegex = new Regex("^\\w+");

        public void OnGenerated()
        {
            Log.Info("Map generated. Sending data to mapping server.");
            string json = "{\"rooms\":[";
            bool first = true;
            foreach (Room room in MapAPI.Rooms)
            {
                if (room.Zone != ZoneType.Unspecified)
                {
                    if (first)
                    {
                        json += "{";
                        first = false;
                    }
                    else
                        json += ",{";

                    json += $@"""posx"":{room.transform.position.x},""posy"":{room.transform.position.y},""posz"":{room.transform.position.z},";
                    json += $@"""rotx"":{room.transform.rotation.eulerAngles.x},";
                    
                    if (getZone(room.Name) == ZoneType.Entrance)
                        json += $@"""roty"":{room.transform.rotation.eulerAngles.y + 90f},";
                    else
                        json += $@"""roty"":{room.transform.rotation.eulerAngles.y},";
                    json += $@"""rotz"":{room.transform.rotation.eulerAngles.z},";
                    json += $@"""name"":""{room.Name}"",""zone"":""{getZone(room.Name)}"",""type"":""{getType(room.Name)}""}}";
                }
            }
            json += "],\"players\":[";
            var players = PlayerAPI.Dictionary;
            first = true;
            foreach (KeyValuePair<UnityEngine.GameObject, PlayerAPI> player in players)
            {
                if (first)
                    first = false;
                else
                    json += ",";
                json += $@"{{""posx"":{player.Key.transform.position.x},""posy"":{player.Key.transform.position.y},""posz"":{player.Key.transform.position.z},";
                json += $@"""rotx"":{player.Key.transform.rotation.eulerAngles.x},""roty"":{player.Key.transform.rotation.eulerAngles.y},""rotz"":{player.Key.transform.rotation.eulerAngles.z},";
                json += $@"""team"":""{player.Value.Team}"",";
                json += $@"""role"":""{player.Value.Role}"",";
                json += $@"""ip"":""{player.Value.IPAddress}"",";
                json += $@"""userid"":""{player.Value.UserId}"",";
                json += $@"""name"":""{player.Value.Nickname}""}}";
            }
            json += "]};";
            byte[] data = new ASCIIEncoding().GetBytes(json);

            BrendanSL.Instance.networkStream.Write(data, 0, data.Length);
        }

        private ZoneType getZone(string name)
        {
            if (name.StartsWith("EZ_")) return ZoneType.Entrance;
            if (name.StartsWith("HCZ_")) return ZoneType.HeavyContainment;
            if (name.StartsWith("LCZ_")) return ZoneType.LightContainment;
            if (name == "Surface") return ZoneType.Surface;
            return ZoneType.Unspecified;
        }

        private RoomType getType(string name)
        {
            switch (typeRegex.Match(name).Value)
            {
                case "LCZ_Armory":
                    return RoomType.LczArmory;
                case "LCZ_Curve":
                    return RoomType.LczCurve;
                case "LCZ_Straight":
                    return RoomType.LczStraight;
                case "LCZ_012":
                    return RoomType.Lcz012;
                case "LCZ_914":
                    return RoomType.Lcz914;
                case "LCZ_Crossing":
                    return RoomType.LczCrossing;
                case "LCZ_TCross":
                    return RoomType.LczTCross;
                case "LCZ_Cafe":
                    return RoomType.LczCafe;
                case "LCZ_Plants":
                    return RoomType.LczPlants;
                case "LCZ_Toilets":
                    return RoomType.LczToilets;
                case "LCZ_Airlock":
                    return RoomType.LczAirlock;
                case "LCZ_173":
                    return RoomType.Lcz173;
                case "LCZ_ClassDSpawn":
                    return RoomType.LczClassDSpawn;
                case "LCZ_ChkpB":
                    return RoomType.LczChkpB;
                case "LCZ_372":
                    return RoomType.LczGlassBox;
                case "LCZ_ChkpA":
                    return RoomType.LczChkpA;
                case "HCZ_079":
                    return RoomType.Hcz079;
                case "HCZ_EZ_Checkpoint":
                    return RoomType.HczEzCheckpoint;
                case "HCZ_Room3ar":
                    return RoomType.HczArmory;
                case "HCZ_Testroom":
                    return RoomType.Hcz939;
                case "HCZ_Hid":
                    return RoomType.HczHid;
                case "HCZ_049":
                    return RoomType.Hcz049;
                case "HCZ_ChkpA":
                    return RoomType.HczChkpA;
                case "HCZ_Crossing":
                    return RoomType.HczCrossing;
                case "HCZ_106":
                    return RoomType.Hcz106;
                case "HCZ_Nuke":
                    return RoomType.HczNuke;
                case "HCZ_Tesla":
                    return RoomType.HczTesla;
                case "HCZ_Servers":
                    return RoomType.HczServers;
                case "HCZ_ChkpB":
                    return RoomType.HczChkpB;
                case "HCZ_Room3":
                    return RoomType.HczTCross;
                case "HCZ_457":
                    return RoomType.Hcz096;
                case "HCZ_Curve":
                    return RoomType.HczCurve;
                case "HCZ_Straight":
                    return RoomType.HczStraight;
                case "EZ_Endoof":
                    return RoomType.EzVent;
                case "EZ_Intercom":
                    return RoomType.EzIntercom;
                case "EZ_GateA":
                    return RoomType.EzGateA;
                case "EZ_PCs_small":
                    return RoomType.EzDownstairsPcs;
                case "EZ_Curve":
                    return RoomType.EzCurve;
                case "EZ_PCs":
                    return RoomType.EzPcs;
                case "EZ_Crossing":
                    return RoomType.EzCrossing;
                case "EZ_CollapsedTunnel":
                    return RoomType.EzCollapsedTunnel;
                case "EZ_Smallrooms2":
                    return RoomType.EzConference;
                case "EZ_Straight":
                    return RoomType.EzStraight;
                case "EZ_Cafeteria":
                    return RoomType.EzCafeteria;
                case "EZ_upstairs":
                    return RoomType.EzUpstairsPcs;
                case "EZ_GateB":
                    return RoomType.EzGateB;
                case "EZ_Shelter":
                    return RoomType.EzShelter;
                case "PocketWorld":
                    return RoomType.Pocket;
                case "Outside":
                    return RoomType.Surface;
                default:
                    return RoomType.Unknown;
            }
        }
    }
}
