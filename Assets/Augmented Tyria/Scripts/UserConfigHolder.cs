using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Forms = System.Windows.Forms;
using UnityEngine;

public class UserConfigHolder : MonoBehaviour
{
    public Console Console;

    public static string Path { get { return Application.streamingAssetsPath + "/UserConfig.json"; } }

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

        // Set the resolution.
        if (this.UserConfig.ScreenWidth > 0)
        {
            if (this.UserConfig.ScreenHeight > 0)
                Screen.SetResolution(this.UserConfig.ScreenWidth, this.UserConfig.ScreenHeight, true);
            else
                Screen.SetResolution(this.UserConfig.ScreenWidth, Screen.height, true);
        }
        else if (this.UserConfig.ScreenHeight > 0)
        {
            Screen.SetResolution(Screen.width, this.UserConfig.ScreenHeight, true);
        }
    }

    private void PopulateInputActions(List<InputAction> inputActions)
    {
        foreach (InputAction inac in inputActions)
        {
            try
            {
                inac.Key = (Forms.Keys)Enum.Parse(typeof(Forms.Keys), inac.KeyName);
            }
            catch
            {
                this.Console.Warning("Could not parse config key name: \"{0}\", ignoring.", inac.KeyName);
            }
        }
    }
}