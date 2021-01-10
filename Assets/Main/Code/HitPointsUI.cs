using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPointsUI : MonoBehaviour
{
    [SerializeField] private HeartUI[] hearts;

    public void UpdateUI(int hp)
    {
        //Debug.Log("UpdateUI");

        if (hp > hearts.Length)
        {
            Debug.LogWarning("hp > hearts.Length");
        }
        for (int i = 0; i < hearts.Length; i++)
        {

            hearts[i].fillImage.enabled = (i < hp);
        }
    }
}
