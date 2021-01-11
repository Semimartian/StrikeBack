using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappearer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Disappear();
    }

    private void Disappear()
    {
        List<Component> components = new List<Component>();
        /*Renderer[] renderers = */
        components.AddRange( GetComponentsInChildren<Renderer>());
        components.AddRange(GetComponentsInChildren<MeshFilter>());
        for (int i = 0; i < components.Count; i++)
        {
            Destroy(components[i]);
        }
        Destroy(this);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
