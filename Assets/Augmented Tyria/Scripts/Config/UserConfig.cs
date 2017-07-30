using System.Collections.Generic;

public class UserConfig
{
    // Behavior.
    public bool AutoUpdateGameDatabase = true;
    public bool StartInFollowMode;
    public bool OrientationHelperDefault = true;
    public int ConsoleFilter = 0;

    // Visuals.
    public float RouteWidth = 1;
    public float NodeSize = .3f;

    // Input.
    public Dictionary<string, List<InputAction>> InputGroups;
}