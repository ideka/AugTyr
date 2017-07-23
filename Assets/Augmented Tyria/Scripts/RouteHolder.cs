using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RouteHolder : MonoBehaviour
{
    public Mumble Mumble;

    [Header("Modes")]
    public EditMode EditMode;
    public FollowMode FollowMode;

    public static string Path { get { return UnityEngine.Application.streamingAssetsPath + "/Routes/"; } }

    [HideInInspector]
    public GameDatabaseHolder GameDatabaseHolder;

    public Route Route = new Route();

    public int NodeIndex { get; set; }
    public GameDatabase GameDatabase { get { return this.GameDatabaseHolder.GameDatabase; } }
    public int MapId { get { return this.Mumble.Link.GetCoordinates().MapId; } }

    private IKeyboardMouseEvents globalHook;
    private int loadedRouteId;

    private void Awake()
    {
        this.NodeIndex = -1;

        this.globalHook = Hook.GlobalEvents();
        this.globalHook.KeyDown += this.GlobalHookKeyDown;

        this.GameDatabaseHolder = FindObjectOfType<GameDatabaseHolder>();
        if (this.GameDatabaseHolder == null)
        {
            SceneManager.LoadScene("Load");
            return;
        }

        this.Load();
    }

    private void OnDestroy()
    {
        this.globalHook.KeyDown -= this.GlobalHookKeyDown;
        this.globalHook.Dispose();
    }

    private void Load(bool fromClipboard = false)
    {
        if (!fromClipboard || !int.TryParse(Clipboard.GetText(), out this.loadedRouteId))
            this.loadedRouteId = this.MapId;

        try
        {
            this.Route = JsonConvert.DeserializeObject<Route>(File.ReadAllText(Path + this.loadedRouteId + ".json"));
        }
        catch (FileNotFoundException)
        {
            this.Route = new Route();
        }
        this.NodeIndex = this.Route.Nodes.Any() ? 0 : -1;

        this.EditMode.Reload();
        this.FollowMode.Reload();

        this.EditMode.gameObject.SetActive(true);
        this.FollowMode.gameObject.SetActive(false);
    }

    private void Save()
    {
        File.WriteAllText(Path + this.loadedRouteId + ".json", JsonConvert.SerializeObject(this.Route, Formatting.Indented));
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (Camera.main.cullingMask == 0)
            return;

        switch (e.KeyCode)
        {
            case Keys.Decimal:
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
                break;

            case Keys.Multiply:
                this.Save();
                break;

            case Keys.Divide:
                this.Load(e.Control);
                break;
        }
    }
}