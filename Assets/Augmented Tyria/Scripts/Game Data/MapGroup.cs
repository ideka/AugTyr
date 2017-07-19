using System.Collections.Generic;
using System.Linq;

public class MapGroup
{
    public List<Map> Maps = new List<Map>();

    private HashSet<int> AllSectors = new HashSet<int>();

    public MapGroup(Map firstMap)
    {
        this.TryAddMap(firstMap);
    }

    public int GetID()
    {
        // The ID of the MapGroup is determined by its map with the
        // lowest ID (presumably the earliest created).
        return this.Maps.Min(m => m.ID);
    }

    public bool TryAddMap(Map map)
    {
        if (!this.Maps.Any() || map.Sectors.Keys.Intersect(this.AllSectors).Any() || this.Maps.Any(m => m.Rect.Overlaps(map.Rect)))
        {
            this.Maps.Add(map);
            this.AllSectors.UnionWith(map.Sectors.Keys);
            return true;
        }

        return false;
    }
}