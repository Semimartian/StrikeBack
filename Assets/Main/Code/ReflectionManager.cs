using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ReflectionManager : MonoBehaviour
{
    [SerializeField] private Camera reflectionCamera;
    private Camera mainCamera;
   [SerializeField] private GameObject reflectionPlane;
    RenderTexture renderTarget;
    [SerializeField] private Material reflectiveMat;
    [Range(0,1)]
    [SerializeField] private float reflectionFactor = 0.5f;

    //Cache:
    Transform mainCameraTransform;
    Transform reflectionPlaneTransform;
    Transform reflectionCameraTransform;

    void Start()
    {
        InitialiseReflectionCamera();
    }

    void Update()
    {
        //TODO: I don't like:
        Shader.SetGlobalFloat("_reflectionFactor",reflectionFactor);
    }

    private void OnPostRender()
    {
        RenderReflection();
    }

    private void InitialiseReflectionCamera()
    {
        mainCamera = Camera.main;

        /*GameObject reflectionCameraGO = new GameObject("ReflectionCamera");
        reflectionCamera = reflectionCameraGO.AddComponent<Camera>();*/
        reflectionCamera.enabled = false;
        //reflectionCamera.CopyFrom(mainCamera);

        /*PostProcessLayer mainCameraPostProcessLayer = mainCamera.GetComponent<PostProcessLayer>();
        if (mainCameraPostProcessLayer != null)
        {
            //Copy component...
            PostProcessLayer reflectionCameraPostProcessLayer = reflectionCameraGO.AddComponent<PostProcessLayer>();
            reflectionCameraPostProcessLayer.volumeTrigger = mainCameraPostProcessLayer.volumeTrigger;
            reflectionCameraPostProcessLayer.volumeLayer = mainCameraPostProcessLayer.volumeLayer;
            reflectionCameraPostProcessLayer.antialiasingMode = mainCameraPostProcessLayer.antialiasingMode;
            reflectionCameraPostProcessLayer.stopNaNPropagation = mainCameraPostProcessLayer.stopNaNPropagation;
        }*/
        //NOTE: might be a bgood idea to play around with the resolution 
        renderTarget = new RenderTexture(Screen.width, Screen.height, 24);

        reflectionCamera.targetTexture = renderTarget;

        mainCameraTransform = mainCamera.transform;
        reflectionPlaneTransform = reflectionPlane.transform;
        reflectionCameraTransform = reflectionCamera.transform;

    }

    private void RenderReflection()
    {
       // reflectionCamera.CopyFrom(mainCamera);

        Vector3 cameraDirectionWorldSpace = mainCameraTransform.forward;
        Vector3 cameraUpWorldSpace = mainCameraTransform.up;
        Vector3 cameraPositionWorldSpace = mainCameraTransform.position;

        Vector3 cameraDirectionPlaneSpace = reflectionPlaneTransform.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = reflectionPlaneTransform.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = reflectionPlaneTransform.InverseTransformPoint(cameraPositionWorldSpace);

        cameraDirectionPlaneSpace.y *= -1;
        cameraUpPlaneSpace.y *= -1;
        cameraPositionPlaneSpace.y *= -1;

        cameraDirectionWorldSpace = reflectionPlaneTransform.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = reflectionPlaneTransform.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = reflectionPlaneTransform.TransformPoint(cameraPositionPlaneSpace);

        reflectionCameraTransform.position = cameraPositionWorldSpace;
        reflectionCameraTransform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);

        reflectionCamera.Render();

        DrawQuad();
    }

    private void DrawQuad()
    {
        GL.PushMatrix();
        reflectiveMat.SetPass(0);
        reflectiveMat.SetTexture("_ReflectionTex", renderTarget);

        GL.LoadOrtho();
        {
            GL.Begin(GL.QUADS);
            GL.TexCoord2(1, 0);
            GL.Vertex3(0, 0, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex3(0, 1, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex3(1, 1, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(1, 0, 0);

            GL.End();
        }
        
        GL.PopMatrix();
    }
}
