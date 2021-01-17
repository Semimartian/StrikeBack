using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAdvancer : MonoBehaviour
{
     private BoxCollider collider;
    // Start is called before the first frame update
    private void Start()
    {
        Destroy( GetComponent<MeshRenderer>());
        collider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if (collider.bounds.Contains(GameManager.playerPosition))
        {
            GameManager.StartNextWave();
            this.gameObject.SetActive(false);
        }   
    }
}
