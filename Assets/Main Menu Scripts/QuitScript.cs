using TMPro;
using UnityEngine;

public class QuitScript : MonoBehaviour
{

    TextMeshPro textMesh;
    // Start is called before the first frame update
    void Start()
    {
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
        Application.Quit();
    }
}
