using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlsController : MonoBehaviour
{
    public TransformController transformController;
    public TextMeshProUGUI textMesh;

    private float radius;

    private void Start()
    {
        radius = transformController.radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Plus")
        {
            radius += 1;
        }
        else if (other.tag == "Minus")
        {
            radius -= 1;
        }
        if (radius < 1)
        {
            radius = 1;
        }
        transformController.radius = radius;
        textMesh.text = radius.ToString();
    }
}
