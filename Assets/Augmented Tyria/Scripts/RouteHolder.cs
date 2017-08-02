using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

[RequireComponent(typeof(GameDatabaseHolder))]
public class RouteHolder : MonoBehaviour, IActionable
{
    public Mumble Mumble;

    [Header("Modes")]
    public EditMode EditMode;
    public FollowMode FollowMode;

    public static string Path { get { return UnityEngine.Application.streamingAssetsPath + "/Routes/"; } }
    public static string UnofficialPath { get { return UnityEngine.Application.streamingAssetsPath + "/UnofficialRoutes/"; } }

    public Route Route = new Route();

    public int NodeIndex { get; set; }

    public MonoBehaviour Holder { get { return this; } }
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

    private int loadedRouteId;
    private int oldMapId;

    public GameDatabaseHolder GameDatabaseHolder { get; private set; }

    private void Awake()
    {
        this.GameDatabaseHolder = this.GetComponent<GameDatabaseHolder>();

        this.NodeIndex = -1;

        this.oldMapId = this.MapId;

        this.SetUp();

        this.Load();
    }

    private void Update()
    {
        if (this.oldMapId != this.MapId)
            this.Console.InfoFade("Map change detected, request a route reload if needed.");
        this.oldMapId = this.MapId;
    }

    private void Load(bool fromClipboard = false)
    {
        if (fromClipboard)
        {
            int clipboardId;
            if (!int.TryParse(Clipboard.GetText(), out clipboardId))
            {
                this.Console.ErrorFade("No valid route ID found in clipboard.");
                return;
            }
            this.loadedRouteId = clipboardId;
        }
        else
        {
            this.loadedRouteId = this.MapId;
        }

        string source = fromClipboard ? "clipboard" : "current map";
        this.TryLoadRoute(new Dictionary<string, string>
        {
            { Path, string.Format("Loaded {0} route ID {1}.", source, "{0}") },
            { UnofficialPath, string.Format("Loaded <i>unofficial</i> {0} route ID {1}.", source, "{0}") }
        }, this.loadedRouteId, out this.Route);
        this.NodeIndex = this.Route.Nodes.Any() ? 0 : -1;

        this.EditMode.Reload();
        this.FollowMode.Reload();

        this.EditMode.gameObject.SetActive(!this.UserConfig.StartInFollowMode);
        this.FollowMode.gameObject.SetActive(this.UserConfig.StartInFollowMode);
    }

    private void Save()
    {
        File.WriteAllText(Path + this.loadedRouteId + ".json", JsonConvert.SerializeObject(this.Route, Formatting.Indented));
        this.Console.InfoFade("Route ID {0} saved.", this.loadedRouteId);
    }

    private bool TryLoadRoute(Dictionary<string, string> pathMessages, int id, out Route route)
    {
        foreach (KeyValuePair<string, string> pathMessage in pathMessages)
        {
            try
            {
                route = JsonConvert.DeserializeObject<Route>(File.ReadAllText(pathMessage.Key + id + ".json"));
            }
            catch (FileNotFoundException)
            {
                continue;
            }
            this.Console.InfoFade(pathMessage.Value, id);
            return true;
        }
        this.Console.InfoFade("Not found, starting new route for ID {0}.", id);
        route = new Route();
        return false;
    }

    private bool RouteExists(List<string> search, int id)
    {
        return search.Any(p => File.Exists(p + id + ".json"));
    }
}