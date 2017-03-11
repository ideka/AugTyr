using MumbleLink_CSharp;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Mumble))]
public class MumbleCamera : MonoBehaviour
{
    public Camera Camera { get; private set; }
    public Mumble Mumble { get; private set; }

    private void Awake()
    {
        this.Camera = this.GetComponent<Camera>();
        this.Mumble = this.GetComponent<Mumble>();
    }

    private void Update()
    {
        this.Camera.fieldOfView = this.Mumble.Link.GetIdentity().Fov * Mathf.Rad2Deg;

        MumbleLinkedMemory mem = this.Mumble.Link.Read();
        this.transform.position = new Vector3(mem.FCameraPosition[0], mem.FCameraPosition[1], mem.FCameraPosition[2]);
        this.transform.rotation = Quaternion.LookRotation(
            new Vector3(mem.FCameraFront[0], mem.FCameraFront[1], mem.FCameraFront[2]));
    }
}