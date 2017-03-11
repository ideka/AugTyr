using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public class RouteHolder : MonoBehaviour
{
    public Mumble Mumble;

    [Header("Modes")]
    public EditMode EditMode;
    public FollowMode FollowMode;

    public static string Path { get { return UnityEngine.Application.streamingAssetsPath + "/Routes/"; } }

    public Route Route = new Route();

    public int NodeIndex { get; set; }

    private IKeyboardMouseEvents globalHook;

    private void Awake()
    {
        this.globalHook = Hook.GlobalEvents();
        this.globalHook.KeyDown += this.GlobalHookKeyDown;

        this.Load();

        this.EditMode.gameObject.SetActive(false);
        this.FollowMode.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        this.globalHook.KeyDown -= this.GlobalHookKeyDown;
        this.globalHook.Dispose();
    }

    private void Load()
    {
        this.Route = JsonConvert.DeserializeObject<Route>(File.ReadAllText(Path + this.Mumble.Link.GetCoordinates().MapId + ".json"));
        this.NodeIndex = this.Route.Nodes.Any() ? 0 : -1;
    }

    private void Save()
    {
        File.WriteAllText(Path + this.Mumble.Link.GetCoordinates().MapId + ".json", JsonConvert.SerializeObject(this.Route));
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
        }
    }
}