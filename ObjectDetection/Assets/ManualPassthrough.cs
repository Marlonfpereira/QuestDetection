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
    public MeshRenderer passthroughMesh;
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
    public PokeInteractable LLockButton;
    public PokeInteractable RLockButton;
    public Camera mainCamera;
    public LayerMask raycastLayer;
    public TextMeshProUGUI debugText;
    private bool righHanded = true;
    private GameObject objectToLock;

    void Start()
    {
        currentMesh = new GameObject();
    }

    private bool isLongPinch = false;
    private float pinchTimer = 0f;
    private float pinchDuration = .5f;
    private GameObject wireframe;
    private bool buttonsVisible = true;

    void Update()
    {
        Vector3 controllerPos = rightHand.GetComponent<OVRSkeleton>().Bones[8].Transform.position;

        if (isCreating)
        {
            controllerSphere.transform.position = controllerPos;

            if (currentSet.Count >= 2)
            {
                if (wireframe != null)
                {
                    Destroy(wireframe);
                }
                wireframe = new GameObject();
                LineRenderer lineRenderer = wireframe.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.green };
                lineRenderer.positionCount = currentSet.Count+1;
                for (int i = 0; i < currentSet.Count; i++)
                {
                    lineRenderer.SetPosition(i, currentSet[i].transform.position);
                }
                lineRenderer.SetPosition(currentSet.Count, currentSet[0].transform.position);
                wireframe.transform.parent = currentMesh.transform;
            }

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
        else
        {
            controllerSphere.transform.position = Vector3.zero;
        }

        if (grabInteractorL.HasSelectedInteractable)
        {
            deleteInteractable.gameObject.SetActive(true);
        }
        else
        {
            deleteInteractable.gameObject.SetActive(false);
        }
    }

    public void ToggleButtons()
    {
        buttonsVisible = !buttonsVisible;
        LLockButton.gameObject.SetActive(buttonsVisible);
        RLockButton.gameObject.SetActive(buttonsVisible);
        createInteractable.gameObject.SetActive(buttonsVisible);
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
        if (wireframe != null)
        {
            Destroy(wireframe);
        }
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

    public void SwitchHands()
    {
        righHanded = !righHanded;
        OVRHand aux = leftHand;
        leftHand = rightHand;
        rightHand = aux;

        HandGrabInteractor aux2 = grabInteractorL;
        grabInteractorL = grabInteractorR;
        grabInteractorR = aux2;

        createInteractable.transform.parent = leftHand.transform;
        deleteInteractable.transform.parent = leftHand.transform;
        // createInteractable.transform.localPosition = Vector3.one * 0.04f;


        if (righHanded)
        {
            createInteractable.transform.localPosition = new Vector3(15, 10, 40) * 0.001f;
            deleteInteractable.transform.localPosition = new Vector3(15, 10, -40) * 0.001f;
            createInteractable.transform.rotation = leftHand.GetComponent<OVRSkeleton>().Bones[0].Transform.rotation * Quaternion.Euler(90, 0, 90);
            deleteInteractable.transform.rotation = leftHand.GetComponent<OVRSkeleton>().Bones[0].Transform.rotation * Quaternion.Euler(90, 0, 90);
        }
        else
        {
            createInteractable.transform.localPosition = new Vector3(15, -10, -40) * 0.001f;
            deleteInteractable.transform.localPosition = new Vector3(15, -10, 40) * 0.001f;
            createInteractable.transform.rotation = leftHand.GetComponent<OVRSkeleton>().Bones[0].Transform.rotation * Quaternion.Euler(-90, 0, -90);
            deleteInteractable.transform.rotation = leftHand.GetComponent<OVRSkeleton>().Bones[0].Transform.rotation * Quaternion.Euler(-90, 0, -90);
        }

        debugText.text = "Hands swapped!";
    }

    public void ToggleLockButton(bool status, bool rightHand, GameObject obj)
    {
        if (buttonsVisible && status)
        {
            if (rightHand)
            {
                RLockButton.gameObject.SetActive(true);
            }
            else
            {
                LLockButton.gameObject.SetActive(true);
            }
            objectToLock = obj;
        }
        else
        {
            RLockButton.gameObject.SetActive(false);
            LLockButton.gameObject.SetActive(false);
        }
    }

    public void LockObject()
    {
        if (objectToLock != null)
        {
            objectToLock.GetComponentInChildren<HandGrabInteractable>().enabled = !objectToLock.GetComponentInChildren<HandGrabInteractable>().enabled;
        }
    }
}
