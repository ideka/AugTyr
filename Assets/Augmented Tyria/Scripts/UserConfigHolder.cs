using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

public class UserConfigHolder : MonoBehaviour
{
    public Console Console;

    public static string Path { get { return UnityEngine.Application.streamingAssetsPath + "/UserConfig.json"; } }

    public UserConfig UserConfig = new UserConfig();

    private void Awake()
    {
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
            this.PopulateInputActions(ig.Value);
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