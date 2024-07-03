using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ManualPassthrough : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    public MeshRenderer meshRenderer;
    private bool isPassthrough = false;

    public Camera mainCamera;
    public GameObject objectLoader;
    private GameObject meshesLoader;
    public Material passthroughMaterial;
    public GameObject testSphere;
    public GameObject rightController;
    private bool triggerPressed = false; 
    private List<GameObject> currentSet = new List<GameObject>();

    private GameObject autoLoader;
    private Ray ray1, ray2, ray3, ray4;

    void Start()
    {
        meshesLoader = new GameObject("MeshesLoader");
        autoLoader = new GameObject("AutoLoader");
        canvas.enabled = isPassthrough;
        meshRenderer.enabled = isPassthrough;
    }

    void Update()
    {
        Vector3 controllerPos = rightController.transform.position + rightController.transform.forward * 0.05f;

        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            isPassthrough = !isPassthrough;

            canvas.enabled = isPassthrough;
            meshRenderer.enabled = isPassthrough;
        }

        if (isPassthrough && OVRInput.GetDown(OVRInput.Button.Three))
        {
            Debug.Log("Getting data from API");
            StartCoroutine(GetDataFromAPI());
        }

        if (OVRInput.GetDown(OVRInput.Button.Four)) {
            foreach (Transform child in autoLoader.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) && !triggerPressed)
        {
            triggerPressed = true;

            GameObject sphere = Instantiate(testSphere, controllerPos, Quaternion.identity);
            sphere.transform.parent = objectLoader.transform;
            currentSet.Add(sphere);
        }

        if (!OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            triggerPressed = false;
        }

        if (OVRInput.Get(OVRInput.Button.One) && currentSet.Count > 2)
        {
            foreach (Transform child in meshesLoader.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            var pbMeshObj = new GameObject();
            ProBuilderMesh pbMesh = pbMeshObj.AddComponent<ProBuilderMesh>();

            Vector3[] vertices = new Vector3[currentSet.Count];
            for (int i = 0; i < currentSet.Count; i++)
            {
                vertices[i] = currentSet[i].transform.position;
            }

            pbMesh.CreateShapeFromPolygon(vertices, 0f, false);
            pbMesh.SetMaterial(pbMesh.faces, passthroughMaterial);
            pbMesh.ToMesh();
            pbMesh.Refresh();
            pbMeshObj.transform.parent = meshesLoader.transform;
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            currentSet.Clear();
            foreach (Transform child in objectLoader.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in meshesLoader.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        testSphere.transform.position = controllerPos;
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

                foreach (Transform child in autoLoader.transform)
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
                    meshObject.GetComponent<MeshRenderer>().material = passthroughMaterial;
                    meshObject.transform.parent = autoLoader.transform;
                }

            }
        }
    }

}
