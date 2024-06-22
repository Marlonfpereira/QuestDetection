using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassthroughManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    public MeshRenderer meshRenderer;

    public TextMeshPro textMesh;
    private bool isPassthrough = false;

    void Start()
    {
        canvas.enabled = isPassthrough;
        meshRenderer.enabled = isPassthrough;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Three) || OVRInput.GetDown(OVRInput.Button.Four))
        {
            isPassthrough = !isPassthrough;
            Debug.Log("Toggling Passthrough");
            textMesh.text = isPassthrough ? "Passthrough: ON" : "Passthrough: OFF";
            canvas.enabled = isPassthrough;
            meshRenderer.enabled = isPassthrough;
        }
    }
}
