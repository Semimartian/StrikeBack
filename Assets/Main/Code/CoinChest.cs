using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinChest : MonoBehaviour, IHittable
{

    public void Hit(Vector3 hitPosition, Vector3 hitForce)
    {
        Break();
    }

    private void Break()
    {
        Vector3 position = transform.position;
        EffectsManager.PlayEffectAt(EffectNames.Coins, position);
        SoundManager.PlayOneShotSoundAt(SoundNames.BoxBreaking,position);
        SoundManager.PlayOneShotSoundAt(SoundNames.Coins, position);

        Destroy(gameObject);
    }
}
