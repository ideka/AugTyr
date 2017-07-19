using System.Linq;

public static class NodeExtensions
{
    public static void SetWaypointCode(this Node node, int mapId, GameDatabase gdb, string waypointName)
    {
        if (string.IsNullOrEmpty(waypointName))
        {
            node.WaypointCode = null;
            return;
        }

        string[] results = gdb.GetChatCodes(mapId, waypointName);
        if (!results.Any())
            node.WaypointCode = waypointName;
        else
            node.WaypointCode = string.Join(" ", results);
    }
}