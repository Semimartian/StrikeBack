using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MaterialAnimator : MonoBehaviour
{
    [Serializable]
    private class MaterialAnimationBlock
    {
        public Material mat;
        [HideInInspector] public float currentTime;
        public AnimationCurve curve;
        public Color colour;
        public string colourName = "_EmissionColor";
    }

    [SerializeField] private MaterialAnimationBlock[] materialAnimationBlocks;

    void Update()
    {
        float deltaTime = Time.deltaTime;
        Color colour;
        Vector4 colourVector;

        for (int i = 0; i < materialAnimationBlocks.Length; i++)
        {
            //TODO: you can do it with modulu or somethin
            MaterialAnimationBlock block = materialAnimationBlocks[i];
            block.currentTime += deltaTime;
            if(block.currentTime > block.curve.keys[block.curve.length - 1].time)
            {
                block.currentTime = 0;
            }

            colour = block.colour;
            colourVector = new Vector4
                (block.colour.r, block.colour.g, block.colour.b, 0);
            block.mat.SetColor
                (block.colourName, colourVector * block.curve.Evaluate(block.currentTime));
        }
    }

   /* private void UpdateGraphics()
    {
        float t = (Mathf.Abs(((float)(hp - 1) / (float)(healthAtStart - 1)) - 1))
            * emmisionMultiplier;
        // Color myColour = Color.Lerp(colourAtOneHealth, colourAtFullHealth, t);
        // mat.color = myColour;
        Vector4 colourVector = new Vector4
            (emmisionColour.r, emmisionColour.g, emmisionColour.b, 0);
        mat.SetColor(emmisionColourName, colourVector * t);
    }*/
}
