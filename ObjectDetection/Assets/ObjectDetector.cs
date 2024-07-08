using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // import UnityWebRequest
using Newtonsoft.Json;


public class ObjectDetector : MonoBehaviour
{
    // The prefab to spawn
    public GameObject objectLoader;
    public GameObject shape;
    public LayerMask raycastLayer;
    public Camera mainCamera;
   
    private Vector3 coords;
    private GameObject point;
    private GameObject point1;
    private GameObject point2;
    private GameObject point3;
    private GameObject point4;
    private void Start()
    {
        coords = new Vector3();
        point3 = new GameObject();
        point4 = new GameObject();
    }

    void Update()
    {
        //Ray ray = mainCamera.ScreenPointToRay(coords);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

        StartCoroutine(GetDataFromAPI());
    }

    IEnumerator GetDataFromAPI()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://192.168.137.1/predictions"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;

                ObjectsList detectedObject = JsonConvert.DeserializeObject<ObjectsList>(response);


                if(detectedObject.predictions.Length != 0) 
                foreach (Transform child in objectLoader.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                foreach (DetectedObject obj in detectedObject.predictions)
                {
                    Debug.Log(obj.label);
                    float x1 = obj.x1 * Screen.width;
                    float x2 = obj.x2 * Screen.width;
                    float y1 = ((obj.y1 * -1) + 1) * Screen.height;
                    float y2 = ((obj.y2 * -1) + 1) * Screen.height;

                    coords.x = (x1 + x2) / 2;
                    coords.y = y2;
                    coords.z = 0;

                    Ray ray = mainCamera.ScreenPointToRay(coords);
                    Ray ray1 = mainCamera.ScreenPointToRay(new Vector3(x1, y1, 0));
                    Ray ray2 = mainCamera.ScreenPointToRay(new Vector3(x2, y1, 0));
                    Ray ray3 = mainCamera.ScreenPointToRay(new Vector3(x1, y2, 0));
                    Ray ray4 = mainCamera.ScreenPointToRay(new Vector3(x2, y2, 0));


                    RaycastHit hit;

                    if (Physics.Raycast(ray3, out hit, 100, raycastLayer))
                        point3.transform.position = hit.point;

                    if (Physics.Raycast(ray4, out hit, 100, raycastLayer))
                        point4.transform.position = hit.point;

                    Vector3 widthVector = point4.transform.position - point3.transform.position; 
                    float width = widthVector.magnitude;

                    float ogWidth = obj.x2 - obj.x1;
                    float ogHeight = obj.y2 - obj.y1;
                    float height = (width * ogHeight) / ogWidth;

                    // got the `width` and `height` variables

                    if (Physics.Raycast(ray, out hit, 100, raycastLayer))
                    {
                        Debug.Log(width);
                        GameObject currentObject = Instantiate(shape, hit.point, Quaternion.identity);
                        newScale(currentObject, height*10, width*10);
                        currentObject.transform.parent = objectLoader.transform;
                    }
                }
            }
        }

    }

    public void newScale(GameObject theGameObject, float height, float width) {

        float sizex = theGameObject.GetComponent<Renderer>().bounds.size.x;
        float sizey = theGameObject.GetComponent<Renderer>().bounds.size.y;
        float sizez = theGameObject.GetComponent<Renderer>().bounds.size.z;

        Vector3 rescale = theGameObject.transform.localScale;

        rescale.x = width * rescale.x / width;
        rescale.y = height * rescale.y / height;
        rescale.z = width * rescale.z / width;

        Debug.Log(rescale);

        theGameObject.transform.localScale = rescale;
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