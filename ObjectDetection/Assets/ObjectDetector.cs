using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // import UnityWebRequest
using Newtonsoft.Json;


public class ObjectDetector : MonoBehaviour
{
    // The prefab to spawn
    public GameObject objectLoader;
    public GameObject bottle;
    public GameObject cellphone;
    public GameObject keyboard;
    public GameObject laptop;
    public GameObject mouse;
    public GameObject tv;
    public GameObject standard;
    public LayerMask raycastLayer;
    public Camera mainCamera;
    private Vector3 coords;

    private Dictionary<string, GameObject> allObjects;

    private void Start()
    {

        // bottle = Instantiate(bottle, Vector3.zero, Quaternion.identity);
        // cellphone = Instantiate(cellphone, Vector3.zero, Quaternion.identity);
        // keyboard = Instantiate(keyboard, Vector3.zero, Quaternion.identity);
        // laptop = Instantiate(laptop, Vector3.zero, Quaternion.identity);
        // mouse = Instantiate(mouse, Vector3.zero, Quaternion.identity);
        // tv = Instantiate(tv, Vector3.zero, Quaternion.identity);
        // standard = Instantiate(standard, Vector3.zero, Quaternion.identity);

        allObjects = new Dictionary<string, GameObject>(){
            {"bottle", bottle},
            {"cellphone", cellphone},
            {"keyboard", keyboard},
            {"laptop", laptop},
            {"mouse", mouse},
            {"tv", tv},
            {"standard", standard}
        };
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

                    float x1 = obj.x1 * Screen.width;
                    float x2 = obj.x2 * Screen.width;
                    float y2 = ((obj.y2 * -1) + 1) * Screen.height;

                    coords.x = (x1 + x2) / 2;
                    coords.y = y2;
                    coords.z = 0;

                    Ray ray = mainCamera.ScreenPointToRay(coords);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, raycastLayer))
                    {
                        // GameObject currentObject = Instantiate(objectToSpawn, hit.point, Quaternion.identity);
                        if (obj.label == "bottle")
                            allObjects["bottle"].transform.position = hit.point;
                        else if (obj.label == "cellphone")
                            allObjects["cellphone"].transform.position = hit.point;
                        // else if (obj.label == "laptop")
                        //     allObjects["laptop"].transform.position = hit.point;
                        else if (obj.label == "mouse")
                            allObjects["mouse"].transform.position = hit.point;
                        else if (obj.label == "tv")
                            allObjects["tv"].transform.position = hit.point;
                        else if (obj.label == "keyboard")
                            allObjects["keyboard"].transform.position = hit.point;
                        // else
                        //     allObjects["standard"].transform.position = hit.point;
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