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

        foreach (List<InputAction> ig in this.UserConfig.InputGroups.Values)
            this.PopulateInputActions(ig);

        // TODO: Make this clear...
        for (IEnumerable<List<string>> id = InputDomains; id.Any(); id = id.Skip(1))
        {
            foreach (List<string> nid in id.Skip(1))
            {
                foreach (string groupName in id.First())
                {
                    IEnumerable<InputAction> inacs = this.UserConfig.InputGroups.Where(ig => ig.Key == groupName).SelectMany(ig => ig.Value);
                    foreach (string nGroupName in nid)
                    {
                        foreach (InputAction inac in this.UserConfig.InputGroups.Where(ig => ig.Key == nGroupName).SelectMany(ig => ig.Value))
                        {
                            foreach (InputAction repeat in inacs.Where(ia => ia.Key == inac.Key && ia.Control == inac.Control))
                            {
                                Console.Warning("Conflicting keybinding for {0}.{1} and {2}.{3}.", groupName, repeat.ActionName, nGroupName, inac.ActionName);
                            }
                        }
                    }
                }
            }
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
            catch (SystemException)
            {
                this.Console.Warning("Could not parse config key name: \"{0}\", ignoring.", inac.KeyName);
            }
        }
    }
}