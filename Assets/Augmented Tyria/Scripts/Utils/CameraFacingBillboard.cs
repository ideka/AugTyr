using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    private void Awake()
    {
        this.Update();
    }

    private void Update()
    {
        transform.LookAt(
            transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }
}