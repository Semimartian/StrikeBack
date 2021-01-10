using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler : MonoBehaviour
{
    [SerializeField] private Rigidbody rootRigidBody;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody[] bonesRigidBodies;
    [SerializeField] private Collider[] bonesColliders;

    // Start is called before the first frame update
    void Start()
    {
        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        animator.enabled = false;
        for (int i = 0; i < bonesRigidBodies.Length; i++)
        {
            bonesRigidBodies[i].isKinematic = false;
        }
        for (int i = 0; i < bonesColliders.Length; i++)
        {
            bonesColliders[i].enabled = true;
        }
    }

    public void DisableRagdoll()
    {
        animator.enabled = true;
        for (int i = 0; i < bonesRigidBodies.Length; i++)
        {
            bonesRigidBodies[i].isKinematic = true;
        }
        for (int i = 0; i < bonesColliders.Length; i++)
        {
            bonesColliders[i].enabled = false;
        }
    }

    public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float explosionUpwardModifier)
    {
        rootRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, explosionUpwardModifier);
    }
   
    public void AddForceAt(Vector3 force, Vector3 position)
    {
        rootRigidBody.AddForceAtPosition(force, position,ForceMode.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EnableRagdoll();
        }
    }
}
