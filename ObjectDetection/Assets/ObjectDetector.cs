using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // import UnityWebRequest



public class SpawnObjectInMiddle : MonoBehaviour
{
    // The prefab to spawn
    public GameObject objectToSpawn;
    public LayerMask raycastLayer;
    public Camera mainCamera;

    private void Start()
    {

        // Calculate the middle point
        // Vector2 middlePoint = (point1 + point2) / 2;

        // // Instantiate the object at the middle point
        // Instantiate(objectToSpawn, new Vector3(middlePoint.x, middlePoint.y, 0), Quaternion.identity);


    }

    void Update()
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);


        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, raycastLayer))
        {
            Debug.Log("Ray hit " + hit.collider.gameObject.name);
        }


        StartCoroutine(GetDataFromAPI());
    }

    IEnumerator GetDataFromAPI()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost/predictions"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                Debug.Log("response:");
                Debug.Log(response);
            }
            else
            {
                Debug.Log(www.error);
            }
        }

    }

}

