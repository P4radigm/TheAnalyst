using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasHelpers;
using TouchBehaviours;

public class CameraTarget : MonoBehaviour
{
    public Transform InitialTarget;
    public Transform StartPosition;
    public float TravelSpeed = 5f;
    Transform currentTarget;
    Transform nextTarget;
    private TouchInputManager tIM;
    private GameManager gM;
    private InteractableObject currentTargetObject;

    private float distanceToCam;

    void Start()
    {
        gM = GameManager.Instance;
        currentTarget = InitialTarget;
        transform.position = StartPosition.position;
        //transform.parent = currentTarget;
    }

    public void MoveToNewTarget(Transform newTarget, float time, float _distanceToCam, GameObject _targetObject, TouchInputManager _tIM)
    {
        if(_tIM != null)
        {
            tIM = _tIM;
        }

        if(_targetObject != null)
        {
            if (_targetObject.GetComponent<InteractableObject>() != null)
            {
                currentTargetObject = _targetObject.GetComponent<InteractableObject>();
            }
            else { currentTargetObject = null; }
        }
        else { currentTargetObject = null; }


        transform.parent = null;
        nextTarget = newTarget;
        distanceToCam = _distanceToCam;
        transform.LerpTransform(this, nextTarget.position, TravelSpeed);

        StartCoroutine(MoveCoroutine(time));
    }

    public void MoveToOldTarget(float time)
    {
        transform.parent = null;

        if(currentTargetObject != null)
        {
            if (currentTargetObject.Back != null)
            {
                nextTarget = currentTargetObject.Back.PrefferedCamTransformPosition;
                //Debug.Log($"currentTargetObject.Back = {currentTargetObject.Back}");
                //Debug.Log($"tIM.target = {currentTargetObject.Back.gameObject}");
                gM.ReInitializeInspectables(currentTargetObject.Back.gameObject);
                tIM.target = currentTargetObject.Back.gameObject;
                tIM.DubbleTapedOnInspectable();
                currentTargetObject = currentTargetObject.Back;
                return;
            }
            else { nextTarget = StartPosition; }
        }
        else { nextTarget = StartPosition; }

        //Debug.LogWarning($"nextTraget = {nextTarget}");

        transform.LerpTransform(this, nextTarget.position, TravelSpeed);

        gM.ReInitializeInspectables(null);
        //StartCoroutine(MoveCoroutine(time));
    }

    IEnumerator MoveCoroutine(float timeToMove)
    {
        float currentTime = 0;
        Vector3 distanceVector = transform.position - nextTarget.position;
        Vector3 distanceVectorNormalized = distanceVector.normalized;
        //Vector3 targetPosition = new Vector3(nextTarget.position.x, nextTarget.position.y, nextTarget.position.z);
        Debug.Log("Current: " + currentTarget + currentTarget.rotation);
        Debug.Log("Next: " + nextTarget + nextTarget.rotation);
        while (currentTime < timeToMove)
        {
            float currentDistance = currentTime / timeToMove;
            //transform.position = Vector3.Lerp(currentTarget.position, nextTarget.position, currentDistance);
            //transform.LerpTransform(this, nextTarget.position, currentDistance);
            transform.rotation = Quaternion.Slerp(currentTarget.rotation, nextTarget.rotation, currentDistance);
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.parent = nextTarget;
        transform.position = nextTarget.position;
        transform.rotation = nextTarget.rotation;
    }
}
