using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum scienceDeskStates
{
    puzzleOne,
    puzzleTwo,
    puzzleThree,
    puzzleFour,
    Solved
}

public class ScienceDeskController : MonoBehaviour
{
    public static ScienceDeskController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [HideInInspector] public City currentCity;
    [HideInInspector] public City lastCity;

    [SerializeField] private AvatarController avatarController;
    public Transform prefCamPosScienceDesk;
    private bool isInitiated = false;
    [HideInInspector] public bool avatarIsInCity;

    [Header("State Changes")]
    public scienceDeskStates state;
    [SerializeField] private int[] hourPoolPerState;
    [SerializeField] private RoadLock[] correctRouteOne;
    [SerializeField] private RoadLock[] correctRouteTwo;
    [SerializeField] private RoadLock[] correctRouteThree;
    [SerializeField] private RoadLock[] correctRouteFour;
    [SerializeField] private GameObject[] parentCitiesPerState;
    [SerializeField] private GameObject[] parentRoadsPerState;
    public List<RoadLock> currentRoute;
    public bool routeIsSame;

    [Header("Hour Pool Calculation")]
    public int hourPool;
    [SerializeField] private float counterDegreesPerSide;
    [SerializeField] private float counterAnimDurationPerSide;
    [SerializeField] private float maxAnimDuration;
    [SerializeField] private AnimationCurve counterAnimCurve;
    [SerializeField] private GameObject[] rollersPuzzle1;
    [SerializeField] private GameObject[] rollersPuzzle2;
    [SerializeField] private GameObject[] rollersPuzzle3;
    [SerializeField] private GameObject[] rollersPuzzle4;
    private GameObject[][] rollersPerState;
    private Coroutine counterAnimRoutine;

    [Header("Main Animation Settings")]
    [SerializeField] private Animator mainAnimator;
    [SerializeField] private string[] puzzleSolvedAnimPerState;
    [SerializeField] private GameObject avatarModelPrefab;
    [SerializeField] private Transform puzzleRotate;
    [SerializeField] private Transform[] avatarStartPositionPerState;

    [Header("Road Lock Animation Settings")]
    public float[] yStartPerState;
    public float[] yEndPerState;
    public float avatarRoadLockDifference;
    public float upAnimTime;
    public AnimationCurve upAnimCurve;
    public float downAnimTime;
    public AnimationCurve downAnimCurve;
    public Material nonPathMat;
    public Material pathMat;

    [Header("King Anim Settings")]
    [SerializeField] private float kingAnimDuration;
    private Vector3 kingPos;
    private Quaternion kingRot;
    [SerializeField] private Transform kingTransform;
    [SerializeField] private AnimationCurve kingCurve;

    private void Start()
    {
        kingPos = kingTransform.position;
        kingRot = kingTransform.rotation;

        avatarController = GetComponent<AvatarController>();
        currentRoute = new List<RoadLock>();
        currentRoute.Clear();
        rollersPerState = new GameObject[4][];
        rollersPerState[0] = rollersPuzzle1;
        rollersPerState[1] = rollersPuzzle2;
        rollersPerState[2] = rollersPuzzle3;
        rollersPerState[3] = rollersPuzzle4;
    }

    public void InitiateScienceDesk()
    {
        state = scienceDeskStates.puzzleOne;
        hourPool = hourPoolPerState[0];
        UpdateHourCounter(state, hourPool);
        mainAnimator.enabled = true;
        StartCoroutine(InitiateSceinceDeskIE());
    }

    private IEnumerator InitiateSceinceDeskIE()
    {
        float _animTime = 0;

        RuntimeAnimatorController ac = mainAnimator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == puzzleSolvedAnimPerState[(int)scienceDeskStates.puzzleOne])
            {
                _animTime = ac.animationClips[i].length;
            }
        }

        yield return new WaitForSeconds(_animTime-avatarController.riseAnimDuration);

        avatarController.avatarModel.transform.position = new Vector3(avatarStartPositionPerState[(int)state].position.x, yEndPerState[(int)state] + avatarRoadLockDifference - avatarController.riseDistance, avatarStartPositionPerState[(int)state].position.z);
        avatarController.avatarInteractable.transform.position = new Vector3(avatarStartPositionPerState[(int)state].position.x, yEndPerState[(int)state] + avatarRoadLockDifference - avatarController.riseDistance, avatarStartPositionPerState[(int)state].position.z);
        avatarController.StartRiseAnim();

        yield return new WaitForSeconds(avatarController.riseAnimDuration);

        avatarController.avatarInteractable.SetActive(true);
    }

    private IEnumerator UpdatePhase(scienceDeskStates _currentState, scienceDeskStates _nextState)
    {
        yield return new WaitForSeconds(0.8f);

        state = _nextState;

        if (_nextState != scienceDeskStates.Solved)
        {
            avatarController.stopRoutine = true;
            Instantiate(avatarModelPrefab, avatarController.avatarModel.transform.position, avatarController.avatarModel.transform.rotation, puzzleRotate);

            avatarController.avatarModel.SetActive(false);
            avatarController.avatarInteractable.SetActive(false);

            avatarController.avatarModel.transform.position = avatarStartPositionPerState[(int)_nextState].position;
            avatarController.avatarInteractable.transform.position = avatarStartPositionPerState[(int)_nextState].position;
        }
        else
        {
            avatarController.enabled = false;
            StartCoroutine(KingAnimIE());
        }
        
        parentCitiesPerState[(int)_currentState].transform.SetParent(puzzleRotate);
        parentRoadsPerState[(int)_currentState].transform.SetParent(puzzleRotate);

        avatarController.avatarSelected = false;
        avatarController.isAnimating = true;


        mainAnimator.SetInteger("state", (int)_nextState);

        float _animTime = 0;

        RuntimeAnimatorController ac = mainAnimator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if(ac.animationClips[i].name == puzzleSolvedAnimPerState[(int)_nextState])
            {
                _animTime = ac.animationClips[i].length;
                Debug.Log(ac.animationClips[i].name);
            }
        }

        yield return new WaitForSeconds(_animTime - avatarController.riseAnimDuration);
        
        if (_nextState != scienceDeskStates.Solved)
        {
            avatarController.avatarModel.transform.position = new Vector3(avatarStartPositionPerState[(int)state].position.x, yEndPerState[(int)state] + avatarRoadLockDifference - avatarController.riseDistance, avatarStartPositionPerState[(int)state].position.z);
            avatarController.avatarInteractable.transform.position = new Vector3(avatarStartPositionPerState[(int)state].position.x, yEndPerState[(int)state] + avatarRoadLockDifference - avatarController.riseDistance, avatarStartPositionPerState[(int)state].position.z);
            avatarController.StartRiseAnim();
        }

        yield return new WaitForSeconds(avatarController.riseAnimDuration);

        UpdatePhaseVoid(_currentState, _nextState);
    }

    private void UpdatePhaseVoid(scienceDeskStates _currentState, scienceDeskStates _nextState)
    {
        if (_nextState != scienceDeskStates.Solved)
        {
            StopAllCoroutines();
            hourPool = hourPoolPerState[(int)_nextState];
            UpdateHourCounter(_nextState, hourPool);
            avatarController.avatarModel.SetActive(true);
            avatarController.avatarInteractable.SetActive(true);
            parentCitiesPerState[(int)_nextState].SetActive(true);
            parentRoadsPerState[(int)_nextState].SetActive(true);
            currentRoute.Clear();
            avatarController.isAnimating = false;
            avatarController.stopRoutine = false;
        }
        else
        {
            
        }
    }

    private IEnumerator KingAnimIE()
    {
        float _timeValue = 0;
        Vector3 _startPos = prefCamPosScienceDesk.transform.position;
        Quaternion _startRot = prefCamPosScienceDesk.rotation;

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / kingAnimDuration;
            float _evaluatedTimeValue = kingCurve.Evaluate(_timeValue);
            Vector3 _newPos = Vector3.Lerp(_startPos, kingPos, _evaluatedTimeValue);
            Quaternion _newRot = Quaternion.Lerp(_startRot, kingRot, _evaluatedTimeValue);

            prefCamPosScienceDesk.transform.position = _newPos;
            prefCamPosScienceDesk.transform.rotation = _newRot;

            yield return null;
        }
    }

    public void AvatarHasEnteredCity(City _city)
    {
        if(currentCity != null && currentCity != lastCity)
        {
            lastCity = currentCity;
        }

        currentCity = _city;

        UpdateRoadLocks(currentCity, lastCity);
    }

    private void UpdateRoadLocks(City _current, City _last)
    {
        if(_last != null && _current != null && _last != _current)
        {
            if (_current.isVisited)
            {
                _last.isVisited = false;
                _current.isVisited = false;

                for (int i = 0; i < _last.connectedRoadLocks.Length; i++)
                {
                    if (_last.connectedRoadLocks[i].isPartOfPath)
                    {
                        _last.connectedRoadLocks[i].startGoDown();
                    }
                }

                for (int i = 0; i < _current.connectedRoadLocks.Length; i++)
                {
                    if (!_current.connectedRoadLocks[i].isPartOfPath)
                    {
                        _current.connectedRoadLocks[i].startGoDown();
                    }
                }
            }
            else
            {
                RoadLock _goldenRoad = null;

                for (int i = 0; i < _last.connectedRoadLocks.Length; i++)
                {

                    for (int j = 0; j < _current.connectedRoadLocks.Length; j++)
                    {
                        if (_last.connectedRoadLocks[i] == _current.connectedRoadLocks[j])
                        {
                            _goldenRoad = _last.connectedRoadLocks[i];
                            //Debug.Log("Gold");
                        }
                    }
                }

                if(_goldenRoad != null)
                {
                    _goldenRoad.startGoUp(true);
                }

                for (int i = 0; i < _last.connectedRoadLocks.Length; i++)
                {
                    if (!_last.connectedRoadLocks[i].isUp)
                    {
                        _last.connectedRoadLocks[i].startGoUp(false);
                    }
                }
            }
        }
    }

    public void TooExpensive()
    {
        avatarController.StartReturnAnim(lastCity);
    }

    public void UpdateHourCounter(scienceDeskStates _currentState, float _currentScore)
    {
        if(counterAnimRoutine != null) { StopCoroutine(counterAnimRoutine); }
        counterAnimRoutine = StartCoroutine(UpdateHourCounterIE(_currentState, _currentScore));
    }

    private IEnumerator UpdateHourCounterIE(scienceDeskStates _currentState, float _currentPool)
    {
        GameObject _tenCounter = rollersPerState[(int)_currentState][0];
        GameObject _oneCounter = rollersPerState[(int)_currentState][1];

        float _currentTenDegrees = _tenCounter.transform.localEulerAngles.z;
        float _currentOneDegrees = _oneCounter.transform.localEulerAngles.z;
        //Debug.Log($"_currentTenDegrees = {_currentTenDegrees} & _currentOneDegrees = {_currentOneDegrees}");

        float _newTenDegrees = Mathf.Floor(_currentPool / 10) * counterDegreesPerSide*-1;
        float _newOneDegrees = (_currentPool % 10) * counterDegreesPerSide*-1;

        float _tenAnimDifference = Mathf.Floor(_currentPool / 10) - (_currentTenDegrees / counterDegreesPerSide);
        float _oneAnimDifference = (_currentPool % 10) - (_currentOneDegrees / counterDegreesPerSide);

        float _tenAnimDuration = Mathf.Abs(_tenAnimDifference)*counterAnimDurationPerSide;
        float _oneAnimDuration = Mathf.Abs(_oneAnimDifference) * counterAnimDurationPerSide;

        float _duration;

        if(_tenAnimDuration < _oneAnimDuration) {
            _duration = _oneAnimDuration; 
        }
        else { 
            _duration = _tenAnimDuration; 
        }

        if(_duration > maxAnimDuration)
        {
            _duration = maxAnimDuration;
        }

        float _timeValue = 0;

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / _duration;
            float _evaluatedTimeValue = counterAnimCurve.Evaluate(_timeValue);

            float _newZDegreesTen = Mathf.Lerp(_currentTenDegrees, _newTenDegrees, _evaluatedTimeValue);
            float _newZDegreesOne = Mathf.Lerp(_currentOneDegrees, _newOneDegrees, _evaluatedTimeValue);

            _tenCounter.transform.localEulerAngles = new Vector3(_tenCounter.transform.localEulerAngles.x, _tenCounter.transform.localEulerAngles.y, _newZDegreesTen);
            _oneCounter.transform.localEulerAngles = new Vector3(_oneCounter.transform.localEulerAngles.x, _oneCounter.transform.localEulerAngles.y, _newZDegreesOne);

            yield return null;
        }

        yield return null;
    }

    public void WinCheck()
    {
        RoadLock[] _currentRouteArray;
        _currentRouteArray = currentRoute.ToArray();

        if(state == scienceDeskStates.puzzleOne)
        {
            if(_currentRouteArray.Length == correctRouteOne.Length)
            {
                for (int i = 0; i < _currentRouteArray.Length; i++)
                {
                    if (_currentRouteArray[i] != correctRouteOne[i])
                    {
                        return;
                    }
                }

                Debug.LogWarning("Finished Phase One");
                StartCoroutine(UpdatePhase(state, scienceDeskStates.puzzleTwo));
            }
        }
        else if(state == scienceDeskStates.puzzleTwo)
        {
            if (_currentRouteArray.Length == correctRouteTwo.Length)
            {
                for (int i = 0; i < _currentRouteArray.Length; i++)
                {
                    if (_currentRouteArray[i] != correctRouteTwo[i])
                    {
                        return;
                    }
                }

                Debug.LogWarning("Finished Phase Two");
                StartCoroutine(UpdatePhase(state, scienceDeskStates.puzzleThree));
            }
        }
        else if(state == scienceDeskStates.puzzleThree)
        {
            if (_currentRouteArray.Length == correctRouteThree.Length)
            {
                for (int i = 0; i < _currentRouteArray.Length; i++)
                {
                    if (_currentRouteArray[i] != correctRouteThree[i])
                    {
                        return;
                    }
                }

                Debug.LogWarning("Finished Phase Three");
                StartCoroutine(UpdatePhase(state, scienceDeskStates.puzzleFour));
            }
        }
        else if(state == scienceDeskStates.puzzleFour)
        {
            if (_currentRouteArray.Length == correctRouteFour.Length)
            {
                for (int i = 0; i < _currentRouteArray.Length; i++)
                {
                    if (_currentRouteArray[i] != correctRouteFour[i])
                    {
                        return;
                    }
                }

                Debug.LogWarning("Finished Phase Four");
                StartCoroutine(UpdatePhase(state, scienceDeskStates.Solved));
            }
        }
        else
        {
            Debug.LogError("State out of bounds");
        }
    }

    private void Update()
    {
        if(prefCamPosScienceDesk.childCount != 0)
        {
            if (!isInitiated)
            {
                isInitiated = true;
                InitiateScienceDesk();
            }
            if (!avatarController.enabled)
            {
                avatarController.enabled = true;
            }
        }
        else
        {
            if (avatarController.enabled)
            {
                avatarController.enabled = false;
            }
        }
    }
}
