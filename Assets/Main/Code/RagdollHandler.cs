﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler : MonoBehaviour
{
    [SerializeField] private Rigidbody rootRigidBody;
    [SerializeField] private Animator animator;
    /*[SerializeField] private Rigidbody[] bonesRigidBodies;
    [SerializeField] private Collider[] bonesColliders;*/
    [SerializeField] private GameObject[] bones;
    [SerializeField] private PhysicMaterial physicMaterial;
    [SerializeField] private float drag;
    [SerializeField] private float angularDrag;

    void Start()
    {
        InitialiseRagdoll();
        DisableRagdoll();
    }


    private void InitialiseRagdoll()
    {
        for (int i = 0; i < bones.Length; i++)
        {

            Collider[] boneColliders = bones[i].GetComponents<Collider>();
            for (int j = 0; j < boneColliders.Length; j++)
            {
                Collider collider = boneColliders[j];
                collider.material = physicMaterial;
            }
            Rigidbody rigidbody = bones[i].GetComponent<Rigidbody>();
            rigidbody.drag = drag;
            rigidbody.angularDrag = angularDrag;
        }
    }

    public void EnableRagdoll()
    {
        animator.enabled = false;
        for (int i = 0; i < bones.Length; i++)
        {
            GameObject bone = bones[i];
            bone.layer = LayerMask.NameToLayer("IgnoreProjectiles");
            Collider[] boneColliders = bone.GetComponents<Collider>();
            for (int j = 0; j < boneColliders.Length; j++)
            {
                Collider collider = boneColliders[j];
                collider.enabled = true;
            }
            Rigidbody rigidbody = bone.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;

        }
        /*  for (int i = 0; i < bonesRigidBodies.Length; i++)
        {
            bonesRigidBodies[i].isKinematic = false;
        }
        for (int i = 0; i < bonesColliders.Length; i++)
        {
            bonesColliders[i].enabled = true;
        }*/
    }

    public void DisableRagdoll()
    {
        animator.enabled = true;
        for (int i = 0; i < bones.Length; i++)
        {
            Collider[] boneColliders = bones[i].GetComponents<Collider>();
            for (int j = 0; j < boneColliders.Length; j++)
            {
                Collider collider = boneColliders[j];
                collider.enabled = false;
            }
            bones[i].GetComponent<Rigidbody>().isKinematic = true;
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
