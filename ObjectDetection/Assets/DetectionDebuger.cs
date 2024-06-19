using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // import UnityWebRequest
using Newtonsoft.Json;


public class DetectionDebuger : MonoBehaviour
{
    // The prefab to spawn
    public GameObject objectLoader;
    public GameObject objectToSpawn;
    public LayerMask raycastLayer;
    public Camera mainCamera;
    public GameObject point;
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public GameObject point4;
    public GameObject corner1;
    public GameObject corner2;
    public GameObject corner3;
    public GameObject corner4;
    private Vector3 coords;
    private void Start()
    {

    }

    void Update()
    {

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


                foreach (Transform child in objectLoader.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                foreach (DetectedObject obj in detectedObject.predictions)
                {
                    if (obj.label == "person") continue;

                    // Debug.Log("x1: " + obj.x1 + " y1: " + obj.y1 + " x2: " + obj.x2 + " y2: " + obj.y2 + " label: " + obj.label);
                    float x1 = obj.x1 * Screen.width;
                    float x2 = obj.x2 * Screen.width;
                    float y1 = ((obj.y1 * -1) + 1) * Screen.height;
                    float y2 = ((obj.y2 * -1) + 1) * Screen.height;

                    coords.x = (x1 + x2) / 2;
                    coords.y = y2;//(y2 + y1) / 2;
                    coords.z = 0;

                    Ray ray = mainCamera.ScreenPointToRay(coords);
                    // Debug.Log(coords);
                    // Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

                    Ray ray1 = mainCamera.ScreenPointToRay(new Vector3(x1, y1, 0));
                    Ray ray2 = mainCamera.ScreenPointToRay(new Vector3(x2, y2, 0));
                    Ray ray3 = mainCamera.ScreenPointToRay(new Vector3(x1, y2, 0));
                    Ray ray4 = mainCamera.ScreenPointToRay(new Vector3(x2, y1, 0));


                    RaycastHit hit;

                    if (Physics.Raycast(ray1, out hit, 100, raycastLayer))
                        point1.transform.position = hit.point;

                    if (Physics.Raycast(ray2, out hit, 100, raycastLayer))
                        point2.transform.position = hit.point;

                    if (Physics.Raycast(ray3, out hit, 100, raycastLayer))
                        point3.transform.position = hit.point;

                    if (Physics.Raycast(ray4, out hit, 100, raycastLayer))
                        point4.transform.position = hit.point;

                    if (Physics.Raycast(ray, out hit, 100, raycastLayer))
                    {
                        point.transform.position = hit.point;
                        // Debug.Log(obj.label + " hit " + hit.collider.gameObject.name);

                        // GameObject currentObject = Instantiate(objectToSpawn, hit.point, Quaternion.identity);
                        // currentObject.transform.parent = objectLoader.transform;
                    }

                    Ray cornerRay1 = mainCamera.ScreenPointToRay(new Vector3(0, 0, 0));
                    Ray cornerRay2 = mainCamera.ScreenPointToRay(new Vector3(0, Screen.height, 0));
                    Ray cornerRay3 = mainCamera.ScreenPointToRay(new Vector3(Screen.width, 0, 0));
                    Ray cornerRay4 = mainCamera.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));

                    if (Physics.Raycast(cornerRay1, out hit, 100, raycastLayer))
                        corner1.transform.position = hit.point;
                    if (Physics.Raycast(cornerRay2, out hit, 100, raycastLayer))
                        corner2.transform.position = hit.point;
                    if (Physics.Raycast(cornerRay3, out hit, 100, raycastLayer))
                        corner3.transform.position = hit.point;
                    if (Physics.Raycast(cornerRay4, out hit, 100, raycastLayer))
                        corner4.transform.position = hit.point;
                }
            }
        }

    }

}