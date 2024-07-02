using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera targetCamera;

    void Start()
    {
        // Find the OVRCameraRig in the scene
        var ovrCameraRig = FindObjectOfType<OVRCameraRig>();
        if (ovrCameraRig != null)
        {
            // Get the center eye camera from the OVRCameraRig
            targetCamera = ovrCameraRig.centerEyeAnchor.GetComponent<Camera>();
        }
        else
        {
            Debug.LogError("OVRCameraRig not found in the scene.");
        }
    }

    void Update()
    {
        if (targetCamera != null)
        {
            Vector3 direction = targetCamera.transform.position - transform.position;
            direction.y = 0; // Keep only the horizontal direction
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}

