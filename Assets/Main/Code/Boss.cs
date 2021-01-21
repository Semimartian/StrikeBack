using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IExplodable
{
    //TODO: this class looks too much like Shooter

    [SerializeField] private Animator myAnimator;
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Projectile projectilePreFab;
    [SerializeField] private Transform barrelPoint;
    [SerializeField] private Transform gun;
    [SerializeField] private float shootInterval;

    private bool isAlive = true;

    private Transform myTransform;
    [SerializeField] private Collider collider;
    [SerializeField] private RagdollHandler ragdollHandler;
    private bool isShooting = false;

    public void WakeUp()
    {
        myAnimator.SetBool("IsShooting", true);
        gunAnimator.SetBool("IsShooting", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            WakeUp();
        }
    }

    public void StartShooting()
    {
        if (!isShooting)
        {
            isShooting = true;
            ReleaseProjectile();
        }
    }

    public void Hit(Vector3 hitPosition, Vector3 hitForce)
    {
        /*if (isAlive)
        {
             hp -= 1;
             if(hp <= 0)
            {
                Die();
                ragdollHandler.EnableRagdoll();
                ragdollHandler.AddForceAt(hitForce, hitPosition);
                gun.isKinematic = false;
                gun.AddForceAtPosition(hitForce, hitPosition, ForceMode.Impulse);
            }
        }*/
    }

    private void ReleaseProjectile()
    {
        if (!isAlive)
        {
            return;
        }
        //Debug.Log("Shoot");
        Projectile projectile = Instantiate(projectilePreFab);
        Vector3 projectilePosition = barrelPoint.transform.position;
        projectile.transform.position = projectilePosition;
        projectile.transform.forward = -Vector3.forward;
        SoundManager.PlayOneShotSoundAt(SoundNames.BlasterShot, projectilePosition);

        //if (repeat)
        {
            Invoke("ReleaseProjectile", shootInterval);
        }
    }

    private void Start()
    {
        ragdollHandler.DisableRagdoll();
        myTransform = transform;
    }

    public void Explode(Vector3 explosionPosition, float explosionForce, float explosionRadius, float explosionUpwardModifier)
    {
        Debug.Log("Explode()");
        if (isAlive)
        {
            Die();
            ragdollHandler.EnableRagdoll();
        }
        ragdollHandler.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, explosionUpwardModifier);
    }

    private void Die()
    {
        isAlive = false;
        //animator.SetTrigger("Die");
        collider.enabled = false;
        //SoundManager.PlayOneShotSoundAt(soundName, myTransform.position);
      //  GameManager.CheckWaveState();
        GameManager.OnBossDeath();

    }
}
