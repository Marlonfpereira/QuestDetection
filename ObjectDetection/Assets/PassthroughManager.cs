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
    public Material material;

    private Ray ray1, ray2, ray3, ray4;

    void Start()
    {
        canvas.enabled = isPassthrough;
        meshRenderer.enabled = isPassthrough;
    }

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
        using (UnityWebRequest www = UnityWebRequest.Get("http://192.168.137.1/centerPrediction"))
        {
            Debug.Log("API called");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;

                DetectedObject detectedObject = JsonConvert.DeserializeObject<DetectedObject>(response);

                foreach (Transform child in objectLoader.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                float x1 = detectedObject.x1 * Screen.width;
                float x2 = detectedObject.x2 * Screen.width;
                float y1 = ((detectedObject.y1 * -1) + 1) * Screen.height;
                float y2 = ((detectedObject.y2 * -1) + 1) * Screen.height;

                ray1 = mainCamera.ScreenPointToRay(new Vector3(x1, y1, 0));
                ray2 = mainCamera.ScreenPointToRay(new Vector3(x2, y1, 0));
                ray3 = mainCamera.ScreenPointToRay(new Vector3(x1, y2, 0));
                ray4 = mainCamera.ScreenPointToRay(new Vector3(x2, y2, 0));

                RaycastHit hit1, hit2, hit3, hit4;
                if (Physics.Raycast(ray1, out hit1, 100) && Physics.Raycast(ray2, out hit2, 100) && Physics.Raycast(ray3, out hit3, 100) && Physics.Raycast(ray4, out hit4, 100))
                {
                    Mesh mesh = new Mesh();
                    mesh.vertices = new Vector3[] { hit1.point, hit2.point, hit3.point, hit4.point };
                    mesh.triangles = new int[] { 0, 1, 2, 3, 2, 1 };

                    GameObject meshObject = new GameObject("Mesh");
                    meshObject.AddComponent<MeshFilter>().mesh = mesh;
                    meshObject.AddComponent<MeshRenderer>();
                    meshObject.GetComponent<MeshRenderer>().material = material;
                    meshObject.transform.parent = objectLoader.transform;
                }

            }
        }
    }
}
