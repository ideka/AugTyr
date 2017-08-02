using Newtonsoft.Json;
using System.Windows.Forms;

public class InputAction
{
    public string ActionName;
    public string KeyName;
    public bool Control;

    [JsonIgnore]
    public Keys Key;

    public bool Duplicate(InputAction other)
    {
        return this.ActionName == other.ActionName && this.Key == other.Key && this.Control == other.Control;
    }
}