using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public interface IActionable
{
    MonoBehaviour Holder { get; }

    UserConfig UserConfig { get; }
    Console Console { get; }

    string InputGroupName { get; }
    Dictionary<string, Action> Actions { get; }
}

public static class IActionableExtensions
{
    private static readonly IKeyboardMouseEvents GlobalHook = Hook.GlobalEvents();

    public static void SetUp(this IActionable actionable)
    {
        actionable.Validate((all, i) => !actionable.Actions.ContainsKey(i.ActionName),
            "Ignoring unknown input action{0} configured for {1}: {2}.", (r, rs) => new string[]
            {
                r.Skip(1).Any() ? "s" : "", actionable.InputGroupName, rs(),
            });

        actionable.Validate((all, i) => all.Any(i.Duplicate),
            "Ignoring duplicate {0} keybindings for: {1}.", (r, rs) => new string[]
            {
                actionable.InputGroupName, rs()
            });

        void callback(object sender, KeyEventArgs ev)
        {
            try
            {
                if (actionable.Holder == null)
                {
                    GlobalHook.KeyDown -= callback;
                }
                else if (CameraVisibility.Focused && actionable.Holder.isActiveAndEnabled)
                {
                    foreach (string actionName in actionable.GetInputActions().Where(i => i.Activated(ev)).Select(i => i.ActionName))
                    {
                        if (actionable.Actions.TryGetValue(actionName, out Action action))
                            action();
                        else
                            actionable.Console.Warning("Ignoring unknown action name for {0}: \"{1}\".", actionable.InputGroupName, actionName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, actionable.Holder);
            }
        }

        GlobalHook.KeyDown += callback;
    }

    private static List<InputAction> GetInputActions(this IActionable actionable)
    {
        if (actionable.UserConfig.InputGroups.TryGetValue(actionable.InputGroupName, out List<InputAction> inacs))
            return inacs;
        return new List<InputAction>();
    }

    private static void Validate(this IActionable actionable, Func<List<InputAction>, InputAction, bool> match,
        string message, Func<HashSet<string>, Func<string>, string[]> args)
    {
        List<InputAction> inacs = actionable.GetInputActions();
        var removed = new HashSet<string>();
        inacs.RemoveAll(i => match(inacs, i) && (removed.Add(i.ActionName) || true));

        if (removed.Any())
            actionable.Console.Warning(message, args(removed, () => string.Join(" ", removed.Select(s => "\"" + s + "\"").ToArray())));
    }
}