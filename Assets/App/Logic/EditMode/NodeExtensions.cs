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

    public static void SetHeartWallValue(this Node node, string input)
    {
        node.HeartWallValue = null;
        if (int.TryParse(input, out int value) && value > 0 && value <= 100)
            node.HeartWallValue = input;
    }
}