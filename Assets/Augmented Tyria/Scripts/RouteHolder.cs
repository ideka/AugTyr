using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

public class RouteHolder : MonoBehaviour
{
    public Mumble Mumble;

    public static string Path { get { return UnityEngine.Application.streamingAssetsPath + "/Routes/"; } }

    public Route Route = new Route();

    private IKeyboardMouseEvents globalHook;

    private void Awake()
    {
        this.globalHook = Hook.GlobalEvents();
        this.globalHook.KeyDown += this.GlobalHookKeyDown;

        this.Load();
    }

    private void OnDestroy()
    {
        this.globalHook.KeyDown -= this.GlobalHookKeyDown;
        this.globalHook.Dispose();
    }

    private void Load()
    {
        this.Route = JsonConvert.DeserializeObject<Route>(File.ReadAllText(Path + this.Mumble.Link.GetCoordinates().MapId + ".json"));
    }

    private void Save()
    {
        File.WriteAllText(Path + this.Mumble.Link.GetCoordinates().MapId + ".json", JsonConvert.SerializeObject(this.Route));
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (Camera.main.cullingMask == 0)
            return;

        if (e.KeyCode == Keys.Multiply)
            this.Save();
    }
}