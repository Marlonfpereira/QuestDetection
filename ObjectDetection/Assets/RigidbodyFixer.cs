using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyFixer : MonoBehaviour
{
    private Transform parentTransform;
    private CapsuleCollider capsuleCollider;

    // Start is called before the first frame update
    void Start()
    {
        // Get the parent transform
        parentTransform = transform.parent;

        // Get the CapsuleCollider component attached to this object
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Set the direction to X-Axis
        capsuleCollider.direction = 0; // 0 corresponds to X-axis
    }

    // Update is called once per frame
    void Update()
    {
        if (parentTransform != null && capsuleCollider != null)
        {
            if (parentTransform.localScale.y > parentTransform.localScale.x)
            {
                capsuleCollider.direction = 1; // 1 corresponds to Y-axis
                float newRadius = parentTransform.localScale.x / 2.1f;
                float newHeight = parentTransform.localScale.y;
                capsuleCollider.radius = newRadius;
                capsuleCollider.height = newHeight;
            }
            else
            {
                capsuleCollider.direction = 0; // 0 corresponds to X-axis
                float newRadius = parentTransform.localScale.y / 2.1f;
                float newHeight = parentTransform.localScale.x;
                capsuleCollider.radius = newRadius;
                capsuleCollider.height = newHeight;
            }
                
                

        }
    }
}
