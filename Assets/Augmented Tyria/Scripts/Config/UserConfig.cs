using System.Collections.Generic;

public class UserConfig
{
    // Behavior.
    public bool AutoUpdateGameDatabase = true;
    public bool StartInFollowMode;
    public bool OrientationHelperDefault = true;

    // Visuals.
    public float RouteWidth = 1;
    public float NodeSize = .3f;

    // Input.
    public List<InputAction> RouteInputs;
    public List<InputAction> EditModeInputs;
    public List<InputAction> FollowModeInputs;
}