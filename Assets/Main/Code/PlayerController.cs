using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour,IHittable
{
    private Collider[] overlappingColliders;
   // [SerializeField] private CapsuleCollider capsuleCollider;
    private Vector3 deflectCapsuleHalfHeight;
    private float deflectCapsuleRadius;
   [SerializeField]  private Animator animator;

    private Transform myTransform;
    private Rigidbody rigidbody;
    private bool isRunning = false;
    [SerializeField] private float maxSpeed;
    private float currentSpeed;
    [SerializeField] private float accelerationPerSecond;
    [SerializeField] private float deaccelerationPerSecond;

    [SerializeField] private Transform mouseRayMarker;
    [SerializeField] private Camera camera;

    //[SerializeField] private float groundY;

    //private int hits = 0;
    private int hitPoints;
    [SerializeField] private HitPointsUI hitPointsUI;
   // [SerializeField] private UIText hitsText;
    [Header("Auto Aim")]
    [SerializeField] private bool enableAutoAim;
    [SerializeField] private float autoAimRadius;
    private Collider[] autoAimCollidersInRange;
    [SerializeField] private Renderer[] blinkers;
    [SerializeField] private int blinksPerHit;
    [SerializeField] private float blinkInterval;
    private bool isBlinking = false;
    [Header("Force Explosion")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;
    [SerializeField] float explosionUpwardModifier;
    private void Awake()
    {
        myTransform = transform;
        rigidbody = GetComponent<Rigidbody>();
        overlappingColliders = new Collider[64];
        autoAimCollidersInRange = new Collider[64];
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        deflectCapsuleHalfHeight = Vector3.up * (capsuleCollider.height / 2);
        deflectCapsuleRadius = capsuleCollider.radius;
        capsuleCollider.enabled = false;
    }

    private void Start()
    {
        hitPoints = 3;
        UpdateHitPointsUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isRunning)
        {
            TryDeflect();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            StartRunning();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            StopRunning();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SoundManager.PlayOneShotSoundAt(SoundNames.Explosion, myTransform.position);

        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SoundManager.PlayOneShotSoundAt(SoundNames.OldExplosion, myTransform.position);
        }
    }

    public void StartRunning()
    {
        isRunning = true;
        animator.SetBool("IsRunning", true);
        rigidbody.rotation = Quaternion.identity;
       // cameraController.TransitionTo(CameraStates.Running);
    }

    public void StopRunning()
    {
        isRunning = false;
        animator.SetBool("IsRunning", false);
      //  cameraController.TransitionTo(CameraStates.Action);
    }

    public void Dance()
    {
        animator.SetTrigger("Dance");
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        if (isRunning)
        {
            currentSpeed += accelerationPerSecond * deltaTime;
        }
        else
        {
            currentSpeed -= deaccelerationPerSecond * deltaTime;
        }

        if (currentSpeed < 0 )
        {
            currentSpeed = 0;
        }
        else if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }

        if (currentSpeed != 0)
        {
            Vector3 newPosition = rigidbody.position +
                 ((currentSpeed * deltaTime) * myTransform.forward);
            rigidbody.MovePosition(newPosition);
        }
    }

    private void TryDeflect()
    {
        Vector3 myPosition = myTransform.position;

        Vector3? nullableLookAtPosition =
            (enableAutoAim ? AutoAimMouseToGroundPlane(Input.mousePosition) : MouseToGroundPlane(Input.mousePosition));

        Vector3 lookAtPosition = new Vector3();

        if (nullableLookAtPosition != null)
        {
            lookAtPosition = (Vector3)nullableLookAtPosition;
        }
       Vector3 direction = //projectile.transform.position - myTransform.position;
               lookAtPosition - myPosition; 
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rigidbody.rotation = rotation;

        string trigger = "Deflect";
        trigger += Random.Range(0, animator.GetInteger("Deflects")).ToString();
        animator.SetTrigger(trigger);
        SoundManager.PlayOneShotSoundAt(SoundNames.LightSaberSwing, myPosition);

        int colliderCount = OverlapDeflectCapsule(ref myPosition);
        for (int i = 0; i < colliderCount; i++)
        {
            Projectile projectile = overlappingColliders[i].GetComponentInParent<Projectile>();
            if (projectile != null)
            {
                DeflectProjectileTo(projectile, lookAtPosition);
            }
        }
    }

    private int OverlapDeflectCapsule(ref Vector3 myPosition)
    {
        Vector3 top = myPosition + deflectCapsuleHalfHeight;
        Vector3 bottom = myPosition - deflectCapsuleHalfHeight;

        int colliderCount =
            Physics.OverlapCapsuleNonAlloc(bottom, top, deflectCapsuleRadius, overlappingColliders);
        if (colliderCount >= overlappingColliders.Length)
        {
            Debug.LogError("NOT GOOD");
        }
        return colliderCount;
    }

    private void DeflectProjectileTo(Projectile projectile, Vector3 direction)
    {
        #region LEARN FROM THIS:
        //LEARN FROM THIS:
        // rigidbody.rotation =
        // myTransform.LookAt(projectile.transform.position);

        /*  Vector3 myYlessPosition = myTransform.position;
          Vector3 projectileYlessPosition = projectile.transform.position;
          myYlessPosition.y = 0;
          projectileYlessPosition.y = 0;

          Quaternion rotation = Quaternion.LookRotation(projectileYlessPosition - myYlessPosition);*/
        #endregion
        projectile.Deflect(direction);
        Vector3 deflectionPosition = projectile.transform.position;
        EffectsManager.PlayEffectAt(EffectNames.Deflection, deflectionPosition);
        SoundManager.PlayOneShotSoundAt(SoundNames.Deflect, deflectionPosition);
    }

    internal void ForceLift(Vector3 lookAtPosition)
    {
        Vector3 direction = //projectile.transform.position - myTransform.position;
        lookAtPosition - myTransform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rigidbody.rotation = rotation;

        animator.SetTrigger("ForceLift");
    }

    public void FRENZY()
    {

        animator.SetBool("FRENZY",true);
        StartCoroutine(AutoDeflectRoutine());
    }

    public void EndFrenzy()
    {

        animator.SetBool("FRENZY", false);
        StopCoroutine(AutoDeflectRoutine());
    }

    private IEnumerator AutoDeflectRoutine()
    {
        StickManEnemy[] waveEnemies = GameManager.GetCurrentWaveEnemies();
        while (true)
        {
            Vector3 myPosition = myTransform.position;

            int colliderCount = OverlapDeflectCapsule(ref myPosition);

            for (int i = 0; i < colliderCount; i++)
            {
                Projectile projectile = overlappingColliders[i].GetComponentInParent<Projectile>();
                if (projectile != null)
                {

                    Vector3 deflectTo = myPosition + (Vector3.forward * 10);// why does it not work withoyt a multiplier..?
                    float shortestDistance = float.MaxValue;
                    for (int j = 0; j < waveEnemies.Length; j++)
                    {
                        StickManEnemy enemy = waveEnemies[j];
                        if (enemy.IsAlive)
                        {
                            float squareDistance = 
                                Vector3.SqrMagnitude(enemy.transform.position - myPosition);
                            if(squareDistance < shortestDistance)
                            {
                                deflectTo = enemy.transform.position;
                                shortestDistance = squareDistance;
                            }
                        }
                    }

                    /*
                    if (deflectTo == null)
                    {
                        Debug.Log("deflectTo == null: " + deflectTo == null);//WHAT THE FUCK???

                        deflectTo = myPosition + Vector3.forward;//Cheap...
                    }
                    else
                    {
                        Debug.Log("deflectTo == null: " + deflectTo == null);
                        Debug.Log("deflectTo: " + (Vector3)deflectTo  );

                    }*/

                    //Debug.Log("deflectTo: " + (Vector3)deflectTo);

                    DeflectProjectileTo(projectile, deflectTo);
                }
            }
            yield return new WaitForSeconds(0.05f);
        }      
    }

    private void UpdateHitPointsUI()
    {
        hitPointsUI.UpdateUI(hitPoints);
       // hitsText.UpdateText("HITS: " + hits.ToString());
    }

    private Vector3 MouseToGroundPlane(Vector3 mousePosition)
    {
        //TODO: Something dumb is going on...
        Ray ray = camera.ScreenPointToRay(mousePosition);
        RaycastHit raycastHit;
        float groundY = 0;
        if (Physics.Raycast(ray, out raycastHit))
        {
            groundY = raycastHit.point.y;
        }

        float rayLength = (ray.origin.y - groundY) / ray.direction.y;

        Debug.DrawLine(ray.origin, ray.origin - (ray.direction * rayLength), Color.red);

        Vector3 results = ray.origin - (ray.direction * rayLength);
        mouseRayMarker.position = results;
        return results;
    }

    private Vector3? AutoAimMouseToGroundPlane(Vector3 mousePosition)
    {
        Ray ray = camera.ScreenPointToRay(mousePosition);
        RaycastHit raycastHit;
        Physics.Raycast(ray, out raycastHit);
        IHittable hittable = raycastHit.collider.gameObject.GetComponentInParent<IHittable>();
        if(hittable != null)
        {
            mouseRayMarker.position = raycastHit.point;
            return raycastHit.point;
        }
        else
        {
            Vector3 autoAimPoint = raycastHit.point;
            mouseRayMarker.position = autoAimPoint;

            int length = Physics.OverlapSphereNonAlloc(autoAimPoint, autoAimRadius, autoAimCollidersInRange);
            if (length >= autoAimCollidersInRange.Length)
            {
                Debug.LogError("NOT GOOD");
            }
            float shortestDistance = float.MaxValue;
            Vector3 closestPoint = new Vector3();
            for (int i = 0; i < length; i++)
            {
                if(autoAimCollidersInRange[i].GetComponentInParent<IHittable>() != null)
                {
                    //Debug.Log("Hittable found");
                    Vector3 hittablePosition = autoAimCollidersInRange[i].transform.position;
                    float squareDistance = Vector3.SqrMagnitude( hittablePosition - autoAimPoint);
                    Debug.Log(autoAimCollidersInRange[i].name + " Distance: "+ squareDistance.ToString("f2"));

                    if (squareDistance < shortestDistance)
                    {
                        shortestDistance = squareDistance;
                        closestPoint = hittablePosition;
                    }
                }
               
            }
            return closestPoint;
        }
       /* Debug.Log("results: " + results);
        Debug.Log("point: " + raycastHit.point);*/
        return null;
    }


    private bool IsHittable
    {
        get { return (!isRunning && (hitPoints > 0) && !isBlinking); }
    }

    public void Hit(Vector3 hitPosition, Vector3 hitForce)
    {
        Debug.Log("I'M HIT!");

        if (IsHittable)
        {
            hitPoints -= 1;
            UpdateHitPointsUI();
            if(hitPoints == 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(Blink());
            }
        }

        /* hits += 1;
         UpdateUI();*/
    }

    private void Die()
    {
        animator.SetTrigger("DEATH");
        GameManager.OnPlayerDeath();
    }

    private IEnumerator Blink()
    {
        isBlinking = true;
        for (int i = 0; i < blinksPerHit; i++)
        {
            for (int j = 0; j < blinkers.Length; j++)
            {
                blinkers[j].enabled = false;
            }
            yield return new WaitForSeconds(blinkInterval);
            for (int j = 0; j < blinkers.Length; j++)
            {
                blinkers[j].enabled = true;
            }
            yield return new WaitForSeconds(blinkInterval);
        }
        isBlinking = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void ForceExplosion()
    {
        Vector3 explosionPosition = myTransform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        List<IExplodable> explodables = new List<IExplodable>();
        for (int i = 0; i < colliders.Length; i++)
        {
            IExplodable explodable = colliders[i].gameObject.GetComponentInParent<IExplodable>();

            if (explodable != null)
            {
                if (!explodables.Contains(explodable))//TODO: Optimise...
                {
                    explodables.Add(explodable);
                }
            }
        }

        for (int i = 0; i < explodables.Count; i++)
        {
            explodables[i].Explode(explosionPosition, explosionForce, explosionRadius, explosionUpwardModifier);
        }

    }
}
