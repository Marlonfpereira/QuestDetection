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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Mesh") && selfObject.CompareTag("LeftHand"))
        {
            Debug.Log("Object collided with: " + other.gameObject.name);
            manualPassthrough.ToggleLockButton(true, false, other.gameObject);
        }
        else if (other.gameObject.CompareTag("Mesh") && selfObject.CompareTag("RightHand"))
        {
            Debug.Log("Object collided with: " + other.gameObject.name);
            manualPassthrough.ToggleLockButton(true, true, other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) 
    {
        manualPassthrough.ToggleLockButton(false, false, null);
    }
}
