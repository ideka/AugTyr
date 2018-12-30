using System.Collections.Generic;

public class UserConfig
{
    // Display.
    public int ScreenWidth = 0;
    public int ScreenHeight = 0;
    public TransparencyMethod TransparencyMethod = TransparencyMethod.ConstantBlit;

    // Behavior.
    public bool AutoUpdateGameDatabase = true;
    public RouteAutoload RouteAutoload = RouteAutoload.Existing;
    public bool StartInFollowMode;
    public bool OrientationHelperDefault = true;
    public int ConsoleFilter = 0;
    public float ConsumeRange = 20;

    // Visuals.
    public int ConsoleFontSize = 20;
    public float RouteWidth = 1;
    public float NodeSize = .3f;

    // Input.
    public Dictionary<string, List<InputAction>> InputGroups = new Dictionary<string, List<InputAction>>();
}

public enum TransparencyMethod
{
    ConstantBlit,
    DiscardAllBlit,
    DefaultBlit,
    NeglectBlit,
    ByColor
}

public enum RouteAutoload
{
    None,
    All,
    Existing,
    NonInstances
}
