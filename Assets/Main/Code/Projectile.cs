using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speedAtSpawn;
    [SerializeField] private float speedAtDeflect;
    [SerializeField] private float hitForce;

    private Rigidbody rigidbody;
    private Transform myTransform;
    private float lifeTime = 0;
    private bool deflected = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        myTransform = transform;
        lifeTime = 0;
    }

    private void FixedUpdate()
    {

        /*Vector3 velocity = myTransform.forward * speed * Time.fixedDeltaTime;
        rigidbody.velocity = velocity;*/
        Vector3 movement =
            myTransform.forward * (deflected ? speedAtDeflect : speedAtSpawn) * Time.fixedDeltaTime;
        rigidbody.MovePosition(rigidbody.position + movement);
        lifeTime += Time.fixedDeltaTime;
        if (lifeTime > 3f)
        {
            Destroy(gameObject);
        }
    }

    private void Hit()
    {
        EffectsManager.PlayEffectAt(EffectNames.BlastExplosion, transform.position);
        Destroy(gameObject);
    }

    internal void Deflect(Vector3 lookAtPosition)
    {
        if (!deflected)
        {
            //This method is dumb
            /*Vector3 lookAtPosition = (-myTransform.forward) + myTransform.position;
            myTransform.LookAt(lookAtPosition);*/
            Vector3 myPosition = myTransform.position;
            Vector3 direction = lookAtPosition - myPosition;
            if (myPosition.y > lookAtPosition.y)
            {
                direction.y = 0;
            }
            Quaternion rotation = Quaternion.LookRotation(direction);
            rigidbody.rotation = rotation;

            lifeTime = 0;
            deflected = true;
        }      
    }


    private void OnTriggerEnter(Collider other)
    {

        if(lifeTime > 0.1f)//Cheap
        {
           // bool hitSomething = false;
            IHittable hittable = other.gameObject.GetComponentInParent<IHittable>();

            if (hittable != null)
            {
                hittable.Hit(rigidbody.position, myTransform.forward * hitForce);
                //hitSomething = true;
            }
           /* else if (other.gameObject.layer == 0)//Hmmmmmmmm
            {
                hitSomething = true;
            }*/

            //if (hitSomething)
            {
                Hit();
            }
        }
        
    }
}
