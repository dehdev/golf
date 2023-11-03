using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResumeButtonScript : MonoBehaviour
{

    TextMeshPro textMesh;
    PauseMenuZoom pauseMenuZoom;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenuZoom = gameObject.GetComponentInParent<PauseMenuZoom>();
        textMesh = GetComponent<TextMeshPro>();
    }

    private void OnMouseEnter()
    {
        textMesh.color = Color.red;
    }

    private void OnMouseExit()
    {
        textMesh.color = Color.white;
    }
    private void OnMouseDown()
    {
        textMesh.color = Color.white;
        pauseMenuZoom.HandleMenu();
    }
}
