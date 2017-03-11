using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Mumble))]
public class SceneRestarter : MonoBehaviour
{
    public int CurrentMapId { get; private set; }

    public Mumble Mumble { get; private set; }

    private void Awake()
    {
        this.Mumble = this.GetComponent<Mumble>();

        this.CurrentMapId = this.Mumble.Link.GetCoordinates().MapId;
    }
    
    private void Update()
    {
        if (this.Mumble.Link.GetCoordinates().MapId != this.CurrentMapId)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}