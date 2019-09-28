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
    private static IKeyboardMouseEvents hook = Hook.GlobalEvents();

    public static void SetUp(this IActionable @this)
    {
        @this.Validate((all, i) => !@this.Actions.ContainsKey(i.ActionName),
            "Ignoring unknown input action{0} configured for {1}: {2}.", (r, rs) => new string[]
            {
                r.Skip(1).Any() ? "s" : "", @this.InputGroupName, rs()
            });

        @this.Validate((all, i) => all.Any(i.Duplicate),
            "Ignoring duplicate {0} keybindings for: {1}.", (r, rs) => new string[]
            {
                @this.InputGroupName, rs()
            });

        KeyEventHandler callback = null;
        callback = (sender, ev) =>
        {
            try
            {
                if (@this.Holder == null)
                {
                    hook.KeyDown -= callback;
                }
                else if (CameraVisibility.Focused && @this.Holder.isActiveAndEnabled)
                {
                    Action action;
                    foreach (string actionName in @this.GetInputActions().Where(i => i.Activated(ev)).Select(i => i.ActionName))
                    {
                        if (@this.Actions.TryGetValue(actionName, out action))
                            action();
                        else
                            @this.Console.Warning("Ignoring unknown action name for {0}: \"{1}\".", @this.InputGroupName, actionName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, @this.Holder);
            }
        };
        hook.KeyDown += callback;
    }

    private static List<InputAction> GetInputActions(this IActionable @for)
    {
        List<InputAction> inacs;
        if (@for.UserConfig.InputGroups.TryGetValue(@for.InputGroupName, out inacs))
            return inacs;
        return new List<InputAction>();
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