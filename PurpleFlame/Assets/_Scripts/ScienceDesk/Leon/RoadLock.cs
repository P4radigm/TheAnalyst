using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLock : MonoBehaviour
{
    [HideInInspector] public bool isUp;
    [HideInInspector] public bool isPartOfPath;

    public City[] connectedCities;

    [SerializeField] private int cost;

    [SerializeField] private Collider connectedCollider;
       
    private ScienceDeskController sdC;

    private Coroutine goUpRoutine;
    private Coroutine goDownRoutine;

    private void Start()
    {
        sdC = ScienceDeskController.instance;
        goUpRoutine = null;
        goDownRoutine = null;
        isUp = false;
        isPartOfPath = false;
        transform.localPosition = new Vector3(transform.localPosition.x, sdC.yStartPerState[(int)sdC.state], transform.localPosition.z);
    }

    public void startGoUp(bool _gold)
    {
        isUp = true;

        if(sdC.hourPool - cost < 0 && _gold)
        {
            sdC.TooExpensive();
            return;
        }

        if (_gold)
        {
            GetComponent<Renderer>().material = sdC.pathMat;
            isPartOfPath = true;
            connectedCollider.enabled = false;
            sdC.hourPool -= cost;
            sdC.UpdateHourCounter(sdC.state, sdC.hourPool);
            sdC.currentRoute.Add(this);
            sdC.WinCheck();
        }
        else
        {
            GetComponent<Renderer>().material = sdC.nonPathMat;
            isPartOfPath = false;
            connectedCollider.enabled = true;
        }

        if (goUpRoutine != null) { StopCoroutine(goUpRoutine); }
        if (goDownRoutine != null) { StopCoroutine(goDownRoutine); }
        goUpRoutine = StartCoroutine(goUpIE());
    }

    public void startGoDown()
    {
        for (int i = 0; i < connectedCities.Length; i++)
        {
            if (connectedCities[i].isVisited)
            {
                return;
            }
        }

        if (isPartOfPath)
        {
            sdC.hourPool += cost;
            sdC.UpdateHourCounter(sdC.state, sdC.hourPool);
            isPartOfPath = false;
            sdC.currentRoute.Remove(this);
        }
        isUp = false;

        connectedCollider.enabled = false;

        if (goUpRoutine != null) { StopCoroutine(goUpRoutine); }
        if(goDownRoutine != null) { StopCoroutine(goDownRoutine); }
        goDownRoutine = StartCoroutine(goDownIE());
    }

    private IEnumerator goUpIE()
    {
        float _timeValue = 0;
        float _startY = transform.localPosition.y;

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / sdC.upAnimTime;
            float _evaluatedTimeValue = sdC.upAnimCurve.Evaluate(_timeValue);
            float _newYPos = Mathf.Lerp(_startY, sdC.yEndPerState[(int)sdC.state], _evaluatedTimeValue);
            
            transform.localPosition = new Vector3(transform.localPosition.x, _newYPos, transform.localPosition.z);
        
            yield return null;
        }

        goUpRoutine = null;
    }

    private IEnumerator goDownIE()
    {
        float _timeValue = 0;
        float _startY = transform.localPosition.y;

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / sdC.downAnimTime;
            float _evaluatedTimeValue = sdC.downAnimCurve.Evaluate(_timeValue);
            float _newYPos = Mathf.Lerp(_startY, sdC.yStartPerState[(int)sdC.state], _evaluatedTimeValue);

            transform.localPosition = new Vector3(transform.localPosition.x, _newYPos, transform.localPosition.z);
            
            yield return null;
        }

        goDownRoutine = null;
    }
}
