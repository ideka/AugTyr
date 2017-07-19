using System.Collections.Generic;
using System.Linq;

public class GameDatabase
{
    public int BuildID = -1;

    public List<MapGroup> MapGroups = new List<MapGroup>();

    public void AddMap(Map map)
    {
        if (this.MapGroups.All(mg => !mg.TryAddMap(map)))
            this.MapGroups.Add(new MapGroup(map));
    }
}