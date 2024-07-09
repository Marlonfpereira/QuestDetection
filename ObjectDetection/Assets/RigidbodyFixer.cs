using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyFixer : MonoBehaviour
{
    private Transform parentTransform;
    private SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        // Get the parent transform
        parentTransform = transform.parent;

        // Get the SphereCollider component attached to this object
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (parentTransform != null && sphereCollider != null)
        {
            // Calculate the new radius based on the parent's smaller scale in X and Y
            if (parentTransform.localScale.x < parentTransform.localScale.y)
            {
                sphereCollider.radius = parentTransform.localScale.x / 2.5f;
            }
            else
            {
                sphereCollider.radius = parentTransform.localScale.y / 2.5f;
            }
        }
    }
}
