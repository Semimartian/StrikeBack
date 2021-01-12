using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGun : Bomb
{
    [SerializeField] private Material mat;
    /*[SerializeField] private Color colourAtFullHealth;
    [SerializeField] private Color colourAtOneHealth;*/
    [SerializeField] private Color emmisionColour;
    [SerializeField] private float emmisionMultiplier;

    private byte healthAtStart;
    [SerializeField] private string emmisionColourName;
    
    protected override void DoOnStart()
    {
        base.DoOnStart();
        healthAtStart = hp;
        UpdateGraphics();
    }
    public override void Hit(Vector3 hitPosition, Vector3 hitForce)
    {
        base.Hit(hitPosition, hitForce);
        UpdateGraphics();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Hit(new Vector3(), new Vector3());
        }
    }

    private void UpdateGraphics()
    {
        float t = (Mathf.Abs(((float)(hp - 1) / (float)(healthAtStart - 1)) - 1))
            * emmisionMultiplier;
        //Debug.Log("t = " + t.ToString("f3"));
       /* Color myColour =
            Color.Lerp(colourAtOneHealth, colourAtFullHealth, t);*/
       // mat.color = myColour;

        Vector4 colourVector = new Vector4
            (emmisionColour.r, emmisionColour.g, emmisionColour.b, 0);
        mat.SetColor (emmisionColourName, colourVector * t);

        //mat.SetColor(colourName, myColour);

    }
}
