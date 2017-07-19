using System.Collections.Generic;
using System.Linq;

public class Map
{
    public int ID;
    public string Name;

    public Dictionary<string, string> Waypoints = new Dictionary<string, string>();

    public ContinentRect Rect;
    public Dictionary<int, string> Sectors = new Dictionary<int, string>();

    public struct ContinentRect
    {
        public int ContinentID;
        public HashSet<int> Floors;

        public int Rect1X;
        public int Rect1Y;
        public int Rect2X;
        public int Rect2Y;

        public bool Overlaps(ContinentRect other)
        {
            return (this.ContinentID == other.ContinentID &&
                this.Floors.Intersect(other.Floors).Any() &&
                this.Rect1X == other.Rect1X &&
                this.Rect1Y == other.Rect1Y &&
                this.Rect2X == other.Rect2X &&
                this.Rect2Y == other.Rect2Y);
        }
    }
}