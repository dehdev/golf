using TMPro;
using UnityEngine;

public class OptionsButton : MonoBehaviour
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
    private void OnMouseUp()
    {
        mainMenuButtons.SetActive(false);
        optionsMenuButtons.SetActive(true);
        textMesh.color = Color.white;
    }
}
