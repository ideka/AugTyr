using MumbleLink_CSharp_GW2;
using UnityEngine;

public class Mumble : MonoBehaviour
{
    public GW2Link Link { get; private set; }

    private void Awake()
    {
        this.Link = new GW2Link();
    }

    private void OnDestroy()
    {
        this.Link.Dispose();
    }
}