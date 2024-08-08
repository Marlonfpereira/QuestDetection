using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering.Universal;

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
    public TextMeshPro MenuButtonText;
    public TextMeshPro resizeButtonText;
    public PokeInteractable createInteractable;
    public PokeInteractable deleteInteractable;
    public PokeInteractable LLockButton;
    public PokeInteractable RLockButton;
    public Camera mainCamera;
    public LayerMask raycastLayer;
    private bool righHanded = true;
    private GameObject objectToLock;

    private GameObject scaleObj;
    void Start()
    {
        currentMesh = new GameObject();
        scaleObj = new GameObject();
    }

    private bool isLongPinch = false;
    private float pinchTimer = 0f;
    private float pinchDuration = .5f;
    private GameObject wireframe;
    private bool buttonsVisible = true;
    private bool resizable = true;


    private Vector3 initialDistanceBetweenHands;
    private Vector3 initialScale;

    void Update()
    {
        Vector3 controllerPos = rightHand.GetComponent<OVRSkeleton>().Bones[8].Transform.position;

        if (isCreating)
        {
            controllerSphere.transform.position = controllerPos;

            if (currentSet.Count >= 2)
            {
                DebugMesh();
                if (wireframe != null)
                {
                    Destroy(wireframe);
                }
                wireframe = new GameObject();
                LineRenderer lineRenderer = wireframe.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                if (debugMesh.faceCount > 0)
                {
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.green };
                } else
                {
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
                }
                lineRenderer.positionCount = currentSet.Count + 1;
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
                    if (isLongPinch)
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
                        lineRenderer.positionCount = 5;
                        lineRenderer.SetPosition(0, currentSet[0].transform.position);
                        lineRenderer.SetPosition(1, new Vector3(currentSet[0].transform.position.x, controllerSphere.transform.position.y, currentSet[0].transform.position.z));
                        lineRenderer.SetPosition(2, controllerSphere.transform.position);
                        lineRenderer.SetPosition(3, new Vector3(controllerSphere.transform.position.x, currentSet[0].transform.position.y, controllerSphere.transform.position.z));
                        lineRenderer.SetPosition(4, currentSet[0].transform.position);
                        wireframe.transform.parent = currentMesh.transform;
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

            if (resizable && grabInteractorR.HasSelectedInteractable && grabInteractorL.SelectedInteractable == grabInteractorR.SelectedInteractable)
            {
                grabInteractorL.SelectedInteractable.transform.parent.gameObject.transform.parent = scaleObj.transform;

                if (initialDistanceBetweenHands == Vector3.zero)
                {
                    initialDistanceBetweenHands = rightHand.transform.position - leftHand.transform.position;
                    initialScale = scaleObj.transform.localScale;
                }
                else
                {
                    Vector3 currentDistanceBetweenHands = rightHand.transform.position - leftHand.transform.position;
                    float distanceRatio = currentDistanceBetweenHands.magnitude / initialDistanceBetweenHands.magnitude;
                    scaleObj.transform.localScale = initialScale * distanceRatio;
                }
            }
            else
            {
                initialDistanceBetweenHands = Vector3.zero;
            }
        }
        else
        {
            deleteInteractable.gameObject.SetActive(false);
            scaleObj = new GameObject();
        }

        if (buttonsVisible)
        {
            Vector3 directionToUser = Camera.main.transform.position - createInteractable.gameObject.transform.position;
            Quaternion rotationToUser = Quaternion.LookRotation(directionToUser);
            bool isFacingUser = Quaternion.Angle(createInteractable.gameObject.transform.rotation, rotationToUser) > 120f;

            if (!isFacingUser)
            {
                createInteractable.gameObject.SetActive(false);
                deleteInteractable.gameObject.SetActive(false);
                LLockButton.gameObject.SetActive(false);
            }
            else
            {
                createInteractable.gameObject.SetActive(true);
            }


            // Vector3 directionToUserLLockButton = Camera.main.transform.position - LLockButton.gameObject.transform.position;
            // Quaternion rotationToUserLLockButton = Quaternion.LookRotation(directionToUserLLockButton);
            // bool isFacingUserLLockButton = Quaternion.Angle(LLockButton.gameObject.transform.rotation, rotationToUserLLockButton) > 120f;

            // if (!isFacingUserLLockButton)
            // {
            // }

            Vector3 directionToUserRLockButton = Camera.main.transform.position - RLockButton.gameObject.transform.position;
            Quaternion rotationToUserRLockButton = Quaternion.LookRotation(directionToUserRLockButton);
            bool isFacingUserRLockButton = Quaternion.Angle(RLockButton.gameObject.transform.rotation, rotationToUserRLockButton) > 120f;

            if (!isFacingUserRLockButton)
            {
                RLockButton.gameObject.SetActive(false);
            }

        }
    }

    public void ToggleButtons()
    {
        buttonsVisible = !buttonsVisible;

        if (buttonsVisible)
        {
            MenuButtonText.text = "Deactivate Wrist Menu";
        }
        else
        {
            MenuButtonText.text = "Activate Wrist Menu";
        }

        LLockButton.gameObject.SetActive(buttonsVisible);
        RLockButton.gameObject.SetActive(buttonsVisible);
        createInteractable.gameObject.SetActive(buttonsVisible);
    }

    public void ToggleCreate()
    {
        if (!isCreating)
        {
            isCreating = true;
            buttonText.text = "Finish";
        }
        else
        {
            isCreating = false;
            buttonText.text = "Create";
            CreateMesh();
            controllerSphere.transform.position = Vector3.zero;
        }
    }

    private GameObject debugMeshObj;
    private ProBuilderMesh debugMesh;
    public Material debugMaterial;

    public void CreateMesh()
    {

        if (debugMeshObj != null)
        {
            Destroy(debugMeshObj);
        }
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

    public void DebugMesh()
    {
        if (debugMeshObj != null)
        {
            Destroy(debugMeshObj);
        }

        Vector3[] vertices = new Vector3[currentSet.Count];
        Vector3 middle = Vector3.zero;
        for (int i = 0; i < currentSet.Count; i++)
            middle += currentSet[i].transform.position;

        middle /= currentSet.Count;
        for (int i = 0; i < currentSet.Count; i++)
            vertices[i] = currentSet[i].transform.position - middle;

        debugMeshObj = Instantiate(meshWrapper, middle, Quaternion.identity);
        debugMeshObj.transform.parent = currentMesh.transform;

        debugMesh = debugMeshObj.AddComponent<ProBuilderMesh>();
        debugMesh.CreateShapeFromPolygon(vertices, 0.05f, false);
        debugMesh.SetMaterial(debugMesh.faces, debugMaterial);
        debugMesh.ToMesh();
        debugMesh.Refresh();
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
    }

    private Oculus.Interaction.InteractableColorVisual.ColorState blueState = new Oculus.Interaction.InteractableColorVisual.ColorState() { Color = new Color(.6f, .7921f, 1) };
    private Oculus.Interaction.InteractableColorVisual.ColorState redState = new Oculus.Interaction.InteractableColorVisual.ColorState() { Color = new Color(1, .5990f, .5990f) };

    public void ToggleLockButton(bool status, bool rightHand, GameObject obj)
    {
        if (buttonsVisible && status)
        {
            if (rightHand)
            {
                RLockButton.GetComponentInChildren<InteractableColorVisual>().InjectOptionalNormalColorState(obj.GetComponentInChildren<HandGrabInteractable>().enabled ? blueState : redState);
                RLockButton.GetComponentInChildren<TextMeshPro>().text = obj.GetComponentInChildren<HandGrabInteractable>().enabled ? "Lock" : "Unlock";
                RLockButton.gameObject.SetActive(true);
            }
            else
            {
                LLockButton.GetComponentInChildren<InteractableColorVisual>().InjectOptionalNormalColorState(obj.GetComponentInChildren<HandGrabInteractable>().enabled ? blueState : redState);
                LLockButton.GetComponentInChildren<TextMeshPro>().text = obj.GetComponentInChildren<HandGrabInteractable>().enabled ? "Lock" : "Unlock";
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
            RLockButton.gameObject.SetActive(false);
            LLockButton.gameObject.SetActive(false);
        }
    }

    public void ToggleResize()
    {
        resizable = !resizable;

        if (resizable)
        {
            resizeButtonText.text = "Deactivate Resizing";
        }
        else
        {
            resizeButtonText.text = "Activate Resizing";
        }
    }
}
