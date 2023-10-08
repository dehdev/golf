using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonHoverSelect : MonoBehaviour
{
    TextMeshPro textMesh;
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
    }

}
