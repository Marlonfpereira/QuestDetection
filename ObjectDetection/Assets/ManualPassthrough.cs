using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

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


    void Start()
    {
        meshesLoader = new GameObject("MeshesLoader");
        canvas.enabled = isPassthrough;
        meshRenderer.enabled = isPassthrough;
    }

    void Update()
    {
        Vector3 controllerPos = rightController.transform.position + rightController.transform.forward * 0.05f;

        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            isPassthrough = !isPassthrough;

            canvas.enabled = isPassthrough;
            meshRenderer.enabled = isPassthrough;
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

}
