using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject plane1;
    public Material[] materials = new Material[20];
    public AudioSource audioSource;
    public AudioManager audioManager;
    private bool isPlaying = false;
    private int currentStep = -1;
    void Start()
    {
        plane1.SetActive(true); ;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentStep++;
            Debug.Log(currentStep);
            ManageSteps();
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentStep--;
            Debug.Log(currentStep);
            ManageSteps();
        }
        else if (Input.GetMouseButton(2) || Input.GetKeyDown(KeyCode.Backspace))
        {
            //reset
            currentStep = -1;
            Debug.Log(currentStep);
            ManageSteps();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            currentStep = 4;
            ManageSteps();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            currentStep = 6;
            ManageSteps();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            currentStep = 15;
            ManageSteps();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            currentStep = 17;
            ManageSteps();
        } else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            currentStep = 19;
            ManageSteps();
        }
    }

    private void ManageSteps()
    {
        // if (currentStep == 0)
        // {
        //     audioManager.StopAllCoroutines();
        // }
        // else if (currentStep == -1)
        // {
        //     audioManager.RemoteStart();
        // }
        // else if (currentStep == -2)
        // {
        //     StartCoroutine(audioManager.WaitAndPlayNextClip());
        // }

        audioManager.StopAllCoroutines();
        audioManager.PlayClip(currentStep);

        plane1.GetComponent<MeshRenderer>().material = materials[currentStep % 20];
    }
}