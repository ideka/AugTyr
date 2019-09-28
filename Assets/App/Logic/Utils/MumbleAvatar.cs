using MumbleLink_CSharp;
using UnityEngine;

public class MumbleAvatar : MonoBehaviour
{
    public Mumble Mumble;

    private void Update()
    {
        MumbleLinkedMemory mem = this.Mumble.Link.Read();
        this.transform.position = new Vector3(mem.FAvatarPosition[0], mem.FAvatarPosition[1], mem.FAvatarPosition[2]);
    }
}