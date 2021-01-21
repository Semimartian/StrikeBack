using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraOrientations:byte
{
    Running,Action,Boss
}

[System.Serializable]
public struct CameraOrientationProperties 
{
    public CameraOrientations name;
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
    [SerializeField] private CameraOrientationProperties[] cameraOrientations;
    [SerializeField] private Transform target;
    [SerializeField] private AnimationCurve transitionCurve;
    private enum CameraStates
    {
         Static, FollowingTarget , Transitioning,
    }
    private CameraStates state = CameraStates.FollowingTarget;

    private void Awake()
    {
        instance = this;
        anchor = GetAnchor(CameraOrientations.Running);

        myTransform = transform;
    }

    private CameraAnchor GetAnchor(CameraOrientations name)
    {
        for (int i = 0; i < cameraOrientations.Length; i++)
        {
            if(name == cameraOrientations[i].name)
            {
                return new CameraAnchor(cameraOrientations[i].anchorTransform);
            }
        }

        Debug.LogError("Could not find state!");
        return new CameraAnchor();
    }

    private void FixedUpdate()
    {
        if (state == CameraStates.FollowingTarget)
        {
            myTransform.position = 
              (target.position + anchor.positionOffset);
        }
    }

    public void SetOrientation(CameraOrientations orientation)
    {
        anchor = GetAnchor(orientation);
        StartCoroutine(OrientateCoroutine());
    }

    private IEnumerator OrientateCoroutine()
    {
        state = CameraStates.Transitioning;
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

        state = CameraStates.FollowingTarget;

    }

    public void GoToStaticDestination(Transform destination)
    {
        StartCoroutine(GoToStaticDestinationCoroutine(destination));
    }

    private IEnumerator GoToStaticDestinationCoroutine(Transform destination)
    {
        state = CameraStates.Transitioning;
        Vector3 originalPosition = myTransform.position;
        Quaternion originalRotation = myTransform.rotation;
        Vector3 destinationPosition = destination.position;
        Quaternion destinationRotation = destination.rotation;

        float time = 0;
        float endTime = transitionCurve.keys[transitionCurve.length - 1].time;

        while (time < endTime)
        {
            //float deltaTime = Time.deltaTime;
            time += Time.fixedDeltaTime;

            float t = transitionCurve.Evaluate(time);
            myTransform.position = Vector3.Lerp(originalPosition, destinationPosition, t);
            myTransform.rotation = Quaternion.Lerp(originalRotation, destinationRotation, t);

            yield return new WaitForFixedUpdate();

        }

        state = CameraStates.Static;
    }
}
