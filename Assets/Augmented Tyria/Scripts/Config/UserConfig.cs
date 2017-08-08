using System.Collections.Generic;

public class UserConfig
{
    // Display.
    public int ScreenWidth = 0;
    public int ScreenHeight = 0;
    public bool ByColorTransparency = false;

    // Behavior.
    public bool AutoUpdateGameDatabase = true;
    public RouteAutoload RouteAutoload = RouteAutoload.Existing;
    public bool StartInFollowMode;
    public bool OrientationHelperDefault = true;
    public int ConsoleFilter = 0;

    // Visuals.
    public float RouteWidth = 1;
    public float NodeSize = .3f;

    // Input.
    public Dictionary<string, List<InputAction>> InputGroups = new Dictionary<string, List<InputAction>>();
}

public enum RouteAutoload
{
    None,
    All,
    Existing,
    NonInstances
}