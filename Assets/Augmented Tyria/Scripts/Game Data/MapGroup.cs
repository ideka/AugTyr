using System.Collections.Generic;
using System.Linq;

public class MapGroup
{
    public Dictionary<int, Map> Maps = new Dictionary<int, Map>();

    private HashSet<int> allSectors = new HashSet<int>();

    public MapGroup()
    {
    }

    public MapGroup(int firstMapId, Map firstMap)
    {
        this.TryAddMap(firstMapId, firstMap);
    }

    public int GetId()
    {
        // The ID of the MapGroup is determined by its map with the
        // lowest ID (presumably the earliest created).
        return this.Maps.Keys.Min();
    }

    public IEnumerable<KeyValuePair<string, string>> GetWaypoints(Map skip = null)
    {
        return this.Maps.Where(m => m.Value != skip).SelectMany(m => m.Value.Waypoints);
    }

    public bool TryAddMap(int id, Map map)
    {
        if (!this.Maps.Any() || map.Sectors.Keys.Intersect(this.allSectors).Any() || this.Maps.Values.Any(m => m.Rect.Overlaps(map.Rect)))
        {
            this.Maps[id] = map;
            this.allSectors.UnionWith(map.Sectors.Keys);
            return true;
        }

        return false;
    }
}