using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectGesture : MonoBehaviour
{
    // Serialize the object to appear
    [SerializeField]
    public GameObject objectToAppear;

    // Assuming you have a reference to the hand object or its transform
    [SerializeField]
    public Transform handTransform;

    // Boolean variable to track the gesture state
    private bool gestureDetected = false;

    // Function to set the gesture detected state to true
    public void SetGestureDetected()
    {
        gestureDetected = true;
    }

    // Function to set the gesture detected state to false
    public void SetGestureNotDetected()
    {
        gestureDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gestureDetected)
        {
            // Enable the object and set its position to the hand's position
            if (objectToAppear != null && handTransform != null)
            {
                objectToAppear.SetActive(true);
                objectToAppear.transform.position = handTransform.position;
                objectToAppear.transform.rotation = handTransform.rotation;
            }
        }
        else
        {
            // Disable the object when the gesture is not detected
            if (objectToAppear != null)
            {
                objectToAppear.SetActive(false);
            }
        }
    }
}
