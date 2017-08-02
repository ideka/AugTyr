using Newtonsoft.Json;
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

    // Teleport attributes.
    public string WaypointCode;

    // Heart wall attributes.
    public string HeartWallValue;
}

public enum NodeType
{
    Reach,
    Teleport,
    Heart,
    HeartWall
}