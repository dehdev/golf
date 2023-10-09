using TMPro;
using UnityEngine;

public class MultiplayerButton : MonoBehaviour
{

    TextMeshPro textMesh;
    [SerializeField] GameObject mainMenuButtons;
    [SerializeField] GameObject multiplayerMenuButtons;

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
        mainMenuButtons.SetActive(false);
        multiplayerMenuButtons.SetActive(true);
        textMesh.color = Color.white;
    }
}
