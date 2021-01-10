using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAdvancer : MonoBehaviour
{
     private BoxCollider collider;
    public bool bossTrigger;
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
            GameManager.StartNextWave(bossTrigger);
            this.gameObject.SetActive(false);
        }   
    }
}
