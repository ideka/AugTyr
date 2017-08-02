using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public interface IActionable
{
    UserConfig UserConfig { get; }
    Console Console { get; }
    string InputGroupName { get; }
    Dictionary<string, Action> Actions { get; }
}

public static class IActionableExtensions
{
    public static IKeyboardMouseEvents SetUp(this IActionable @this, MonoBehaviour holder, bool ignoreInputIfDisabled)
    {
        @this.Validate();

        IKeyboardMouseEvents hook = Hook.GlobalEvents();
        hook.KeyDown += (sender, ev) =>
        {
            try
            {
                if (Camera.main.cullingMask != 0 && (holder.isActiveAndEnabled || !ignoreInputIfDisabled))
                    @this.Act(ev.KeyCode, ev.Control);
            }
            catch (Exception e)
            {
                Debug.LogException(e, holder);
            }
        };
        return hook;
    }

    private static void Act(this IActionable @as, Keys key, bool control)
    {
        Action action;
        foreach (string actionName in @as.GetInputActions().Where(i => i.Key == key && i.Control == control).Select(i => i.ActionName))
        {
            if (@as.Actions.TryGetValue(actionName, out action))
                action();
            else
                @as.Console.Warning("Ignoring unknown action name for {0}: \"{1}\".", @as.InputGroupName, actionName);
        }
    }

    private static List<InputAction> GetInputActions(this IActionable @for)
    {
        List<InputAction> inacs;
        if (@for.UserConfig.InputGroups.TryGetValue(@for.InputGroupName, out inacs))
            return inacs;
        return new List<InputAction>();
    }

    private static void Validate(this IActionable @for)
    {
        @for.Validate((all, i) => !@for.Actions.ContainsKey(i.ActionName),
            "Ignoring unknown input action{0} configured for {1}: {2}.", (r, rs) => new string[]
            {
                r.Skip(1).Any() ? "s" : "", @for.InputGroupName, rs()
            });

        @for.Validate((all, i) => all.Any(i.Duplicate),
            "Ignoring duplicate {0} keybindings for: {1}.", (r, rs) => new string[]
            {
                @for.InputGroupName, rs()
            });
    }

    private static void Validate(this IActionable @for, Func<List<InputAction>, InputAction, bool> match,
        string message, Func<HashSet<string>, Func<string>, string[]> args)
    {
        List<InputAction> inacs = @for.GetInputActions();
        HashSet<string> removed = new HashSet<string>();
        inacs.RemoveAll(i => match(inacs, i) && (removed.Add(i.ActionName) || true));

        if (removed.Any())
            @for.Console.Warning(message, args(removed, () => string.Join(" ", removed.Select(s => "\"" + s + "\"").ToArray())));
    }
}