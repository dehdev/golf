using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererColor : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private PlayerController playerController;

    Color greenColor = new(0.1529412f, 0.682353f, 0.3764706f);
    Color yellowColor = new(0.9529412f, 0.6117647f, 0.07058824f);
    Color redColor = new(0.9058824f, 0.2980392f, 0.2352941f);

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        float clampedStrength = playerController.GetStrength();
        Color lineColor = Color.Lerp(greenColor, yellowColor, clampedStrength);
        lineColor = Color.Lerp(lineColor, redColor, clampedStrength);

        lineRenderer.material.color = lineColor;
    }
}
