using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleplayerButton : MonoBehaviour
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
        Debug.Log("Singleplayer");
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
    }
}
