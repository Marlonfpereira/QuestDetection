using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject plane1;
    public Material[] materials = new Material[11];
    public AudioSource audioSource;
    public AudioManager audioManager;
    private bool isPlaying = false;
    private int currentStep = 0;
    void Start()
    {
        plane1.SetActive(true);;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentStep++;
            Debug.Log(currentStep);
            ManageSteps();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            currentStep--;
            Debug.Log(currentStep);
            ManageSteps();
        }
        else if (Input.GetMouseButton(2))
        {
            //reset
            currentStep = 0;
            Debug.Log(currentStep);
            ManageSteps();
        }
    }

    private void ManageSteps()
    {
        if (currentStep == 0)
        {
            audioManager.StopAllCoroutines();
        }
        else if (currentStep == -1)
        {
            audioManager.RemoteStart();
        }
        else if (currentStep == -2)
        {
            StartCoroutine(audioManager.WaitAndPlayNextClip());
        }

        plane1.GetComponent<MeshRenderer>().material = materials[currentStep%11];
    }
}