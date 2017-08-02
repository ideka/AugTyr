using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public class UserConfigHolder : MonoBehaviour
{
    public Console Console;

    public static string Path { get { return UnityEngine.Application.streamingAssetsPath + "/UserConfig.json"; } }
    public static readonly List<List<string>> InputDomains = new List<List<string>>
    {
        new List<string> { Console.SInputGroupName },
        new List<string> { RouteHolder.SInputGroupName },
        new List<string> { EditMode.SInputGroupName, FollowMode.SInputGroupName }
    };

    public UserConfig UserConfig = new UserConfig();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        this.Load();
    }

    private void Load()
    {
        try
        {
            this.UserConfig = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText(Path));
        }
        catch (FileNotFoundException)
        {
        }

        foreach (KeyValuePair<string, List<InputAction>> ig in this.UserConfig.InputGroups)
        {
            this.PopulateInputActions(ig.Value);

            List<InputAction> inacs = ig.Value.ToList();
            ig.Value.Clear();
            HashSet<string> duplicates = new HashSet<string>();
            foreach (InputAction toAdd in inacs)
            {
                if (!ig.Value.Any(i => i.Duplicate(toAdd)))
                    ig.Value.Add(toAdd);
                else
                    duplicates.Add(toAdd.ActionName);
            }

            if (duplicates.Any())
                Console.Warning("Ignoring duplicate {0} keybindings for: {1}.",
                    ig.Key, string.Join(" ", duplicates.Select(d => "\"" + d + "\"").ToArray()));
        }
    }

    private void PopulateInputActions(List<InputAction> inputActions)
    {
        foreach (InputAction inac in inputActions)
        {
            try
            {
                inac.Key = (Keys)Enum.Parse(typeof(Keys), inac.KeyName);
            }
            catch
            {
                this.Console.Warning("Could not parse config key name: \"{0}\", ignoring.", inac.KeyName);
            }
        }
    }
}