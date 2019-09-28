using System.Collections.Generic;
using System.Linq;

public class GameDatabase
{
    public int BuildId = -1;

    public List<MapGroup> MapGroups = new List<MapGroup>();

    public bool TryGetMap(int id, out Map map)
    {
        foreach (Dictionary<int, Map> maps in this.MapGroups.Select(g => g.Maps))
            if (maps.TryGetValue(id, out map))
                return true;
        map = default(Map);
        return false;
    }

    public void AddMap(int id, Map map)
    {
        if (!map.IsInstance)
        {
            if (this.MapGroups.All(mg => !mg.TryAddMapBySector(id, map)))
                this.MapGroups.Add(new MapGroup(id, map));
        }
        else
        {
            if (this.MapGroups.All(mg => !mg.TryAddMapBySector(id, map)) && this.MapGroups.All(mg => !mg.TryAddMapByRect(id, map)))
                this.MapGroups.Add(new MapGroup(id, map));
        }
    }

    public IEnumerable<KeyValuePair<string, string>> GetWaypoints(MapGroup skip = null)
    {
        return this.MapGroups.Where(g => g != skip).SelectMany(g => g.GetWaypoints());
    }

    public string[] GetChatCodes(int mapId, string waypointName)
    {
        waypointName = waypointName.TrimStart('[').TrimEnd(']');

        string[] result;
        foreach (MapGroup group in this.MapGroups)
        {
            Map map;
            if (group.Maps.TryGetValue(mapId, out map))
            {
                if (TryGetChatCodes(map.Waypoints, waypointName, out result))
                    return result;

                if (TryGetChatCodes(group.GetWaypoints(map), waypointName, out result))
                    return result;

                if (TryGetChatCodes(this.GetWaypoints(group), waypointName, out result))
                    return result;

                return new string[] { };
            }
        }

        if (TryGetChatCodes(this.GetWaypoints(), waypointName, out result))
            return result;

        return new string[] { };
    }

    private static bool TryGetChatCodes(IEnumerable<KeyValuePair<string, string>> waypoints, string name, out string[] result)
    {
        IEnumerable<string> i = waypoints.Where(wp => wp.Value == name).Select(wp => wp.Key);
        if (i.Any())
        {
            result = i.ToArray();
            return true;
        }
        result = null;
        return false;
    }
}