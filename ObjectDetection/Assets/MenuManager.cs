using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public Canvas canvas;
    public ManualPassthrough manualPassthrough;

    void Start()
    {

    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            canvas.enabled = !canvas.enabled;
            foreach (Transform child in canvas.transform)
            {
                child.gameObject.SetActive(canvas.enabled);
            }
        }
    }
}
