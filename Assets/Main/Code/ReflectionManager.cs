using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionManager : MonoBehaviour
{
    private Camera reflectionCamera;
    private Camera mainCamera;
   [SerializeField] private GameObject reflectionPlane;
    RenderTexture renderTarget;
    [SerializeField] private Material reflectiveMat;
    [Range(0,1)]
    [SerializeField] private float reflectionFactor = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject reflectionCameraGO = new GameObject("ReflectionCamera");
        reflectionCamera = reflectionCameraGO.AddComponent<Camera>();
        reflectionCamera.enabled = false;

        mainCamera = Camera.main;

        renderTarget = new RenderTexture(Screen.width, Screen.height, 24);
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_reflectionFactor",reflectionFactor);
    }

    private void OnPostRender()
    {
        RenderReflection();
    }

    private void RenderReflection()
    {
        reflectionCamera.CopyFrom(mainCamera);
        Transform mainCameraTransform = mainCamera.transform;

        Vector3 cameraDirectionWorldSpace = mainCameraTransform.forward;
        Vector3 cameraUpWorldSpace = mainCameraTransform.up;
        Vector3 cameraPositionWorldSpace = mainCameraTransform.position;

        Transform reflectionPlaneTransform = reflectionPlane.transform;

        Vector3 cameraDirectionPlaneSpace = reflectionPlaneTransform.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = reflectionPlaneTransform.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = reflectionPlaneTransform.InverseTransformPoint(cameraPositionWorldSpace);

        cameraDirectionPlaneSpace.y *= -1;
        cameraUpPlaneSpace.y *= -1;
        cameraPositionPlaneSpace.y *= -1;

        cameraDirectionWorldSpace = reflectionPlaneTransform.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = reflectionPlaneTransform.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = reflectionPlaneTransform.TransformPoint(cameraPositionPlaneSpace);

        Transform reflectionCameraTransform = reflectionCamera.transform;

        reflectionCameraTransform.position = cameraPositionWorldSpace;
        reflectionCameraTransform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);

        reflectionCamera.targetTexture = renderTarget;

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
