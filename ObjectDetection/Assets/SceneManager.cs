using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject plane1;
    public GameObject plane2;
    void Start()
    {
        plane1.SetActive(true);
        plane2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePlane() {
        plane2.SetActive(plane1.activeSelf);
        plane1.SetActive(!plane1.activeSelf);
    }
}
