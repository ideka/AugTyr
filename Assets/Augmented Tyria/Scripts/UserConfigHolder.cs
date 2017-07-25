using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

public class UserConfigHolder : MonoBehaviour
{
    public static string Path { get { return UnityEngine.Application.streamingAssetsPath + "/UserConfig.json"; } }

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

        this.PopulateInputActions(ref this.UserConfig.RouteInputs);
        this.PopulateInputActions(ref this.UserConfig.EditModeInputs);
        this.PopulateInputActions(ref this.UserConfig.FollowModeInputs);
    }

    private void PopulateInputActions(ref List<InputAction> inputActions)
    {
        foreach (InputAction inac in inputActions)
        {
            try
            {
                inac.Key = (Keys)Enum.Parse(typeof(Keys), inac.KeyName);
            }
            catch (SystemException)
            {
            }
        }
    }
}