using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassthroughManager : MonoBehaviour
{
    // Start is called before the first frame update
    public OVRPassthroughLayer passthroughLayer;

    public TextMeshPro textMesh;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Three) || OVRInput.GetDown(OVRInput.Button.Four) ) 
        {
            Debug.Log("Toggling Passthrough");
            textMesh.text = passthroughLayer.overlayType == OVROverlay.OverlayType.Underlay ? "Passthrough: OFF" : "Passthrough: ON";
            passthroughLayer.overlayType = passthroughLayer.overlayType == OVROverlay.OverlayType.Underlay ? OVROverlay.OverlayType.None : OVROverlay.OverlayType.Underlay;
        }
    }
}
