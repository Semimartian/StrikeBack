using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IHittable, IExplodable
{

    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;
    [SerializeField] float explosionUpwardModifier;
    [SerializeField] protected byte hp = 1;
    private Transform myTransform;
    // Start is called before the first frame update
    void Start()
    {
        DoOnStart();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    protected virtual void DoOnStart()
    {
        myTransform = transform;
    }

    private void Explode()
    {
        Debug.Log("Bomb Exploded!");
        Vector3 explosionPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        List<IExplodable> explodables = new List<IExplodable>();
        for (int i = 0; i < colliders.Length; i++)
        {
            IExplodable explodable = colliders[i].gameObject.GetComponentInParent<IExplodable>();

            if (explodable != null && explodable != this)
            {
                if (!explodables.Contains(explodable))//TODO: Optimise...
                {
                    explodables.Add(explodable);
                }
            }
        }

        Debug.Log("explodables:" + explodables.Count);

        for (int i = 0; i < explodables.Count; i++)
        {
            explodables[i].Explode(explosionPosition, explosionForce, explosionRadius, explosionUpwardModifier);
        }
        EffectsManager.PlayEffectAt(EffectNames.Explosion, explosionPosition);
        SoundManager.PlayOneShotSoundAt(SoundNames.Explosion, explosionPosition);
        Destroy(gameObject);

    }

    public void Explode(Vector3 explosionPosition, float explosionForce, float explosionRadius, float explosionUpwardModifier)
    {
        Explode();
    }

    public virtual void Hit(Vector3 hitPosition, Vector3 hitForce)
    {
        hp--;
        if(hp == 0)
        {
            Explode();

        }

    }
}
