using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class PassthroughManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    public MeshRenderer meshRenderer;
    public TextMeshPro textMesh;
    private bool isPassthrough = false;

    public Camera mainCamera;
    public GameObject objectLoader;
    public GameObject test;
    private Vector3 coords;

    void Start()
    {
        canvas.enabled = isPassthrough;
        meshRenderer.enabled = isPassthrough;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three) || OVRInput.GetDown(OVRInput.Button.Four))
        {
            isPassthrough = !isPassthrough;
            Debug.Log("Toggling Passthrough");
            textMesh.text = isPassthrough ? "Passthrough: ON" : "Passthrough: OFF";
            canvas.enabled = isPassthrough;
            meshRenderer.enabled = isPassthrough;
        }

        if (isPassthrough && OVRInput.GetDown(OVRInput.Button.Two))
        {
            Debug.Log("Getting data from API");
            StartCoroutine(GetDataFromAPI());
        }
    }

    IEnumerator GetDataFromAPI()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://192.168.137.41/predictions"))
        {
            Debug.Log("API called");
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
                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        test.transform.position = hit.point;
                    }
                }
            }
        }
    }

}
