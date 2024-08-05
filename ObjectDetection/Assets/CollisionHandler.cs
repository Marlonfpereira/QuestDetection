using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public ManualPassthrough manualPassthrough;
    private GameObject selfObject;

    void Awake()
    {
        selfObject = gameObject;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Mesh") && selfObject.CompareTag("LeftHand"))
        {
            manualPassthrough.ToggleLockButton(true, false, other.gameObject);
        }
        else if (other.gameObject.CompareTag("Mesh") && selfObject.CompareTag("RightHand"))
        {
            manualPassthrough.ToggleLockButton(true, true, other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) 
    {
        manualPassthrough.ToggleLockButton(false, false, null);
    }
}
