using System.Collections.Generic;
using Oculus.Interaction;
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
    public GrabInteractor grabInteractor;
    private List<GameObject> currentSet = new List<GameObject>();
    private GameObject currentMesh;

    void Start()
    {
        currentMesh = new GameObject();
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
            GameObject sphere = Instantiate(vertexSphere, controllerPos, Quaternion.identity);
            currentSet.Add(sphere);
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

            var pbMeshObj = Instantiate(meshWrapper, middle, Quaternion.identity);
            pbMeshObj.transform.parent = currentMesh.transform;
            ProBuilderMesh pbMesh = pbMeshObj.AddComponent<ProBuilderMesh>();

            pbMesh.CreateShapeFromPolygon(vertices, 0.05f, false);
            pbMesh.SetMaterial(pbMesh.faces, passthroughMaterial);
            pbMesh.ToMesh();
            pbMesh.Refresh();

            MeshCollider meshCollider = pbMeshObj.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = pbMeshObj.GetComponent<MeshFilter>().mesh;

            foreach (GameObject obj in currentSet)
                Destroy(obj);
            currentSet.Clear();
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            foreach (GameObject obj in currentSet)
            {
                Destroy(obj);
            }
            currentSet.Clear();

            if (grabInteractor.HasSelectedInteractable)
                Destroy(grabInteractor.SelectedInteractable.transform.parent.gameObject);
                // Destroy(grabInteractor.SelectedInteractable);
        }

        controllerSphere.transform.position = controllerPos;
    }
    
    public void switchMode()
    {
        isPassthrough = !isPassthrough;
        canvas.enabled = isPassthrough;
        passthroughMesh.enabled = isPassthrough;
    }
}
