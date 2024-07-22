using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class PassthroughObjectsFinder : MonoBehaviour
{
    public OVRHand handThatPinch;
    public GameObject controllerSphere;
    public GameObject vertexSphere;
    public GameObject objectToSpawn;

    private List<GameObject> currentSet = new List<GameObject>();
    private bool isPinchingRight = false;

    void Start()
    {
        // Initialize any necessary components
    }

    void Update()
    {
        HandleHandPinch(handThatPinch, ref isPinchingRight);
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
                CreateAveragedObject();
            }
        }
    }

    private void SpawnSphereAt(Vector3 position)
    {
        GameObject sphere = Instantiate(vertexSphere, position, Quaternion.identity);
        currentSet.Add(sphere);
    }

    private void CreateAveragedObject()
    {
        if (currentSet.Count < 2) return;

        // Get the two spheres for creating and object
        GameObject sphere1 = currentSet[currentSet.Count - 2];
        GameObject sphere2 = currentSet[currentSet.Count - 1];

        // Calculate the average position
        Vector3 middlePosition = (sphere1.transform.position + sphere2.transform.position) / 2;

        // Calculate the dimensions
        float lengthX = Mathf.Abs(sphere1.transform.position.x - sphere2.transform.position.x);
        float lengthY = Mathf.Abs(sphere1.transform.position.y - sphere2.transform.position.y);
        float lengthZ = Mathf.Abs(sphere1.transform.position.z - sphere2.transform.position.z);

        Vector3 size = new Vector3(lengthX, lengthY, lengthZ);

        // Calculate the rotation and dimensions
        Vector3 direction = sphere2.transform.position - sphere1.transform.position;

        if (lengthX >= lengthY && lengthX >= lengthZ)
        {
            // X is the longest
            float maxLength = Mathf.Max(lengthY, lengthZ);
            size = new Vector3(lengthX, maxLength, maxLength);
            direction = new Vector3(1, 0, 0);
        }
        else if (lengthY >= lengthX && lengthY >= lengthZ)
        {
            // Y is the longest
            float maxLength = Mathf.Max(lengthX, lengthZ);
            size = new Vector3(maxLength, lengthY, maxLength);
            direction = new Vector3(0, 1, 0);
        }
        else
        {
            // Z is the longest
            float maxLength = Mathf.Max(lengthX, lengthY);
            size = new Vector3(maxLength, maxLength, lengthZ);
            direction = new Vector3(0, 0, 1);
        }

        Quaternion rotation = Quaternion.LookRotation(direction);

        // put object in the average location
        GameObject avgObject = Instantiate(objectToSpawn, middlePosition, rotation);
        avgObject.transform.localScale = size;

        // Destroy spheres after 1 second
        StartCoroutine(DeleteSpheresAfterDelay(sphere1, sphere2, 1.0f));
    }

    // Iniciar la coroutine para borrar las esferas después de 1 segundo

    private System.Collections.IEnumerator DeleteSpheresAfterDelay(GameObject sphere1, GameObject sphere2, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(sphere1);
        Destroy(sphere2);
    }

}