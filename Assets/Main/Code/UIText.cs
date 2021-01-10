using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIText : MonoBehaviour
{
   [SerializeField] private TMPro.TextMeshProUGUI textMesh;
    public void UpdateText(string text)
    {
        textMesh.text = text;
    }
}
