using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public interface IActionable
{
    Dictionary<string, Action> Actions { get; }
}

public static class IActionableExtensions
{
    public static void Act(this IActionable @as, List<InputAction> inputActions, Keys key, bool control)
    {
        Action action;
        foreach (string actionName in inputActions.Where(i => i.Key == key && i.Control == control).Select(i => i.ActionName))
        {
            if (@as.Actions.TryGetValue(actionName, out action))
                action();
            // TODO: Report errors.
        }
    }
}