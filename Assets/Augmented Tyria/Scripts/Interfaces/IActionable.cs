using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public interface IActionable
{
    UserConfig UserConfig { get; }
    Console Console { get; }
    string InputGroupName { get; }
    Dictionary<string, Action> Actions { get; }
}

public static class IActionableExtensions
{
    public static List<InputAction> GetInputActions(this IActionable @for)
    {
        List<InputAction> inacs;
        if (@for.UserConfig.InputGroups.TryGetValue(@for.InputGroupName, out inacs))
            return inacs;
        return new List<InputAction>();
    }

    public static void Act(this IActionable @as, Keys key, bool control)
    {
        Action action;
        foreach (string actionName in @as.GetInputActions().Where(i => i.Key == key && i.Control == control).Select(i => i.ActionName))
        {
            if (@as.Actions.TryGetValue(actionName, out action))
                action();
            else
                @as.Console.PrintWarning(false, "Ignoring unknown action name for {0}: \"{1}\".", @as.InputGroupName, actionName);
        }
    }
}