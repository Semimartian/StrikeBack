using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraStates:byte
{
    Running,Action,Boss
}

[System.Serializable]
public struct CameraStateProperties 
{
    public CameraStates name;
    public Transform anchorTransform;
}

public struct CameraAnchor
{
    public Vector3 positionOffset;
    public Quaternion rotation;
    public CameraAnchor(Transform anchorTransform)
    {
        positionOffset = anchorTransform.localPosition;
        rotation = anchorTransform.localRotation;
    }
}

public class MainCamera : MonoBehaviour
{
    public static MainCamera instance;

    private Transform myTransform;
    private CameraAnchor anchor;
    [SerializeField] private CameraStateProperties[] cameraStates;
    [SerializeField] private Transform target;
    [SerializeField] private AnimationCurve transitionCurve;
    private bool isTransitioning = false;

    private void Awake()
    {
        instance = this;
        anchor = GetAnchor(CameraStates.Running);

        myTransform = transform;
    }

    private CameraAnchor GetAnchor(CameraStates name)
    {
        for (int i = 0; i < cameraStates.Length; i++)
        {
            if(name == cameraStates[i].name)
            {
                return new CameraAnchor(cameraStates[i].anchorTransform);
            }
        }

        Debug.LogError("Could not find state!");
        return new CameraAnchor();
    }

    private void FixedUpdate()
    {
        if (!isTransitioning)
        {
            myTransform.position = 
              (target.position + anchor.positionOffset);
        }
    }

    public void TransitionTo(CameraStates state)
    {
        anchor = GetAnchor(state);
        StartCoroutine(TransitionCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {
        isTransitioning = true;
        Vector3 originalPosition = myTransform.position;
        Quaternion originalRotation = myTransform.rotation;

        float time = 0;
        float endTime = transitionCurve.keys[transitionCurve.length - 1].time;

        while (time < endTime)
        {
            //float deltaTime = Time.deltaTime;
            time += Time.fixedDeltaTime;

            float t = transitionCurve.Evaluate(time);
            Vector3 anchorPosition = (target.position + anchor.positionOffset);
            myTransform.position = Vector3.Lerp(originalPosition, anchorPosition, t);
            // Vector3.MoveTowards(transform.position, target.position, speed * deltaTime);
            myTransform.rotation = Quaternion.Lerp(originalRotation, anchor.rotation, t);
            //Quaternion.RotateTowards(transform.rotation, target.rotation, rotationSpeed * deltaTime);

            yield return new WaitForFixedUpdate();

        }

        isTransitioning = false;

    }
}
