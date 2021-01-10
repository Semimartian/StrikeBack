using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : StickManEnemy, IHittable,IExplodable
{
    [SerializeField] private Projectile projectilePreFab;
    [SerializeField] private Transform gunAnchor;
    [SerializeField] private Rigidbody gunRigidbody;
    [SerializeField] private Transform barrelPoint;
    private bool isShooting = false;
    [SerializeField] private float minShootInterval;
    [SerializeField] private float maxShootInterval;

    //private sbyte hp = 4;
    //[SerializeField] private float shootingDistanceFromPlayer;
    //[SerializeField] private Transform[] heldItems;

    protected override void DoOnStart()
    {
        base.DoOnStart();

        if (GameManager.allowAutomaticShooting)
        {
            Invoke("ShootRoutine", Random.Range(1f, 4f));
        }

        gunRigidbody.transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        if (lookAtPlayer && isAlive)
        {
            gunRigidbody.position = gunAnchor.position;

            Vector3 playerPosition = GameManager.playerPosition;
            Vector3 gunDirection = playerPosition - gunRigidbody.position;
            gunRigidbody.rotation = Quaternion.LookRotation(gunDirection);

            Vector3 myDirection = playerPosition - myTransform.position;
            myDirection.y = 0;
            myTransform.rotation = Quaternion.LookRotation(myDirection);
        }   
    }

    public void TryShoot()
    {
        if (!isShooting)
        {
            animator.SetTrigger("Shoot");
            isShooting = true;
        }
        else
        {
            Debug.Log("I am shooting");
        }
    }

    public void ReleaseProjectile()
    {
        Projectile projectile = Instantiate(projectilePreFab);
        Vector3 projectilePosition = barrelPoint.transform.position;
        projectile.transform.position = projectilePosition;
        projectile.transform.rotation = gunRigidbody.rotation;
        SoundManager.PlayOneShotSoundAt(SoundNames.BlasterShot, projectilePosition);
        isShooting = false;
    }

    private void ShootRoutine()
    {
        if (isAwake)
        {
            /*float distanceFromPlayer = Vector3.Distance(myTransform.position, GameManager.playerPosition);
            if (distanceFromPlayer < shootingDistanceFromPlayer)*/
            {
                TryShoot();
            }
        }

        Invoke("ShootRoutine", Random.Range(minShootInterval, maxShootInterval));
    } 

}
