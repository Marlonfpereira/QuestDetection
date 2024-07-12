using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public Canvas controllerScript;
    private ManualPassthrough passthroughController;

    void Start()
    {
        passthroughController = controllerScript.GetComponent<ManualPassthrough>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            passthroughController.switchMode();
        }
    }
}
