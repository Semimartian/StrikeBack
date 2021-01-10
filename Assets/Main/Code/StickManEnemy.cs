using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickManEnemy : MonoBehaviour, IHittable,IExplodable
{
    [SerializeField] protected Animator animator;
    protected bool isAwake = false;
    protected bool isAlive =true;
    public bool IsAlive
    {
        get { return isAlive; }
    }
    [SerializeField] protected bool lookAtPlayer;
    protected Transform myTransform;
    [SerializeField] protected Collider collider;
    [SerializeField] private RagdollHandler ragdollHandler;
    [SerializeField] private Rigidbody[] heldItems;

    //private sbyte hp = 4;

    private void Start()
    {
        DoOnStart();
    }

    protected virtual void DoOnStart()
    {
        ragdollHandler.DisableRagdoll();
        myTransform = transform;
        Invoke("PlayIdleAnimation", Random.Range(0, 1.5f));
    } 

    private void PlayIdleAnimation()
    {
        //Debug.Log("PlayIdleAnimation");
        animator.SetTrigger("Idle");
    }

    public  void Hit(Vector3 hitPosition, Vector3 hitForce)
    {
        if (isAlive)
        {
           /* hp -= 1;
            if(hp <= 0)*/
            {
                Die();
                ragdollHandler.EnableRagdoll();
                ragdollHandler.AddForceAt(hitForce, hitPosition);

                for (int i = 0; i < heldItems.Length; i++)
                {
                    Rigidbody item = heldItems[i];
                    item.transform.SetParent(null);
                    item.isKinematic = false;
                    Collider[] colliders = item.GetComponentsInChildren<Collider>();
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        colliders[j].enabled = true;
                    }
                    item.AddForceAtPosition(hitForce, hitPosition, ForceMode.Impulse);
                }

            }
        }
    }

    public void Explode(Vector3 explosionPosition, float explosionForce, float explosionRadius, float explosionUpwardModifier)
    {
        Debug.Log("Explode()");

        if (isAlive)
        {
            Die();
            ragdollHandler.EnableRagdoll();
            for (int i = 0; i < heldItems.Length; i++)
            {
                Rigidbody item = heldItems[i];
                item.transform.SetParent(null);
                item.isKinematic = false;
                item.AddExplosionForce
                    (explosionForce, explosionPosition, explosionRadius, explosionUpwardModifier);
            }
        }

        ragdollHandler.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, explosionUpwardModifier);

    }

    private void Die()
    {
        isAlive = false;
        //animator.SetTrigger("Die");
        collider.enabled = false;
        SoundNames soundName = (Random.Range(0, 4) > 0) ? SoundNames.Scream : SoundNames.Wilhelm;
       // Debug.Log(soundName.ToString());
        SoundManager.PlayOneShotSoundAt(soundName, myTransform.position);

        GameManager.CheckWaveState();
    }

    public virtual void Awaken()
    {
        isAwake = true;
    }

    public void StartDancing()
    {
        animator.SetTrigger("Dance");
        isAwake = false;
    }
}
