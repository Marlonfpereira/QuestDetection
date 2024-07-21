using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
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
    public OVRHand rightHand;
    public OVRHand leftHand;
    public GameObject controllerSphere;
    public GameObject vertexSphere;
    public GameObject meshWrapper;
    public HandGrabInteractor grabInteractorR;
    public HandGrabInteractor grabInteractorL;
    private List<GameObject> currentSet = new List<GameObject>();
    private GameObject currentMesh;
    private bool isCreating = false;
    private bool spawningVertice = false;
    public TextMeshPro buttonText;
    public PokeInteractable createInteractable;
    public PokeInteractable deleteInteractable;
    public Camera mainCamera;
    public LayerMask raycastLayer;

    void Start()
    {
        currentMesh = new GameObject();
        canvas.enabled = isPassthrough;
        passthroughMesh.enabled = isPassthrough;
    }

    private bool isLongPinch = false;
    private float pinchTimer = 0f;
    private float pinchDuration = .5f;

    void Update()
    {
        Vector3 controllerPos = rightHand.GetComponent<OVRSkeleton>().Bones[8].Transform.position;

        if (isCreating)
        {
            controllerSphere.transform.position = controllerPos;

            if (rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                if (!spawningVertice)
                {
                    spawningVertice = true;
                    GameObject sphere = Instantiate(vertexSphere, controllerPos, Quaternion.identity);
                    currentSet.Add(sphere);
                }
                else
                {
                    pinchTimer += Time.deltaTime;
                    if (pinchTimer >= pinchDuration)
                    {
                        isLongPinch = true;
                    }
                }
            }
            else
            {
                spawningVertice = false;

                if (isLongPinch)
                {
                    GameObject sphere = Instantiate(vertexSphere, controllerPos, Quaternion.identity);
                    currentSet.Add(sphere);
                    ToggleCreate();
                }

                pinchTimer = 0f;
                isLongPinch = false;
            }
        }

        if (grabInteractorL.HasSelectedInteractable)
        {
            deleteInteractable.gameObject.SetActive(true);
        }
        else
        {
            deleteInteractable.gameObject.SetActive(false);
        }

        Debug.Log(leftHand.GetComponent<OVRSkeleton>().Bones[8].Transform.rotation);

        if(leftHand.GetComponent<OVRSkeleton>().Bones[8].Transform.rotation.x < 0) {
            createInteractable.gameObject.SetActive(true);
        } else {
            deleteInteractable.gameObject.SetActive(false);
            createInteractable.gameObject.SetActive(false);
        }

    }
    public void ToggleCreate()
    {
        if (!isCreating)
        {
            isCreating = true;
            buttonText.text = "Finnish";
        }
        else
        {
            isCreating = false;
            buttonText.text = "Create";
            CreateMesh();
            controllerSphere.transform.position = Vector3.zero;
        }
    }

    public void CreateMesh()
    {
        if (currentSet.Count == 2)
        {
            GameObject aux1 = Instantiate(vertexSphere, new Vector3(currentSet[0].transform.position.x, currentSet[1].transform.position.y, currentSet[0].transform.position.z), Quaternion.identity);
            GameObject aux2 = Instantiate(vertexSphere, new Vector3(currentSet[1].transform.position.x, currentSet[0].transform.position.y, currentSet[1].transform.position.z), Quaternion.identity);
            currentSet = new List<GameObject> { currentSet[0], aux1, currentSet[1], aux2 };
        }

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

    public void DestroyMesh()
    {
        foreach (GameObject obj in currentSet)
        {
            Destroy(obj);
        }
        currentSet.Clear();

        if (grabInteractorL.HasSelectedInteractable)
            Destroy(grabInteractorL.SelectedInteractable.transform.parent.gameObject);
    }
}
