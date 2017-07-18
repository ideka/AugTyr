using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

public class Node
{
    [JsonIgnore]
    public Vector3 Position
    {
        get
        {
            return new Vector3(this.X, this.Y, this.Z);
        }

        set
        {
            this.X = value.x;
            this.Y = value.y;
            this.Z = value.z;
        }
    }
    public float X;
    public float Y;
    public float Z;

    public string Comment;

    public NodeType Type;

    // Waypoint attributes.
    public string WaypointCode;

    public void SetWaypointCode(string waypointName)
    {
        if (waypointName == null)
        {
            this.WaypointCode = null;
            return;
        }

        string trimmed = waypointName.TrimStart('[').TrimEnd(']');
        string result = Waypoints.Codes.FirstOrDefault(kv => kv.Value == trimmed).Key;
        if (result == null)
            this.WaypointCode = waypointName;
        else
            this.WaypointCode = result;
    }

    // Heart wall attributes.
    public string HeartWallValue;
}