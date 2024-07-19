using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class PassthroughObjectsFinder : MonoBehaviour
{
    public OVRHand rightHand;
    public OVRHand leftHand;
    public GameObject controllerSphere;
    public GameObject vertexSphere;
    public GameObject averagedSpherePrefab;

    private List<GameObject> currentSet = new List<GameObject>();
    private bool isPinchingRight = false;
    private bool isPinchingLeft = false;

    void Start()
    {
        // Initialize any necessary components
    }

    void Update()
    {
        HandleHandPinch(rightHand, ref isPinchingRight);
        HandleHandPinch(leftHand, ref isPinchingLeft);
    }

    private void HandleHandPinch(OVRHand hand, ref bool isPinching)
    {
        Vector3 controllerPos = hand.GetComponent<OVRSkeleton>().Bones[8].Transform.position;
        controllerSphere.transform.position = controllerPos;

        if (hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if (!isPinching)
            {
                isPinching = true;
                SpawnSphereAt(controllerPos);
            }
        }
        else if (isPinching)
        {
            isPinching = false;
            SpawnSphereAt(controllerPos);
            if (currentSet.Count >= 2)
            {
                CreateAveragedSphere();
            }
        }
    }

    private void SpawnSphereAt(Vector3 position)
    {
        GameObject sphere = Instantiate(vertexSphere, position, Quaternion.identity);
        currentSet.Add(sphere);
    }

    private void CreateAveragedSphere()
    {
        if (currentSet.Count < 2) return;

        // Get the last two spheres
        GameObject sphere1 = currentSet[currentSet.Count - 2];
        GameObject sphere2 = currentSet[currentSet.Count - 1];

        // Calculate the average position
        Vector3 avgPosition = (sphere1.transform.position + sphere2.transform.position) / 2;

        // Calculate the dimensions
        Vector3 dimensions = new Vector3(
            Mathf.Abs(sphere1.transform.position.x - sphere2.transform.position.x),
            Mathf.Abs(sphere1.transform.position.y - sphere2.transform.position.y),
            Mathf.Abs(sphere1.transform.position.z - sphere2.transform.position.z)
        );

        // Instantiate a new sphere at the average position with the calculated dimensions
        GameObject avgSphere = Instantiate(averagedSpherePrefab, avgPosition, Quaternion.identity);
        avgSphere.transform.localScale = dimensions;
    }
}