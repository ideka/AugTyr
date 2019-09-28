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
        this.AddMap(firstMapId, firstMap);
    }

    public IEnumerable<KeyValuePair<string, string>> GetWaypoints(Map skip = null)
    {
        return this.Maps.Where(m => m.Value != skip).SelectMany(m => m.Value.Waypoints);
    }

    public bool TryAddMapBySector(int id, Map map)
    {
        if (!this.Maps.Any() || map.Sectors.Keys.Intersect(this.allSectors).Any())
        {
            this.AddMap(id, map);
            return true;
        }

        return false;
    }

    public bool TryAddMapByRect(int id, Map map)
    {
        if (!this.Maps.Any() || this.Maps.Values.Where(m => !m.IsInstance).Any(m => m.Rect.Overlaps(map.Rect)))
        {
            this.AddMap(id, map);
            return true;
        }

        return false;
    }

    private void AddMap(int id, Map map)
    {
        this.Maps[id] = map;
        this.allSectors.UnionWith(map.Sectors.Keys);
    }
}