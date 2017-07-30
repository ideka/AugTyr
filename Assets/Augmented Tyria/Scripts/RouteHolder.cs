using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RouteHolder : MonoBehaviour, IActionable
{
    public Mumble Mumble;

    [Header("Modes")]
    public EditMode EditMode;
    public FollowMode FollowMode;

    public static string Path { get { return UnityEngine.Application.streamingAssetsPath + "/Routes/"; } }

    public Route Route = new Route();

    public int NodeIndex { get; set; }
    public GameDatabase GameDatabase { get { return this.GameDatabaseHolder.GameDatabase; } }
    public UserConfig UserConfig { get { return this.GameDatabaseHolder.UserConfig; } }
    public Console Console { get { return this.GameDatabaseHolder.Console; } }
    public int MapId { get { return this.Mumble.Link.GetCoordinates().MapId; } }

    public string InputGroupName { get { return "Route"; } }
    public Dictionary<string, Action> Actions
    {
        get
        {
            return new Dictionary<string, Action>()
            {
                {
                    "Save", this.Save
                },
                {
                    "Load", () => this.Load()
                },
                {
                    "LoadClipboardId", () => this.Load(true)
                },
                {
                    "ToggleMode", () =>
                    {
                        if (this.EditMode.gameObject.activeSelf)
                        {
                            this.EditMode.gameObject.SetActive(false);
                            this.FollowMode.gameObject.SetActive(true);
                        }
                        else
                        {
                            this.EditMode.gameObject.SetActive(true);
                            this.FollowMode.gameObject.SetActive(false);
                        }
                    }
                },
            };
        }
    }

    private IKeyboardMouseEvents globalHook;
    private int loadedRouteId;
    private int oldMapId;

    public GameDatabaseHolder GameDatabaseHolder { get; private set; }

    private void Awake()
    {
        this.globalHook = Hook.GlobalEvents();
        this.globalHook.KeyDown += this.GlobalHookKeyDown;

        this.GameDatabaseHolder = FindObjectOfType<GameDatabaseHolder>();
        if (this.GameDatabaseHolder == null)
        {
            SceneManager.LoadScene("Load");
            return;
        }

        this.NodeIndex = -1;
        this.oldMapId = this.MapId;

        this.Load();
    }

    private void OnDestroy()
    {
        this.globalHook.KeyDown -= this.GlobalHookKeyDown;
        this.globalHook.Dispose();
    }

    private void Update()
    {
        if (this.oldMapId != this.MapId)
            this.Console.PrintInfo(true, "Map change detected, request a route reload if needed.");
        this.oldMapId = this.MapId;
    }

    private void Load(bool fromClipboard = false)
    {
        if (fromClipboard)
        {
            if (!int.TryParse(Clipboard.GetText(), out this.loadedRouteId))
            {
                this.Console.PrintError(true, "No valid route ID found in clipboard.");
                return;
            }
            this.Console.PrintInfo(true, "Loading clipboard route ID {0}.", this.loadedRouteId);
        }
        else
        {
            this.loadedRouteId = this.MapId;
            this.Console.PrintInfo(true, "Loading current map route ID {0}.", this.loadedRouteId);
        }

        try
        {
            this.Route = JsonConvert.DeserializeObject<Route>(File.ReadAllText(Path + this.loadedRouteId + ".json"));
        }
        catch (FileNotFoundException)
        {
            this.Console.PrintInfo(true, "Not found, starting new route for ID {0}.", this.loadedRouteId);
            this.Route = new Route();
        }
        this.NodeIndex = this.Route.Nodes.Any() ? 0 : -1;

        this.EditMode.Reload();
        this.FollowMode.Reload();

        this.EditMode.gameObject.SetActive(!this.UserConfig.StartInFollowMode);
        this.FollowMode.gameObject.SetActive(this.UserConfig.StartInFollowMode);
    }

    private void Save()
    {
        File.WriteAllText(Path + this.loadedRouteId + ".json", JsonConvert.SerializeObject(this.Route, Formatting.Indented));
        this.Console.PrintInfo(true, "Route ID {0} saved.", this.loadedRouteId);
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (Camera.main.cullingMask == 0)
            return;

        this.Act(e.KeyCode, e.Control);
    }
}