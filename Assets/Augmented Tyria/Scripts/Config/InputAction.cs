using Newtonsoft.Json;
using System.Windows.Forms;

public class InputAction
{
    public string ActionName;
    public string KeyName;
    public bool Control;

    [JsonIgnore]
    public Keys Key;
}