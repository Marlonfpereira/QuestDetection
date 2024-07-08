using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ManualPassthrough : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    public MeshRenderer passthroughMesh;
    private bool isPassthrough = false;
    public Material passthroughMaterial;
    public Material standardMaterial;
    public GameObject rightController;
    public GameObject controllerSphere;
    public GameObject vertexSphere;
    public GameObject meshWrapper;
    private bool triggerPressed = false; 
    private List<GameObject> currentSet = new List<GameObject>();
    private GameObject cube;


    void Start()
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<MeshRenderer>().material = standardMaterial;
        cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        canvas.enabled = isPassthrough;
        passthroughMesh.enabled = isPassthrough;
    }

    void Update()
    {
        Vector3 controllerPos = rightController.transform.position + rightController.transform.forward * 0.05f;

        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            isPassthrough = !isPassthrough;

            canvas.enabled = isPassthrough;
            passthroughMesh.enabled = isPassthrough;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            triggerPressed = true;

            GameObject sphere = Instantiate(vertexSphere, controllerPos, Quaternion.identity);
            currentSet.Add(sphere);
        }

        if (!OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            triggerPressed = false;
        }

        if (OVRInput.GetDown(OVRInput.Button.One) && currentSet.Count > 2)
        {
            Vector3[] vertices = new Vector3[currentSet.Count];
            Vector3 middle = Vector3.zero;
            for (int i = 0; i < currentSet.Count; i++)
                middle += currentSet[i].transform.position;
            
            middle /= currentSet.Count;
            for (int i = 0; i < currentSet.Count; i++)
                vertices[i] = currentSet[i].transform.position - middle;

            cube.transform.position = middle;
            var pbMeshObj = Instantiate(meshWrapper, middle, Quaternion.identity);
            ProBuilderMesh pbMesh = pbMeshObj.AddComponent<ProBuilderMesh>();

            pbMesh.CreateShapeFromPolygon(vertices, 0f, false);
            pbMesh.SetMaterial(pbMesh.faces, passthroughMaterial);
            pbMesh.ToMesh();
            pbMesh.Refresh();
            
            MeshCollider meshCollider = pbMeshObj.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = pbMeshObj.GetComponent<MeshFilter>().mesh;
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            currentSet.Clear();
        }

        controllerSphere.transform.position = controllerPos;
    }
}
