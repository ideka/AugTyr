using System.Collections.Generic;
using System.Linq;

public class GameDatabase
{
    public int BuildId = -1;

    public List<MapGroup> MapGroups = new List<MapGroup>();

    public void AddMap(int id, Map map)
    {
        if (this.MapGroups.All(mg => !mg.TryAddMap(id, map)))
            this.MapGroups.Add(new MapGroup(id, map));
    }

    public string GetChatCode(int mapId, string waypointName)
    {
        return null;
    }
}