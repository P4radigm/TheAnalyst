using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class AvatarController : MonoBehaviour
{
    private void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUp -= HandleFingerUp;
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    private ScienceDeskController sdC;
    [SerializeField] private LayerMask interactableLayerMask;


    [Header("Player Input")]
    [SerializeField] private float clampValue;
    public GameObject avatarInteractable;
    public GameObject avatarModel;
    [SerializeField] private Collider interactionCollider;
    public Collider avatarInteractableCollider;
    private Camera mainCam;
    public bool avatarSelected;
    private Vector2 currentHitPoint;

    [Header("Return Animation")]
    public bool isAnimating;
    [SerializeField] float returnAnimDuration;
    [SerializeField] AnimationCurve returnAnimCurve;
    [SerializeField] float centerAnimDuration;
    [SerializeField] AnimationCurve centerAnimCurve;
    private Coroutine centerAnimRoutine;
    public bool stopRoutine;

    [Header("Rise Animation")]
    public float riseDistance;
    public float riseAnimDuration;
    [SerializeField] AnimationCurve riseAnimCurve;
    private Coroutine riseAnimRoutine;

    void Start()
    {
        sdC = ScienceDeskController.instance;
        mainCam = Camera.main;
        avatarSelected = false;
        isAnimating = false;
        avatarInteractableCollider = avatarInteractable.GetComponentInChildren<Collider>();
    }

    private void HandleFingerDown(LeanFinger finger)
    {
        if (finger.IsOverGui)
        {
            Debug.Log("You just tapped the screen on top of the GUI!");
        }
        else
        {
            Ray r = mainCam.ScreenPointToRay(finger.ScreenPosition);
            //Debug.DrawRay(mainCam.transform.position, finger.GetLastWorldPosition(4f, mainCam), Color.magenta, 2f);
            RaycastHit hit;

            if(Physics.Raycast(r, out hit, Mathf.Infinity, interactableLayerMask))
            {

                Debug.Log($"RayCast hit {hit}");

                if (hit.transform.tag == "AvatarInteractable")
                {
                    avatarSelected = true;
                    Debug.Log("Avatar Selected");
                    avatarInteractableCollider.enabled = false;
                }
            }
        }
    }

    private void HandleFingerUp(LeanFinger finger)
    {
        interactionCollider.enabled = false;

        Debug.Log("Avatar Deselected");
        if (sdC.avatarIsInCity)
        {
            StartCenterAnim();
        }

        avatarSelected = false;
        avatarInteractableCollider.enabled = true;
        avatarInteractable.GetComponent<Rigidbody>().velocity = Vector3.zero;
        avatarModel.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void HandleFingerUpdate(LeanFinger finger)
    {
        if (avatarSelected)
        {
            interactionCollider.enabled = true;

            if (!isAnimating)
            {
                Vector2 _modelPos2D = new Vector2(avatarModel.transform.position.x, avatarModel.transform.position.z);

                Ray r = mainCam.ScreenPointToRay(finger.ScreenPosition);
                RaycastHit hit;


                if (Physics.Raycast(r, out hit, Mathf.Infinity, interactableLayerMask))
                {
                    currentHitPoint = new Vector2(hit.point.x, hit.point.z);
                }

                //Clamping hitpoint, so that the interactable can't get too far from the model
                //this makes the spring joint more stable and makes sure the avatar can't escape the arena
                Vector2 _moveDirection = currentHitPoint - _modelPos2D;
                _moveDirection.Normalize();
                float _moveDistance = Mathf.Clamp(Vector2.Distance(currentHitPoint, _modelPos2D), 0f, clampValue);
                Vector2 _newInteractablePos = _modelPos2D + (_moveDirection * _moveDistance);

                //Update avatarinteractable position to clamped hitpoint
                avatarInteractable.transform.position = new Vector3(_newInteractablePos.x, avatarInteractable.transform.position.y, _newInteractablePos.y);
            }
        }
    }

    public void StartReturnAnim(City _returnCity)
    {
        isAnimating = true;
        if(centerAnimRoutine != null) { StopCoroutine(centerAnimRoutine); }
        centerAnimRoutine = StartCoroutine(CenterAnimIE(_returnCity, returnAnimDuration, returnAnimCurve));
    }

    public void StartCenterAnim()
    {
        isAnimating = true;
        if (centerAnimRoutine == null) { centerAnimRoutine = StartCoroutine(CenterAnimIE(sdC.currentCity, centerAnimDuration, centerAnimCurve)); }
    }

    private IEnumerator CenterAnimIE(City _returnCity, float _duration, AnimationCurve _animCurve)
    {
        float _timeValue = 0;
        Vector2 _start2DPos = new Vector2(avatarModel.transform.position.x, avatarModel.transform.position.z);
        Vector2 _returnCity2DPos = new Vector2(_returnCity.transform.position.x, _returnCity.transform.position.z);

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / _duration;
            float _evaluatedTimeValue = _animCurve.Evaluate(_timeValue);
            Vector2 _new2DPos = Vector2.Lerp(_start2DPos, _returnCity2DPos, _evaluatedTimeValue);

            avatarInteractable.transform.position = new Vector3(_new2DPos.x, avatarInteractable.transform.position.y, _new2DPos.y);

            yield return null;
        }

        isAnimating = false;
        centerAnimRoutine = null;
    }

    public void StartRiseAnim()
    {
        avatarInteractableCollider.enabled = false;
        if (riseAnimRoutine != null) { StopCoroutine(riseAnimRoutine); }
        riseAnimRoutine = StartCoroutine(RiseAnimIE());
    }

    private IEnumerator RiseAnimIE()
    {
        avatarModel.SetActive(true);
        //avatarModel.transform.position = new Vector3(avatarModel.transform.position.x, avatarModel.transform.position.y - riseDistance, avatarModel.transform.position.z);

        avatarModel.GetComponentInChildren<Collider>().enabled = false;
        float _timeValue = 0;
        float _startYPos = sdC.yEndPerState[(int)sdC.state] + sdC.avatarRoadLockDifference - riseDistance;

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / riseAnimDuration;
            float _evaluatedTimeValue = riseAnimCurve.Evaluate(_timeValue);
            float _newYPos = Mathf.Lerp(_startYPos, _startYPos + riseDistance, _evaluatedTimeValue);

            avatarModel.transform.localPosition = new Vector3(avatarModel.transform.localPosition.x, _newYPos, avatarModel.transform.localPosition.z);
            avatarInteractable.transform.localPosition = new Vector3(avatarModel.transform.localPosition.x, _newYPos, avatarModel.transform.localPosition.z);

            yield return null;
        }

        riseAnimRoutine = null;
        avatarModel.GetComponentInChildren<Collider>().enabled = true;
        avatarInteractableCollider.enabled = true;
    }

    private void Update()
    {
        if(centerAnimRoutine != null & stopRoutine)
        {
            StopCoroutine(centerAnimRoutine);
            centerAnimRoutine = null;
        }
    }
}
