using TMPro;
using UnityEngine;

public class ApplyScript : MonoBehaviour
{

    TextMeshPro textMesh;
    [SerializeField] GameObject mainMenuButtons;
    [SerializeField] GameObject optionsMenuButtons;
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
        textMesh.color = Color.white;
        Debug.Log("Apply");
    }
}
