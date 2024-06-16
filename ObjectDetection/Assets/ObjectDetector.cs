using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // import UnityWebRequest
using Newtonsoft.Json;


public class ObjectDetector : MonoBehaviour
{
    // The prefab to spawn
    public GameObject objectLoader;
    public GameObject objectToSpawn;
    public LayerMask raycastLayer;
    public Camera mainCamera;
    private Vector3 coords;

    private void Start()
    {

        // Calculate the middle point
        // Vector2 middlePoint = (point1 + point2) / 2;

        // // Instantiate the object at the middle point
        // Instantiate(objectToSpawn, new Vector3(middlePoint.x, middlePoint.y, 0), Quaternion.identity);


    }

    void Update()
    {

        // Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);


        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit, 100, raycastLayer))
        // {
        //     Debug.Log("Ray hit " + hit.collider.gameObject.name);
        // }

        Ray ray = mainCamera.ScreenPointToRay(coords);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);



        StartCoroutine(GetDataFromAPI());
    }

    IEnumerator GetDataFromAPI()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://192.168.137.41/predictions"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;

                ObjectsList detectedObject = JsonConvert.DeserializeObject<ObjectsList>(response);

                foreach (DetectedObject obj in detectedObject.predictions)
                {
                    if (obj.label == "person") continue;

                    // Debug.Log("x1: " + obj.x1 + " y1: " + obj.y1 + " x2: " + obj.x2 + " y2: " + obj.y2 + " label: " + obj.label);
                    float x1 = obj.x1 * Screen.width;
                    float x2 = obj.x2 * Screen.width;
                    float y1 = obj.y1 * Screen.height;
                    float y2 = ((obj.y2 * -1) + 1) * Screen.height;

                    coords.x = (x1 + x2) / 2;
                    coords.y = y2;
                    coords.z = 0;
                    
                    Ray ray = mainCamera.ScreenPointToRay(coords);
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);


                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, raycastLayer))
                    {
                        Debug.Log(obj.label + " hit " + hit.collider.gameObject.name);
                    }
                }
            }
        }

    }

}


class ObjectsList
{
    public DetectedObject[] predictions;
}

class DetectedObject
{

    public float x1;
    public float y1;
    public float x2;
    public float y2;
    public string label;
}