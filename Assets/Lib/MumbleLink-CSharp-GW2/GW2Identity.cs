using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MumbleLink_CSharp_GW2
{
    public class GW2Identity
    {

        public String Name;
        public GW2Profession Profession;
        public GW2Race Race;
        public int MapId;
        public int WorldId;
        public int TeamColorId;
        public Boolean IsCommander;
        public float Fov;

        [JsonConstructor]
        public GW2Identity(string name, GW2Profession profession, GW2Race race, int map_id, int world_id, int team_color_id, bool commander, float fov)
        {
            Name = name;
            Profession = profession;
            Race = race;
            MapId = map_id;
            WorldId = world_id;
            TeamColorId = team_color_id;
            IsCommander = commander;
            Fov = fov;
        }

        public override string ToString()
        {
            var r = new StringBuilder();
            r.AppendLine("Name : " + Name);
            r.AppendLine("Profession : " + Profession);
            r.AppendLine("Race : " + Race);
            r.AppendLine("MapId : " + MapId);
            r.AppendLine("WorldId : " + WorldId);
            r.AppendLine("TeamColorId : " + TeamColorId);
            r.AppendLine("IsCommander : " + IsCommander);
            r.AppendLine("Fov : " + Fov);

            return r.ToString();

        }
    }


}
