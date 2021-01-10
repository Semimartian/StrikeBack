using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    void Hit(Vector3 hitPosition, Vector3 hitForce);
}

public interface IExplodable
{
    void Explode(Vector3 explosionPosition, float explosionForce, float explosionRadius, float explosionUpwardModifier);
}