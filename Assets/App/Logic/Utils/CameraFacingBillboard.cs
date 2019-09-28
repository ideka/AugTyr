using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    private void Awake()
    {
        this.LateUpdate();
    }

    private void LateUpdate()
    {
        transform.LookAt(
            transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }
}