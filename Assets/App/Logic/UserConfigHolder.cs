﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Keys = System.Windows.Forms.Keys;

public class UserConfigHolder : MonoBehaviour
{
    public Console Console;

    public static string Path { get { return Application.streamingAssetsPath + "/UserConfig.json"; } }

    public UserConfig UserConfig = new UserConfig();

    private void Awake()
    {
        Application.logMessageReceived += this.MessageReceived;

        this.Load();
    }

    private void MessageReceived(string text, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                this.Console.Info(text);
                break;

            case LogType.Warning:
                this.Console.Warning(text);
                break;

            case LogType.Error:
            case LogType.Assert:
            case LogType.Exception:
                this.Console.Error(text);
                break;
        }
    }

    private void Load()
    {
        if (File.Exists(Path))
            this.UserConfig = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText(Path));

        foreach (var inputGroup in this.UserConfig.InputGroups.Values)
            this.PopulateInputActions(inputGroup);

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
                inac.Key = (Keys)Enum.Parse(typeof(Keys), inac.KeyName);
            }
            catch
            {
                this.Console.Warning("Could not parse config key name: \"{0}\", ignoring.", inac.KeyName);
            }
        }
    }
}