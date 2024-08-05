using UnityEngine;
public class Cube : MonoBehaviour
{
    // Public reference to the cube object that should always face the user
    public GameObject cube;
    void Update()
    {
        // Check if the cube is assigned
        if (cube != null)
        {
            // Update the cube's rotation to face the user
            cube.transform.rotation = GetRotationFacingUser(cube.transform.position);
        }
    }
    // Method to get the rotation needed for the object to face the user
    Quaternion GetRotationFacingUser(Vector3 objectPosition)
    {
        // Calculate the direction vector from the object to the user (camera)
        Vector3 directionToUser = Camera.main.transform.position - objectPosition;
        // Create a rotation that points the object's forward vector in the direction of the user
        return Quaternion.LookRotation(directionToUser);
    }
}